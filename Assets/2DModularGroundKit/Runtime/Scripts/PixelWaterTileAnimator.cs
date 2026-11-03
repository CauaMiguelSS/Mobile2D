using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace Timbeaux.ModularGroundKit
{
    [RequireComponent(typeof(Tilemap))]
    public class PixelWaterTileAnimator : MonoBehaviour
    {
        [Header("Water Sprites")]
        [Tooltip("Add only full-water sprites. Do not add shoreline sprites.")]
        [SerializeField] private List<Sprite> waterSprites = new List<Sprite>();

        [Header("Animation")]
        [SerializeField, Min(0.05f)]
        private float frameTime = 0.25f;

        private Tilemap tilemap;

        private readonly List<Vector3Int> waterCells =
            new List<Vector3Int>();

        private float timer;
        private int frame;

        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();

            FindWaterCells();
            ApplyFrame();
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer < frameTime)
                return;

            timer -= frameTime;

            frame = (frame + 1) % 4;

            ApplyFrame();
        }

        private void FindWaterCells()
        {
            waterCells.Clear();

            foreach (Vector3Int position in
                     tilemap.cellBounds.allPositionsWithin)
            {
                Sprite sprite = tilemap.GetSprite(position);

                if (sprite == null)
                    continue;

                if (!waterSprites.Contains(sprite))
                    continue;

                tilemap.SetTileFlags(
                    position,
                    TileFlags.None
                );

                waterCells.Add(position);
            }

            Debug.Log(
                "Pixel Water Animator found " +
                waterCells.Count +
                " water cells."
            );
        }

        private void ApplyFrame()
        {
            Matrix4x4 waterTransform =
                GetWaterTransform(frame);

            foreach (Vector3Int position in waterCells)
            {
                tilemap.SetTransformMatrix(
                    position,
                    waterTransform
                );
            }
        }

        private Matrix4x4 GetWaterTransform(int state)
        {
            switch (state)
            {
                case 1:
                    return Matrix4x4.Scale(
                        new Vector3(-1f, 1f, 1f)
                    );

                case 2:
                    return Matrix4x4.Rotate(
                        Quaternion.Euler(0f, 0f, 180f)
                    );

                case 3:
                    return Matrix4x4.Scale(
                        new Vector3(1f, -1f, 1f)
                    );

                default:
                    return Matrix4x4.identity;
            }
        }
    }
}