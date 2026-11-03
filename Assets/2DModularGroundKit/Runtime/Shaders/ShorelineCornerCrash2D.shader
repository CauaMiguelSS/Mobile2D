Shader "Custom/ShorelineCornerCrash2D"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _WaveSpeed ("Wave Speed", Range(0.1,3)) = 0.7
        _CrashDistance ("Crash Distance", Range(1,6)) = 4
        _FoamStrength ("Foam Strength", Range(0,1)) = 0.8
        _WetDarkness ("Wet Darkness", Range(0,0.5)) = 0.22
        _Phase ("Phase", Float) = 0
        _WaveTime ("Wave Time", Float) = 0

        _SnowThreshold ("Snow Threshold", Range(0.5,1)) = 0.72
        _SnowWetMultiplier ("Snow Wet Multiplier", Range(0,1)) = 0.15
        _SnowFoamColor ("Snow Foam Color", Color) = (0.55,0.88,1,1)

        _CellPitch ("Cell Pitch", Vector) = (1,1,0,0)
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
                float _WaveSpeed;
                float _CrashDistance;
                float _FoamStrength;
                float _WetDarkness;
                float _Phase;
                float _WaveTime;

                float _SnowThreshold;
                float _SnowWetMultiplier;
                float4 _SnowFoamColor;

                float4 _CellPitch;
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
                float cornerCode : TEXCOORD2;
                half2 lightingUV : TEXCOORD3;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;

                o.positionHCS =
                    TransformObjectToHClip(v.positionOS);

                o.uv = v.uv;
                o.positionOS = v.positionOS.xy;
                o.cornerCode = v.color.r;

                o.lightingUV =
                    half2(
                        ComputeScreenPos(
                            o.positionHCS /
                            o.positionHCS.w
                        ).xy
                    );

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
                    _MainTex_TexelSize.xy *
                    0.5;

                return clamp(
                    uv,
                    spriteMin +
                    halfPixel,
                    spriteMax -
                    halfPixel
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

            float FindWaterDistance(
                float2 uv,
                float2 direction,
                float2 spriteMin,
                float2 spriteMax)
            {
                half4 s1 =
                    SafeSample(
                        uv +
                        direction *
                        _MainTex_TexelSize.xy,
                        spriteMin,
                        spriteMax
                    );

                half4 s2 =
                    SafeSample(
                        uv +
                        direction *
                        _MainTex_TexelSize.xy *
                        2.0,
                        spriteMin,
                        spriteMax
                    );

                half4 s3 =
                    SafeSample(
                        uv +
                        direction *
                        _MainTex_TexelSize.xy *
                        3.0,
                        spriteMin,
                        spriteMax
                    );

                half4 s4 =
                    SafeSample(
                        uv +
                        direction *
                        _MainTex_TexelSize.xy *
                        4.0,
                        spriteMin,
                        spriteMax
                    );

                half4 s5 =
                    SafeSample(
                        uv +
                        direction *
                        _MainTex_TexelSize.xy *
                        5.0,
                        spriteMin,
                        spriteMax
                    );

                half4 s6 =
                    SafeSample(
                        uv +
                        direction *
                        _MainTex_TexelSize.xy *
                        6.0,
                        spriteMin,
                        spriteMax
                    );

                if (WaterAmount(s1) > 0.15)
                    return 1.0;

                if (WaterAmount(s2) > 0.15)
                    return 2.0;

                if (WaterAmount(s3) > 0.15)
                    return 3.0;

                if (WaterAmount(s4) > 0.15)
                    return 4.0;

                if (WaterAmount(s5) > 0.15)
                    return 5.0;

                if (WaterAmount(s6) > 0.15)
                    return 6.0;

                return 99.0;
            }

            float GetWaveAmount(
                float frame)
            {
                if (frame < 1.0)
                    return 0.0;

                if (frame < 2.0)
                    return 0.33;

                if (frame < 3.0)
                    return 0.66;

                if (frame < 5.0)
                    return 1.0;

                if (frame < 6.0)
                    return 0.66;

                if (frame < 7.0)
                    return 0.33;

                return 0.0;
            }

            void EvaluateWave(
                float2 uv,
                float2 positionOS,
                float2 direction,
                float2 spriteMin,
                float2 spriteMax,
                float frame,
                float waveAmount,
                out float crest,
                out float wet)
            {
                float distanceToWater =
                    FindWaterDistance(
                        uv,
                        direction,
                        spriteMin,
                        spriteMax
                    );

                if (distanceToWater > 6.0)
                {
                    crest = 0.0;
                    wet = 0.0;

                    return;
                }

                float2 alongDirection =
                    float2(
                        -direction.y,
                        direction.x
                    );

                float alongPixel =
                    floor(
                        dot(
                            positionOS,
                            alongDirection
                        ) *
                        32.0
                    );

                float irregular =
                    Hash(
                        floor(
                            alongPixel /
                            2.0
                        )
                    );

                float surge =
                    waveAmount *
                    _CrashDistance;

                float localSurge =
                    surge +
                    (irregular - 0.5) *
                    1.15;

                float reached =
                    step(
                        distanceToWater,
                        localSurge
                    );

                crest =
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

                float retreat =
                    step(
                        5.0,
                        frame
                    );

                wet =
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
            }

            half4 frag(
                Varyings i) : SV_Target
            {
                half4 original =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        i.uv
                    );

                if (WaterAmount(original) > 0.15)
                    return Apply2DLighting(
                        original,
                        i.uv,
                        i.lightingUV
                    );

                float2 pitch =
                    max(
                        _CellPitch.xy,
                        float2(
                            0.0001,
                            0.0001
                        )
                    );

                float2 local =
                    frac(
                        (i.positionOS + 0.0001) /
                        pitch
                    );

                float2 spriteSizeUV =
                    _MainTex_TexelSize.xy *
                    32.0;

                float2 spriteMin =
                    i.uv -
                    local *
                    spriteSizeUV;

                float2 spriteMax =
                    spriteMin +
                    spriteSizeUV;

                float2 horizontalDirection;
                float2 verticalDirection;

                if (i.cornerCode < 0.25)
                {
                    horizontalDirection =
                        float2(-1, 0);

                    verticalDirection =
                        float2(0, 1);
                }
                else if (i.cornerCode < 0.50)
                {
                    horizontalDirection =
                        float2(1, 0);

                    verticalDirection =
                        float2(0, 1);
                }
                else if (i.cornerCode < 0.75)
                {
                    horizontalDirection =
                        float2(-1, 0);

                    verticalDirection =
                        float2(0, -1);
                }
                else
                {
                    horizontalDirection =
                        float2(1, 0);

                    verticalDirection =
                        float2(0, -1);
                }

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

                float waveAmount =
                    GetWaveAmount(
                        frame
                    );

                float crestHorizontal;
                float wetHorizontal;

                float crestVertical;
                float wetVertical;

                EvaluateWave(
                    i.uv,
                    i.positionOS,
                    horizontalDirection,
                    spriteMin,
                    spriteMax,
                    frame,
                    waveAmount,
                    crestHorizontal,
                    wetHorizontal
                );

                EvaluateWave(
                    i.uv,
                    i.positionOS,
                    verticalDirection,
                    spriteMin,
                    spriteMax,
                    frame,
                    waveAmount,
                    crestVertical,
                    wetVertical
                );

                float crest =
                    max(
                        crestHorizontal,
                        crestVertical
                    );

                float wet =
                    max(
                        wetHorizontal,
                        wetVertical
                    );

                float snow =
                    SnowAmount(
                        original.rgb
                    );

                float effectiveWetDarkness =
                    lerp(
                        _WetDarkness,
                        _WetDarkness *
                        _SnowWetMultiplier,
                        snow
                    );

                half3 result =
                    original.rgb;

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