using UnityEngine;
using UnityEngine.Tilemaps;

namespace Timbeaux.ModularGroundKit
{

    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]


    public class BiomeBlendZone2D : MonoBehaviour
    {
        public enum BlendDirection
        {
            LeftToRight,
            RightToLeft,
            BottomToTop,
            TopToBottom
        }

        public enum BlendSourceMode
        {
            SameTilemap,
            CrossTilemap
        }

        public enum TilemapTargetMode
        {
            AllSourceTilemaps,
            SpecificTilemap
        }

        [Header("Blend Source")]

        [SerializeField]
        private BlendSourceMode blendSourceMode =
            BlendSourceMode.SameTilemap;

        [Header("Same Tilemap Mode")]

        [SerializeField]
        private TilemapTargetMode tilemapTargetMode =
            TilemapTargetMode.AllSourceTilemaps;

        [SerializeField]
        private Tilemap specificTilemap;

        [Header("Cross Tilemap Mode")]

        [Tooltip(
            "Tilemap representing the start side of the blend direction."
        )]
        [SerializeField]
        private Tilemap startTilemap;

        [Tooltip(
            "Tilemap representing the end side of the blend direction."
        )]
        [SerializeField]
        private Tilemap endTilemap;

        [Header("Blend Direction")]

        [SerializeField]
        private BlendDirection blendDirection =
            BlendDirection.LeftToRight;

        [Header("Transition Look")]

        [SerializeField, Range(0f, 1f)]
        private float irregularity = 0.75f;

        [Tooltip(
            "Size of the procedural pixel clusters. " +
            "1 = individual pixels, 2 = small chunky groups."
        )]
        [SerializeField, Range(1, 6)]
        private int noiseBlockSize = 2;

        [SerializeField]
        private int seed = 12345;

        [Header("Zone")]

        [SerializeField]
        private bool zoneEnabled = true;

        [Header("Scene View")]

        [SerializeField]
        private bool showBounds = true;

        [SerializeField]
        private bool showDirection = true;

        [SerializeField]
        private Color gizmoColor =
            new Color(
                1f,
                0.65f,
                0.1f,
                0.18f
            );

        private BoxCollider2D boxCollider;

        public BlendSourceMode SourceMode =>
            blendSourceMode;

        public TilemapTargetMode TargetMode =>
            tilemapTargetMode;

        public Tilemap SpecificTilemap =>
            specificTilemap;

        public Tilemap StartTilemap =>
            startTilemap;

        public Tilemap EndTilemap =>
            endTilemap;

        public BlendDirection Direction =>
            blendDirection;

        public float Irregularity =>
            irregularity;

        public int NoiseBlockSize =>
            Mathf.Max(
                1,
                noiseBlockSize
            );

        public int Seed =>
            seed;

        public bool ZoneEnabled =>
            zoneEnabled;

        public bool ShowBounds =>
            showBounds;

        public bool ShowDirection =>
            showDirection;

        public Color GizmoColor =>
            gizmoColor;

        public BoxCollider2D BoundsCollider
        {
            get
            {
                CacheCollider();
                return boxCollider;
            }
        }

        private void Reset()
        {
            CacheCollider();

            if (boxCollider != null)
            {
                boxCollider.isTrigger = true;

                boxCollider.size =
                    new Vector2(
                        2f,
                        4f
                    );
            }

            blendSourceMode =
                BlendSourceMode.SameTilemap;

            tilemapTargetMode =
                TilemapTargetMode.AllSourceTilemaps;

            specificTilemap = null;

            startTilemap = null;
            endTilemap = null;

            blendDirection =
                BlendDirection.LeftToRight;

            irregularity = 0.75f;

            noiseBlockSize = 2;

            seed = 12345;

            zoneEnabled = true;

            showBounds = true;

            showDirection = true;
        }

        private void OnEnable()
        {
            CacheCollider();

            if (boxCollider != null)
            {
                boxCollider.isTrigger = true;

                if (Application.isPlaying)
                {
                    boxCollider.enabled = false;
                }
            }
        }

        private void OnValidate()
        {
            CacheCollider();

            if (boxCollider != null)
            {
                boxCollider.isTrigger = true;
            }

            noiseBlockSize =
                Mathf.Max(
                    1,
                    noiseBlockSize
                );
        }

        private void CacheCollider()
        {
            if (boxCollider == null)
            {
                boxCollider =
                    GetComponent<BoxCollider2D>();
            }
        }

        public bool ContainsWorldPoint(
            Vector3 worldPoint)
        {
            CacheCollider();

            if (boxCollider == null)
                return false;

            Vector3 localPoint =
                transform.InverseTransformPoint(
                    worldPoint
                );

            Vector2 center =
                boxCollider.offset;

            Vector2 halfSize =
                boxCollider.size * 0.5f;

            return
                localPoint.x >=
                    center.x - halfSize.x &&
                localPoint.x <=
                    center.x + halfSize.x &&
                localPoint.y >=
                    center.y - halfSize.y &&
                localPoint.y <=
                    center.y + halfSize.y;
        }

        public float GetBlend01(
            Vector3 worldPoint)
        {
            CacheCollider();

            if (boxCollider == null)
                return 0f;

            Vector3 localPoint =
                transform.InverseTransformPoint(
                    worldPoint
                );

            Vector2 center =
                boxCollider.offset;

            Vector2 halfSize =
                boxCollider.size * 0.5f;

            float result;

            switch (blendDirection)
            {
                case BlendDirection.RightToLeft:

                    result =
                        Mathf.InverseLerp(
                            center.x + halfSize.x,
                            center.x - halfSize.x,
                            localPoint.x
                        );

                    break;

                case BlendDirection.BottomToTop:

                    result =
                        Mathf.InverseLerp(
                            center.y - halfSize.y,
                            center.y + halfSize.y,
                            localPoint.y
                        );

                    break;

                case BlendDirection.TopToBottom:

                    result =
                        Mathf.InverseLerp(
                            center.y + halfSize.y,
                            center.y - halfSize.y,
                            localPoint.y
                        );

                    break;

                default:

                    result =
                        Mathf.InverseLerp(
                            center.x - halfSize.x,
                            center.x + halfSize.x,
                            localPoint.x
                        );

                    break;
            }

            return Mathf.Clamp01(
                result
            );
        }

        public Vector3 GetOutsideSamplePoint(
            Vector3 referenceWorldPoint,
            bool startSide,
            float padding)
        {
            CacheCollider();

            if (boxCollider == null)
                return referenceWorldPoint;

            Vector3 localPoint =
                transform.InverseTransformPoint(
                    referenceWorldPoint
                );

            Vector2 center =
                boxCollider.offset;

            Vector2 halfSize =
                boxCollider.size * 0.5f;

            switch (blendDirection)
            {
                case BlendDirection.LeftToRight:

                    localPoint.x =
                        startSide
                            ? center.x -
                              halfSize.x -
                              padding
                            : center.x +
                              halfSize.x +
                              padding;

                    break;

                case BlendDirection.RightToLeft:

                    localPoint.x =
                        startSide
                            ? center.x +
                              halfSize.x +
                              padding
                            : center.x -
                              halfSize.x -
                              padding;

                    break;

                case BlendDirection.BottomToTop:

                    localPoint.y =
                        startSide
                            ? center.y -
                              halfSize.y -
                              padding
                            : center.y +
                              halfSize.y +
                              padding;

                    break;

                case BlendDirection.TopToBottom:

                    localPoint.y =
                        startSide
                            ? center.y +
                              halfSize.y +
                              padding
                            : center.y -
                              halfSize.y -
                              padding;

                    break;
            }

            return transform.TransformPoint(
                localPoint
            );
        }

        public Bounds GetWorldBounds()
        {
            CacheCollider();

            if (boxCollider == null)
            {
                return new Bounds(
                    transform.position,
                    Vector3.zero
                );
            }

            Vector2 center =
                boxCollider.offset;

            Vector2 halfSize =
                boxCollider.size * 0.5f;

            Vector3[] corners =
            {
            transform.TransformPoint(
                new Vector3(
                    center.x - halfSize.x,
                    center.y - halfSize.y,
                    0f
                )
            ),

            transform.TransformPoint(
                new Vector3(
                    center.x + halfSize.x,
                    center.y - halfSize.y,
                    0f
                )
            ),

            transform.TransformPoint(
                new Vector3(
                    center.x - halfSize.x,
                    center.y + halfSize.y,
                    0f
                )
            ),

            transform.TransformPoint(
                new Vector3(
                    center.x + halfSize.x,
                    center.y + halfSize.y,
                    0f
                )
            )
        };

            Bounds bounds =
                new Bounds(
                    corners[0],
                    Vector3.zero
                );

            for (int i = 1;
                 i < corners.Length;
                 i++)
            {
                bounds.Encapsulate(
                    corners[i]
                );
            }

            return bounds;
        }

        private void OnDrawGizmos()
        {
            if (!showBounds)
                return;

            CacheCollider();

            if (boxCollider == null)
                return;

            DrawZoneGizmo(
                false
            );
        }

        private void OnDrawGizmosSelected()
        {
            if (!showBounds)
                return;

            CacheCollider();

            if (boxCollider == null)
                return;

            DrawZoneGizmo(
                true
            );
        }

        private void DrawZoneGizmo(
            bool selected)
        {
            Gizmos.matrix =
                transform.localToWorldMatrix;

            Vector3 center =
                new Vector3(
                    boxCollider.offset.x,
                    boxCollider.offset.y,
                    0f
                );

            Vector3 size =
                new Vector3(
                    boxCollider.size.x,
                    boxCollider.size.y,
                    0.01f
                );

            Color fillColor =
                gizmoColor;

            Color outlineColor =
                gizmoColor;

            if (selected)
            {
                fillColor.a =
                    Mathf.Max(
                        fillColor.a,
                        0.28f
                    );

                outlineColor.a = 1f;
            }
            else
            {
                outlineColor.a = 0.65f;
            }

            Gizmos.color =
                fillColor;

            Gizmos.DrawCube(
                center,
                size
            );

            Gizmos.color =
                outlineColor;

            Gizmos.DrawWireCube(
                center,
                size
            );

            if (showDirection)
            {
                DrawDirectionArrow(
                    center,
                    size
                );
            }

            Gizmos.matrix =
                Matrix4x4.identity;
        }

        private void DrawDirectionArrow(
            Vector3 center,
            Vector3 size)
        {
            Vector3 direction;

            switch (blendDirection)
            {
                case BlendDirection.RightToLeft:

                    direction =
                        Vector3.left;

                    break;

                case BlendDirection.BottomToTop:

                    direction =
                        Vector3.up;

                    break;

                case BlendDirection.TopToBottom:

                    direction =
                        Vector3.down;

                    break;

                default:

                    direction =
                        Vector3.right;

                    break;
            }

            float arrowLength;

            if (Mathf.Abs(direction.x) > 0f)
            {
                arrowLength =
                    Mathf.Max(
                        0.4f,
                        size.x * 0.35f
                    );
            }
            else
            {
                arrowLength =
                    Mathf.Max(
                        0.4f,
                        size.y * 0.35f
                    );
            }

            Vector3 start =
                center -
                direction *
                arrowLength *
                0.5f;

            Vector3 end =
                center +
                direction *
                arrowLength *
                0.5f;

            Gizmos.DrawLine(
                start,
                end
            );

            Vector3 perpendicular =
                new Vector3(
                    -direction.y,
                    direction.x,
                    0f
                );

            float headSize =
                Mathf.Min(
                    arrowLength * 0.25f,
                    0.35f
                );

            Gizmos.DrawLine(
                end,
                end -
                direction *
                headSize +
                perpendicular *
                headSize *
                0.6f
            );

            Gizmos.DrawLine(
                end,
                end -
                direction *
                headSize -
                perpendicular *
                headSize *
                0.6f
            );
        }
    }
}