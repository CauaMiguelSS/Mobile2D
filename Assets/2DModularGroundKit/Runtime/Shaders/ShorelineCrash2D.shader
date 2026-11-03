Shader "Custom/ShorelineCrash2D"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _Color ("Tint", Color) = (1,1,1,1)

        _WaterSide ("Water Side", Float) = 1
        _WaveSpeed ("Wave Speed", Range(0.1,3)) = 0.7
        _CrashDistance ("Crash Distance", Range(1,6)) = 4
        _FoamStrength ("Foam Strength", Range(0,1)) = 0.8
        _WetDarkness ("Wet Darkness", Range(0,0.5)) = 0.22
        _Phase ("Phase", Float) = 0
        _WaveTime ("Wave Time", Float) = 0

        _SnowThreshold ("Snow Brightness Threshold", Range(0.5,1)) = 0.72
        _SnowWetMultiplier ("Snow Wet Darkness", Range(0,1)) = 0.15
        _SnowFoamColor ("Snow Foam Color", Color) = (0.55,0.88,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode"="Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _WaterSide;
                float _WaveSpeed;
                float _CrashDistance;
                float _FoamStrength;
                float _WetDarkness;
                float _Phase;
                float _WaveTime;

                float _SnowThreshold;
                float _SnowWetMultiplier;
                float4 _SnowFoamColor;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 positionOS : TEXCOORD1;
                half2 lightingUV : TEXCOORD2;
                half4 color : COLOR;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;

                o.positionHCS =
                    TransformObjectToHClip(v.positionOS);

                o.uv = v.uv;
                o.positionOS = v.positionOS.xy;

                o.lightingUV =
                    half2(
                        ComputeScreenPos(
                            o.positionHCS /
                            o.positionHCS.w
                        ).xy
                    );

                o.color = v.color;

                return o;
            }

            half4 Apply2DLighting(
                half4 color,
                float2 uv,
                half2 lightingUV)
            {
                SurfaceData2D surfaceData;
                InputData2D inputData;

                InitializeSurfaceData(
                    color.rgb,
                    color.a,
                    half4(1, 1, 1, 1),
                    surfaceData
                );

                InitializeInputData(
                    uv,
                    lightingUV,
                    inputData
                );

                return CombinedShapeLightShared(
                    surfaceData,
                    inputData
                );
            }

            float WaterAmount(half4 c)
            {
                float maxRGB =
                    max(c.r, max(c.g, c.b));

                float minRGB =
                    min(c.r, min(c.g, c.b));

                float saturation =
                    maxRGB - minRGB;

                float blueDominance =
                    c.b - max(c.r, c.g);

                float saturatedEnough =
                    smoothstep(
                        0.18,
                        0.32,
                        saturation
                    );

                float blueEnough =
                    smoothstep(
                        0.05,
                        0.16,
                        blueDominance
                    );

                return saturate(
                    saturatedEnough *
                    blueEnough
                );
            }

            float SnowAmount(half3 c)
            {
                float highest =
                    max(c.r, max(c.g, c.b));

                float lowest =
                    min(c.r, min(c.g, c.b));

                float brightness =
                    (c.r + c.g + c.b) / 3.0;

                float saturation =
                    highest - lowest;

                float bright =
                    smoothstep(
                        _SnowThreshold,
                        _SnowThreshold + 0.12,
                        brightness
                    );

                float neutral =
                    1.0 -
                    smoothstep(
                        0.12,
                        0.28,
                        saturation
                    );

                return saturate(
                    bright * neutral
                );
            }

            float Hash(float n)
            {
                return frac(
                    sin(n * 12.9898) *
                    43758.5453
                );
            }

            float2 ClampToSprite(
                float2 uv,
                float2 spriteMin,
                float2 spriteMax)
            {
                float2 halfPixel =
                    _MainTex_TexelSize.xy * 0.5;

                return clamp(
                    uv,
                    spriteMin + halfPixel,
                    spriteMax - halfPixel
                );
            }

            half4 SafeSample(
                float2 uv,
                float2 spriteMin,
                float2 spriteMax)
            {
                return SAMPLE_TEXTURE2D(
                    _MainTex,
                    sampler_MainTex,
                    ClampToSprite(
                        uv,
                        spriteMin,
                        spriteMax
                    )
                );
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 original =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        i.uv
                    )
                    * i.color
                    * _Color;

                const float spritePixels = 32.0;

                float2 local =
                    frac(
                        i.positionOS +
                        0.0001
                    );

                float2 spriteSizeUV =
                    _MainTex_TexelSize.xy *
                    spritePixels;

                float2 spriteMin =
                    i.uv -
                    local *
                    spriteSizeUV;

                float2 spriteMax =
                    spriteMin +
                    spriteSizeUV;

                float2 towardWater;
                float along;

                if (_WaterSide < 0.5)
                {
                    towardWater =
                        float2(0, 1);

                    along =
                        i.positionOS.x;
                }
                else if (_WaterSide < 1.5)
                {
                    towardWater =
                        float2(0, -1);

                    along =
                        i.positionOS.x;
                }
                else if (_WaterSide < 2.5)
                {
                    towardWater =
                        float2(-1, 0);

                    along =
                        i.positionOS.y;
                }
                else
                {
                    towardWater =
                        float2(1, 0);

                    along =
                        i.positionOS.y;
                }

                float currentWater =
                    WaterAmount(original);

                if (currentWater > 0.15)
                    return Apply2DLighting(
                        original,
                        i.uv,
                        i.lightingUV
                    );

                half4 s1 =
                    SafeSample(
                        i.uv +
                        towardWater *
                        _MainTex_TexelSize.xy,
                        spriteMin,
                        spriteMax
                    );

                half4 s2 =
                    SafeSample(
                        i.uv +
                        towardWater *
                        _MainTex_TexelSize.xy *
                        2.0,
                        spriteMin,
                        spriteMax
                    );

                half4 s3 =
                    SafeSample(
                        i.uv +
                        towardWater *
                        _MainTex_TexelSize.xy *
                        3.0,
                        spriteMin,
                        spriteMax
                    );

                half4 s4 =
                    SafeSample(
                        i.uv +
                        towardWater *
                        _MainTex_TexelSize.xy *
                        4.0,
                        spriteMin,
                        spriteMax
                    );

                half4 s5 =
                    SafeSample(
                        i.uv +
                        towardWater *
                        _MainTex_TexelSize.xy *
                        5.0,
                        spriteMin,
                        spriteMax
                    );

                half4 s6 =
                    SafeSample(
                        i.uv +
                        towardWater *
                        _MainTex_TexelSize.xy *
                        6.0,
                        spriteMin,
                        spriteMax
                    );

                float distanceToWater =
                    99.0;

                if (WaterAmount(s1) > 0.15)
                    distanceToWater = 1.0;
                else if (WaterAmount(s2) > 0.15)
                    distanceToWater = 2.0;
                else if (WaterAmount(s3) > 0.15)
                    distanceToWater = 3.0;
                else if (WaterAmount(s4) > 0.15)
                    distanceToWater = 4.0;
                else if (WaterAmount(s5) > 0.15)
                    distanceToWater = 5.0;
                else if (WaterAmount(s6) > 0.15)
                    distanceToWater = 6.0;

                if (distanceToWater > 6.0)
                    return Apply2DLighting(
                        original,
                        i.uv,
                        i.lightingUV
                    );

                float cycle =
                    frac(
                        (_WaveTime *
                        _WaveSpeed) +
                        _Phase
                    );

                float frame =
                    floor(
                        cycle *
                        8.0
                    );

                float waveAmount;

                if (frame < 1.0)
                    waveAmount = 0.0;
                else if (frame < 2.0)
                    waveAmount = 0.33;
                else if (frame < 3.0)
                    waveAmount = 0.66;
                else if (frame < 5.0)
                    waveAmount = 1.0;
                else if (frame < 6.0)
                    waveAmount = 0.66;
                else if (frame < 7.0)
                    waveAmount = 0.33;
                else
                    waveAmount = 0.0;

                float surge =
                    waveAmount *
                    _CrashDistance;

                float alongPixel =
                    floor(
                        along *
                        spritePixels
                    );

                float irregular =
                    Hash(
                        floor(
                            alongPixel /
                            2.0
                        )
                    );

                float localSurge =
                    surge +
                    (irregular - 0.5) *
                    1.15;

                float reached =
                    step(
                        distanceToWater,
                        localSurge
                    );

                float crest =
                    reached *
                    (
                        1.0 -
                        step(
                            0.9,
                            abs(
                                distanceToWater -
                                localSurge
                            )
                        )
                    );

                float breakup =
                    step(
                        0.15,
                        Hash(
                            alongPixel +
                            frame *
                            7.0
                        )
                    );

                crest *= breakup;

                float snow =
                    SnowAmount(
                        original.rgb
                    );

                half3 result =
                    original.rgb;

                float retreat =
                    step(
                        5.0,
                        frame
                    );

                float wet =
                    step(
                        distanceToWater,
                        _CrashDistance
                    );

                wet *=
                    max(
                        reached *
                        0.65,
                        retreat
                    );

                float effectiveWetDarkness =
                    lerp(
                        _WetDarkness,
                        _WetDarkness *
                        _SnowWetMultiplier,
                        snow
                    );

                result *=
                    1.0 -
                    wet *
                    effectiveWetDarkness;

                half3 normalFoam =
                    half3(
                        0.94,
                        0.98,
                        1.0
                    );

                half3 foamColor =
                    lerp(
                        normalFoam,
                        _SnowFoamColor.rgb,
                        snow
                    );

                result =
                    lerp(
                        result,
                        foamColor,
                        crest *
                        _FoamStrength
                    );

                return Apply2DLighting(
                    half4(
                        result,
                        original.a
                    ),
                    i.uv,
                    i.lightingUV
                );
            }

            ENDHLSL
        }
    }
}