using UnityEngine;
using UnityEngine.Tilemaps;
namespace Timbeaux.ModularGroundKit
{
    [RequireComponent(typeof(Tilemap))]
    [RequireComponent(typeof(TilemapRenderer))]
    public class ShorelineCornerWaveController2D : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Tilemap waterTilemap;

        [Header("Corner Detection")]
        [SerializeField, Range(1, 5)]
        private int detectionRadius = 3;

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

        private Tilemap shorelineTilemap;
        private TilemapRenderer tilemapRenderer;
        private Material materialInstance;
        private Grid parentGrid;

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

        private static readonly int CellPitchID =
            Shader.PropertyToID("_CellPitch");

        private void Awake()
        {
            shorelineTilemap = GetComponent<Tilemap>();
            tilemapRenderer = GetComponent<TilemapRenderer>();
            parentGrid = GetComponentInParent<Grid>();

            FindWaterTilemap();
            DetectCornerDirections();

            materialInstance = tilemapRenderer.material;

            ApplySettings();
        }

        private void Update()
        {
            if (materialInstance == null)
                return;

            ApplySettings();

            materialInstance.SetFloat(
                TimeID,
                Time.time
            );
        }

        private void FindWaterTilemap()
        {
            if (waterTilemap != null)
                return;

            if (transform.parent == null)
                return;

            Transform waterObject =
                transform.parent.Find("Water");

            if (waterObject != null)
            {
                waterTilemap =
                    waterObject.GetComponent<Tilemap>();
            }
        }

        [ContextMenu("Detect Corner Directions")]
        public void DetectCornerDirections()
        {
            if (shorelineTilemap == null)
                shorelineTilemap = GetComponent<Tilemap>();

            FindWaterTilemap();

            if (waterTilemap == null)
            {
                Debug.LogWarning(
                    "Shoreline Corner Wave: Water Tilemap not found."
                );

                return;
            }

            foreach (
                Vector3Int cell in
                shorelineTilemap.cellBounds.allPositionsWithin)
            {
                if (!shorelineTilemap.HasTile(cell))
                    continue;

                float upLeft =
                    ScoreQuadrant(cell, -1, 1);

                float upRight =
                    ScoreQuadrant(cell, 1, 1);

                float downLeft =
                    ScoreQuadrant(cell, -1, -1);

                float downRight =
                    ScoreQuadrant(cell, 1, -1);

                float bestScore = upLeft;
                float cornerCode = 0.125f;

                if (upRight > bestScore)
                {
                    bestScore = upRight;
                    cornerCode = 0.375f;
                }

                if (downLeft > bestScore)
                {
                    bestScore = downLeft;
                    cornerCode = 0.625f;
                }

                if (downRight > bestScore)
                {
                    cornerCode = 0.875f;
                }

                shorelineTilemap.SetTileFlags(
                    cell,
                    TileFlags.None
                );

                shorelineTilemap.SetColor(
                    cell,
                    new Color(
                        cornerCode,
                        1f,
                        1f,
                        1f
                    )
                );
            }

            shorelineTilemap.RefreshAllTiles();
        }

        private float ScoreQuadrant(
            Vector3Int origin,
            int xDirection,
            int yDirection)
        {
            float score = 0f;

            for (int x = 0; x <= detectionRadius; x++)
            {
                for (int y = 0; y <= detectionRadius; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    Vector3Int check =
                        origin +
                        new Vector3Int(
                            x * xDirection,
                            y * yDirection,
                            0
                        );

                    if (!waterTilemap.HasTile(check))
                        continue;

                    float distance =
                        Mathf.Max(1f, x + y);

                    score += 1f / distance;
                }
            }

            return score;
        }

        private void ApplySettings()
        {
            if (materialInstance == null)
                return;

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

            Vector2 pitch =
                Vector2.one;

            if (parentGrid != null)
            {
                pitch.x =
                    parentGrid.cellSize.x +
                    parentGrid.cellGap.x;

                pitch.y =
                    parentGrid.cellSize.y +
                    parentGrid.cellGap.y;
            }

            materialInstance.SetVector(
                CellPitchID,
                new Vector4(
                    pitch.x,
                    pitch.y,
                    0f,
                    0f
                )
            );
        }
    }
}