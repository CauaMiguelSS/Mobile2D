using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace Timbeaux.ModularGroundKit.Editor
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(TrailRenderer))]
    public class CozyFirefly2D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float wanderRadius = 20f;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float bobAmount = 0.1f;
        [SerializeField] private float bobSpeed = 2f;

        [Header("Glow")]
        [SerializeField] private float glowSpeed = 2f;
        [SerializeField] private float minAlpha = 0.35f;
        [SerializeField] private float maxAlpha = 1f;

        [SerializeField, Range(0f, 1f)]
        private float minBrightness = 0.45f;

        [SerializeField] private Light2D glowLight;

        [SerializeField] private float minLightIntensity = 0.09f;
        [SerializeField] private float maxLightIntensity = 0.74f;

        [Header("Light Shape")]
        [SerializeField] private float lightInnerRadius = 0.6f;
        [SerializeField] private float lightOuterRadius = 4.5f;

        [SerializeField, Range(0f, 1f)]
        private float lightFalloffStrength = 0.75f;

        [Header("Light Edge Flicker")]
        [SerializeField] private bool flickerLightEdge = true;

        [SerializeField, Range(0.1f, 15f)]
        private float lightFlickerSpeed = 4f;

        [SerializeField, Range(0f, 1f)]
        private float lightRadiusFlicker = 0.12f;

        [SerializeField, Range(0f, 0.5f)]
        private float lightIntensityFlicker = 0.08f;

        [Header("Trail")]
        [SerializeField] private bool useTrail = true;
        [SerializeField] private float trailTime = 5f;
        [SerializeField] private float trailStartWidth = 2f;
        [SerializeField] private float trailEndWidth = 0.01f;

        [SerializeField]
        private Color trailColor =
            new Color(1f, 0.9f, 0.15f, 0.8f);

        [Header("Trail Flicker")]
        [SerializeField] private bool flickerTrailEnd = true;

        [SerializeField, Range(1f, 20f)]
        private float trailFlickerSpeed = 8f;

        [SerializeField, Range(0f, 1f)]
        private float trailFlickerStrength = 0.65f;

        private SpriteRenderer spriteRenderer;
        private TrailRenderer trailRenderer;

        private Vector3 homePosition;
        private Vector3 currentPosition;
        private Vector3 targetPosition;

        private Color baseSpriteColor;

        private float glowOffset;
        private float flickerOffset;
        private float lightNoiseOffset;

        private Material trailMaterial;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            trailRenderer = GetComponent<TrailRenderer>();

            baseSpriteColor = spriteRenderer.color;

            homePosition = transform.position;
            currentPosition = homePosition;

            glowOffset = Random.Range(0f, 10f);
            flickerOffset = Random.Range(0f, 10f);
            lightNoiseOffset = Random.Range(0f, 100f);

            SetupTrail();
            SetupLight();

            PickNewTarget();
        }

        private void Start()
        {
            trailRenderer.Clear();
            trailRenderer.emitting = useTrail;
        }

        private void Update()
        {
            MoveFirefly();

            float pulse = CalculatePulse();

            UpdateFireflyGlow(pulse);
            UpdateLight(pulse);
            UpdateTrail(pulse);
        }

        private float CalculatePulse()
        {
            return (
                Mathf.Sin(
                    Time.time * glowSpeed +
                    glowOffset
                ) + 1f
            ) * 0.5f;
        }

        private void MoveFirefly()
        {
            currentPosition =
                Vector3.MoveTowards(
                    currentPosition,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

            if (Vector3.Distance(
                    currentPosition,
                    targetPosition
                ) < 0.03f)
            {
                PickNewTarget();
            }

            float bob =
                Mathf.Sin(
                    Time.time * bobSpeed +
                    glowOffset
                ) * bobAmount;

            transform.position =
                currentPosition +
                new Vector3(
                    0f,
                    bob,
                    0f
                );
        }

        private void PickNewTarget()
        {
            Vector2 offset =
                Random.insideUnitCircle *
                wanderRadius;

            targetPosition =
                homePosition +
                new Vector3(
                    offset.x,
                    offset.y,
                    0f
                );
        }

        private void UpdateFireflyGlow(float pulse)
        {
            float brightness =
                Mathf.Lerp(
                    minBrightness,
                    1f,
                    pulse
                );

            float alpha =
                Mathf.Lerp(
                    minAlpha,
                    maxAlpha,
                    pulse
                );

            spriteRenderer.color =
                new Color(
                    baseSpriteColor.r * brightness,
                    baseSpriteColor.g * brightness,
                    baseSpriteColor.b * brightness,
                    alpha
                );
        }

        private void SetupLight()
        {
            if (glowLight == null)
                return;

            glowLight.pointLightInnerRadius =
                lightInnerRadius;

            glowLight.pointLightOuterRadius =
                lightOuterRadius;

            glowLight.falloffIntensity =
                lightFalloffStrength;
        }

        private void UpdateLight(float pulse)
        {
            if (glowLight == null)
                return;

            float flicker = 0.5f;

            if (flickerLightEdge)
            {
                flicker =
                    Mathf.PerlinNoise(
                        lightNoiseOffset,
                        Time.time * lightFlickerSpeed
                    );
            }

            // Main brightness follows the firefly pulse.
            float intensity =
                Mathf.Lerp(
                    minLightIntensity,
                    maxLightIntensity,
                    pulse
                );

            // Very small irregular light shimmer.
            float intensityVariation =
                Mathf.Lerp(
                    1f - lightIntensityFlicker,
                    1f + lightIntensityFlicker,
                    flicker
                );

            glowLight.intensity =
                intensity *
                intensityVariation;

            // The INNER radius stays stable.
            // The OUTER edge gently breathes/flickers.
            glowLight.pointLightInnerRadius =
                lightInnerRadius;

            float radiusVariation =
                Mathf.Lerp(
                    1f - lightRadiusFlicker,
                    1f + lightRadiusFlicker,
                    flicker
                );

            glowLight.pointLightOuterRadius =
                lightOuterRadius *
                radiusVariation;

            glowLight.falloffIntensity =
                lightFalloffStrength;
        }

        private void SetupTrail()
        {
            Shader shader =
                Shader.Find("Sprites/Default");

            if (shader != null)
            {
                trailMaterial =
                    new Material(shader);

                trailMaterial.name =
                    "FireflyTrail_Runtime";

                trailRenderer.material =
                    trailMaterial;
            }

            trailRenderer.Clear();
        }

        private void UpdateTrail(float pulse)
        {
            if (trailRenderer == null)
                return;

            trailRenderer.enabled = useTrail;
            trailRenderer.emitting = useTrail;

            trailRenderer.time = trailTime;
            trailRenderer.startWidth = trailStartWidth;
            trailRenderer.endWidth = trailEndWidth;

            trailRenderer.minVertexDistance =
                0.05f;

            trailRenderer.alignment =
                LineAlignment.View;

            trailRenderer.textureMode =
                LineTextureMode.Stretch;

            trailRenderer.sortingLayerID =
                spriteRenderer.sortingLayerID;

            trailRenderer.sortingOrder =
                spriteRenderer.sortingOrder - 1;

            float brightness =
                Mathf.Lerp(
                    minBrightness,
                    1f,
                    pulse
                );

            float alpha =
                Mathf.Lerp(
                    minAlpha,
                    maxAlpha,
                    pulse
                );

            Color liveTrailColor =
                new Color(
                    trailColor.r * brightness,
                    trailColor.g * brightness,
                    trailColor.b * brightness,
                    trailColor.a
                );

            float mainAlpha =
                trailColor.a * alpha;

            float endFlicker = 1f;

            if (flickerTrailEnd)
            {
                float flickerA =
                    (
                        Mathf.Sin(
                            Time.time *
                            trailFlickerSpeed +
                            flickerOffset
                        ) + 1f
                    ) * 0.5f;

                float flickerB =
                    (
                        Mathf.Sin(
                            Time.time *
                            trailFlickerSpeed *
                            1.73f +
                            flickerOffset *
                            2.13f
                        ) + 1f
                    ) * 0.5f;

                endFlicker =
                    Mathf.Lerp(
                        1f,
                        flickerA * flickerB,
                        trailFlickerStrength
                    );
            }

            Gradient gradient =
                new Gradient();

            gradient.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(
                    liveTrailColor,
                    0f
                ),

                new GradientColorKey(
                    liveTrailColor,
                    0.70f
                ),

                new GradientColorKey(
                    liveTrailColor,
                    1f
                )
                },

                new GradientAlphaKey[]
                {
                new GradientAlphaKey(
                    mainAlpha,
                    0f
                ),

                new GradientAlphaKey(
                    mainAlpha * 0.7f,
                    0.55f
                ),

                new GradientAlphaKey(
                    mainAlpha *
                    0.45f *
                    endFlicker,
                    0.82f
                ),

                new GradientAlphaKey(
                    0f,
                    1f
                )
                }
            );

            trailRenderer.colorGradient =
                gradient;
        }

        [ContextMenu("Apply Cozy Defaults")]
        private void ApplyCozyDefaults()
        {
            wanderRadius = 20f;
            moveSpeed = 1f;
            bobAmount = 0.1f;
            bobSpeed = 2f;

            glowSpeed = 2f;
            minAlpha = 0.35f;
            maxAlpha = 1f;
            minBrightness = 0.45f;

            minLightIntensity = 0.09f;
            maxLightIntensity = 0.74f;

            lightInnerRadius = 0.6f;
            lightOuterRadius = 4.5f;
            lightFalloffStrength = 0.75f;

            flickerLightEdge = true;
            lightFlickerSpeed = 4f;
            lightRadiusFlicker = 0.12f;
            lightIntensityFlicker = 0.08f;

            useTrail = true;
            trailTime = 5f;
            trailStartWidth = 2f;
            trailEndWidth = 0.01f;

            trailColor =
                new Color(
                    1f,
                    0.9f,
                    0.15f,
                    0.8f
                );

            flickerTrailEnd = true;
            trailFlickerSpeed = 8f;
            trailFlickerStrength = 0.65f;

            SetupLight();
        }

        private void Reset()
        {
            ApplyCozyDefaults();
        }

        private void OnDestroy()
        {
            if (trailMaterial != null)
            {
                Destroy(trailMaterial);
            }
        }
    }
}