using UnityEngine;
using UnityEngine.Tilemaps;
namespace Timbeaux.ModularGroundKit
{
    [RequireComponent(typeof(TilemapRenderer))]
    public class ShorelineWaveController2D : MonoBehaviour
    {
        public enum WaterSide
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
            AutoCorner = 4
        }

        [Header("Setup")]
        [SerializeField] private WaterSide waterSide = WaterSide.Down;

        public WaterSide CurrentWaterSide
        {
            get { return waterSide; }
        }

        [Header("Wave")]
        [SerializeField, Range(0.1f, 3f)]
        private float waveSpeed = 0.7f;

        [SerializeField, Range(1f, 6f)]
        private float crashDistance = 4f;

        [SerializeField, Range(0f, 1f)]
        private float foamStrength = 0.8f;

        [SerializeField, Range(0f, 0.5f)]
        private float wetDarkness = 0.22f;

        [SerializeField]
        private float phaseOffset = 0f;

        private TilemapRenderer tilemapRenderer;
        private Material materialInstance;

        private static readonly int WaterSideID =
            Shader.PropertyToID("_WaterSide");

        private static readonly int SpeedID =
            Shader.PropertyToID("_WaveSpeed");

        private static readonly int CrashDistanceID =
            Shader.PropertyToID("_CrashDistance");

        private static readonly int FoamID =
            Shader.PropertyToID("_FoamStrength");

        private static readonly int WetID =
            Shader.PropertyToID("_WetDarkness");

        private static readonly int PhaseID =
            Shader.PropertyToID("_Phase");

        private static readonly int TimeID =
            Shader.PropertyToID("_WaveTime");

        private void Awake()
        {
            tilemapRenderer = GetComponent<TilemapRenderer>();
            materialInstance = tilemapRenderer.material;

            ApplySettings();
        }

        private void Update()
        {
            if (materialInstance == null)
                return;

            ApplySettings();
            materialInstance.SetFloat(TimeID, Time.time);
        }

        private void ApplySettings()
        {
            if (materialInstance == null)
                return;

            materialInstance.SetFloat(
                WaterSideID,
                (float)waterSide
            );

            materialInstance.SetFloat(
                SpeedID,
                waveSpeed
            );

            materialInstance.SetFloat(
                CrashDistanceID,
                crashDistance
            );

            materialInstance.SetFloat(
                FoamID,
                foamStrength
            );

            materialInstance.SetFloat(
                WetID,
                wetDarkness
            );

            materialInstance.SetFloat(
                PhaseID,
                phaseOffset
            );
        }
    }
}