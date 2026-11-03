using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace Timbeaux.ModularGroundKit
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class BiomeBlendController2D : MonoBehaviour
    {
        [Header("Source Tilemaps")]

        [Tooltip(
            "Tilemaps available to Same Tilemap blend zones."
        )]
        [SerializeField]
        private List<Tilemap> sourceTilemaps =
            new List<Tilemap>();

        [SerializeField, HideInInspector]
        private Tilemap sourceTilemap;

        [Header("Generated Overlay")]

        [Tooltip(
            "Generated blend sprites render this many sorting-order " +
            "steps above the source Tilemap."
        )]
        [SerializeField]
        private int sortingOrderOffset = 1;

        [Tooltip(
            "Automatically build the biome blends when the scene starts."
        )]
        [SerializeField]
        private bool rebuildOnPlay = true;

        [Header("Debug")]

        [SerializeField]
        private bool logGenerationSummary = true;

        private const string GeneratedRootName =
            "__GeneratedBiomeBlend";

        private readonly Dictionary<Sprite, SpritePixelData>
            spriteCache =
                new Dictionary<Sprite, SpritePixelData>();

        private class SpritePixelData
        {
            public Color32[] pixels;
            public int width;
            public int height;
            public float pixelsPerUnit;
        }

        private void OnEnable()
        {
            MigrateLegacySourceTilemap();
        }

        private void OnValidate()
        {
            MigrateLegacySourceTilemap();
        }

        private void Start()
        {
            MigrateLegacySourceTilemap();

            if (Application.isPlaying &&
                rebuildOnPlay)
            {
                RebuildTransitions();
            }
        }

        private void MigrateLegacySourceTilemap()
        {
            if (sourceTilemap == null)
                return;

            if (sourceTilemaps == null)
            {
                sourceTilemaps =
                    new List<Tilemap>();
            }

            if (!sourceTilemaps.Contains(
                    sourceTilemap))
            {
                sourceTilemaps.Add(
                    sourceTilemap
                );
            }

            sourceTilemap = null;
        }

        [ContextMenu("Rebuild Biome Transitions")]
        public void RebuildTransitions()
        {
            ClearTransitions();

            MigrateLegacySourceTilemap();

            BiomeBlendZone2D[] zones =
                GetComponentsInChildren<
                    BiomeBlendZone2D
                >(true);

            if (zones.Length == 0)
            {
                Debug.LogWarning(
                    "BiomeBlendController2D: " +
                    "No BiomeBlendZone2D children were found.",
                    this
                );

                return;
            }

            GameObject generatedRoot =
                new GameObject(
                    GeneratedRootName
                );

            generatedRoot.transform.SetParent(
                transform,
                false
            );

            generatedRoot.transform.localPosition =
                Vector3.zero;

            int generatedCount = 0;
            int processedZoneCount = 0;

            spriteCache.Clear();

            for (int i = 0;
                 i < zones.Length;
                 i++)
            {
                BiomeBlendZone2D zone =
                    zones[i];

                if (zone == null ||
                    !zone.ZoneEnabled ||
                    !zone.gameObject.activeInHierarchy)
                {
                    continue;
                }

                GameObject zoneRoot =
                    new GameObject(
                        "Zone_" +
                        zone.gameObject.name
                    );

                zoneRoot.transform.SetParent(
                    generatedRoot.transform,
                    false
                );

                int zoneGeneratedCount;

                if (zone.SourceMode ==
                    BiomeBlendZone2D.BlendSourceMode.CrossTilemap)
                {
                    zoneGeneratedCount =
                        BuildCrossTilemapZone(
                            zone,
                            zoneRoot.transform
                        );
                }
                else
                {
                    zoneGeneratedCount =
                        BuildSameTilemapZone(
                            zone,
                            zoneRoot.transform
                        );
                }

                if (zoneGeneratedCount == 0)
                {
                    DestroyUnityObject(
                        zoneRoot
                    );
                }
                else
                {
                    generatedCount +=
                        zoneGeneratedCount;

                    processedZoneCount++;
                }
            }

            spriteCache.Clear();

            if (generatedCount == 0)
            {
                DestroyUnityObject(
                    generatedRoot
                );
            }

            if (logGenerationSummary)
            {
                Debug.Log(
                    "BiomeBlendController2D generated " +
                    generatedCount +
                    " blended cell overlay(s) across " +
                    processedZoneCount +
                    " zone(s).",
                    this
                );
            }
        }

        [ContextMenu("Clear Biome Transitions")]
        public void ClearTransitions()
        {
            Transform existingRoot =
                transform.Find(
                    GeneratedRootName
                );

            if (existingRoot == null)
                return;

            SpriteRenderer[] renderers =
                existingRoot.GetComponentsInChildren<
                    SpriteRenderer
                >(true);

            for (int i = 0;
                 i < renderers.Length;
                 i++)
            {
                SpriteRenderer renderer =
                    renderers[i];

                if (renderer == null ||
                    renderer.sprite == null)
                {
                    continue;
                }

                Sprite generatedSprite =
                    renderer.sprite;

                Texture2D generatedTexture =
                    generatedSprite.texture;

                DestroyUnityObject(
                    generatedSprite
                );

                if (generatedTexture != null)
                {
                    DestroyUnityObject(
                        generatedTexture
                    );
                }
            }

            DestroyUnityObject(
                existingRoot.gameObject
            );
        }

        private int BuildSameTilemapZone(
            BiomeBlendZone2D zone,
            Transform zoneRoot)
        {
            int total = 0;

            if (zone.TargetMode ==
                BiomeBlendZone2D.TilemapTargetMode.SpecificTilemap)
            {
                if (zone.SpecificTilemap != null)
                {
                    total +=
                        BuildZoneOnSingleTilemap(
                            zone,
                            zone.SpecificTilemap,
                            zoneRoot
                        );
                }

                return total;
            }

            if (sourceTilemaps == null)
                return 0;

            HashSet<Tilemap> processed =
                new HashSet<Tilemap>();

            for (int i = 0;
                 i < sourceTilemaps.Count;
                 i++)
            {
                Tilemap tilemap =
                    sourceTilemaps[i];

                if (tilemap == null)
                    continue;

                if (!processed.Add(
                        tilemap))
                {
                    continue;
                }

                total +=
                    BuildZoneOnSingleTilemap(
                        zone,
                        tilemap,
                        zoneRoot
                    );
            }

            return total;
        }

        private int BuildZoneOnSingleTilemap(
            BiomeBlendZone2D zone,
            Tilemap targetTilemap,
            Transform zoneRoot)
        {
            TilemapRenderer sourceRenderer =
                targetTilemap.GetComponent<
                    TilemapRenderer
                >();

            if (sourceRenderer == null)
                return 0;

            GameObject tilemapRoot =
                new GameObject(
                    "Tilemap_" +
                    targetTilemap.gameObject.name
                );

            tilemapRoot.transform.SetParent(
                zoneRoot,
                false
            );

            int result =
                BuildSameTilemapCells(
                    zone,
                    targetTilemap,
                    sourceRenderer,
                    tilemapRoot.transform
                );

            if (result == 0)
            {
                DestroyUnityObject(
                    tilemapRoot
                );
            }

            return result;
        }

        private int BuildSameTilemapCells(
            BiomeBlendZone2D zone,
            Tilemap targetTilemap,
            TilemapRenderer sourceRenderer,
            Transform generatedRoot)
        {
            Bounds worldBounds =
                zone.GetWorldBounds();

            Vector3Int minimumCell =
                targetTilemap.WorldToCell(
                    worldBounds.min
                );

            Vector3Int maximumCell =
                targetTilemap.WorldToCell(
                    worldBounds.max
                );

            int generatedCount = 0;

            for (int x = minimumCell.x;
                 x <= maximumCell.x;
                 x++)
            {
                for (int y = minimumCell.y;
                     y <= maximumCell.y;
                     y++)
                {
                    Vector3Int cell =
                        new Vector3Int(
                            x,
                            y,
                            0
                        );

                    Sprite baseSprite =
                        targetTilemap.GetSprite(
                            cell
                        );

                    if (baseSprite == null)
                        continue;

                    Vector3 cellCenter =
                        targetTilemap
                            .GetCellCenterWorld(
                                cell
                            );

                    SpritePixelData baseData =
                        GetSpritePixelData(
                            baseSprite
                        );

                    if (baseData == null)
                        continue;

                    Sprite generatedSprite =
                        CreateSameTilemapBlendSprite(
                            zone,
                            targetTilemap,
                            cell,
                            cellCenter,
                            baseSprite,
                            baseData
                        );

                    if (generatedSprite == null)
                        continue;

                    CreateGeneratedRenderer(
                        generatedRoot,
                        generatedSprite,
                        cellCenter,
                        sourceRenderer,
                        sourceRenderer.sharedMaterial,
                        MultiplyColors(
                            targetTilemap.color,
                            targetTilemap.GetColor(
                                cell
                            )
                        ),
                        "Blend_" +
                        cell.x +
                        "_" +
                        cell.y
                    );

                    generatedCount++;
                }
            }

            return generatedCount;
        }

        private Sprite CreateSameTilemapBlendSprite(
            BiomeBlendZone2D zone,
            Tilemap targetTilemap,
            Vector3Int cell,
            Vector3 cellCenter,
            Sprite baseSprite,
            SpritePixelData baseData)
        {
            float cellBlend =
                zone.GetBlend01(
                    cellCenter
                );

            float sourcePadding =
                GetApproximateCellSize(
                    targetTilemap
                ) *
                0.55f;

            Vector3 startSampleWorld =
                zone.GetOutsideSamplePoint(
                    cellCenter,
                    true,
                    sourcePadding
                );

            Vector3 endSampleWorld =
                zone.GetOutsideSamplePoint(
                    cellCenter,
                    false,
                    sourcePadding
                );

            Vector3Int startCell =
                targetTilemap.WorldToCell(
                    startSampleWorld
                );

            Vector3Int endCell =
                targetTilemap.WorldToCell(
                    endSampleWorld
                );

            Sprite startSprite =
                targetTilemap.GetSprite(
                    startCell
                );

            Sprite endSprite =
                targetTilemap.GetSprite(
                    endCell
                );

            bool baseIsStartSide =
                cellBlend < 0.5f;

            if (baseIsStartSide)
            {
                startSprite =
                    baseSprite;
            }
            else
            {
                endSprite =
                    baseSprite;
            }

            if (startSprite == null ||
                endSprite == null)
            {
                return null;
            }

            SpritePixelData startData =
                GetSpritePixelData(
                    startSprite
                );

            SpritePixelData endData =
                GetSpritePixelData(
                    endSprite
                );

            if (startData == null ||
                endData == null)
            {
                return null;
            }

            return CreateTransparentBlendTexture(
                zone,
                cell,
                cellCenter,
                baseSprite,
                baseData,
                startData,
                endData,
                baseIsStartSide,
                targetTilemap.gameObject.name
            );
        }

        private int BuildCrossTilemapZone(
            BiomeBlendZone2D zone,
            Transform zoneRoot)
        {
            Tilemap startTilemap =
                zone.StartTilemap;

            Tilemap endTilemap =
                zone.EndTilemap;

            if (startTilemap == null ||
                endTilemap == null)
            {
                Debug.LogWarning(
                    "BiomeBlendController2D: Cross Tilemap zone '" +
                    zone.gameObject.name +
                    "' needs both Start Tilemap and End Tilemap.",
                    zone
                );

                return 0;
            }

            if (startTilemap == endTilemap)
            {
                return BuildZoneOnSingleTilemap(
                    zone,
                    startTilemap,
                    zoneRoot
                );
            }

            TilemapRenderer startRenderer =
                startTilemap.GetComponent<
                    TilemapRenderer
                >();

            TilemapRenderer endRenderer =
                endTilemap.GetComponent<
                    TilemapRenderer
                >();

            if (startRenderer == null ||
                endRenderer == null)
            {
                Debug.LogWarning(
                    "BiomeBlendController2D: Cross Tilemap zone '" +
                    zone.gameObject.name +
                    "' has a Tilemap without a TilemapRenderer.",
                    zone
                );

                return 0;
            }

            TilemapRenderer topRenderer =
                GetHigherRenderer(
                    startRenderer,
                    endRenderer
                );

            int total = 0;

            GameObject startRoot =
                new GameObject(
                    "Start_" +
                    startTilemap.gameObject.name
                );

            startRoot.transform.SetParent(
                zoneRoot,
                false
            );

            int startCount =
                BuildCrossSide(
                    zone,
                    startTilemap,
                    endTilemap,
                    startRenderer,
                    endRenderer,
                    topRenderer,
                    true,
                    startRoot.transform
                );

            if (startCount == 0)
            {
                DestroyUnityObject(
                    startRoot
                );
            }

            total +=
                startCount;

            GameObject endRoot =
                new GameObject(
                    "End_" +
                    endTilemap.gameObject.name
                );

            endRoot.transform.SetParent(
                zoneRoot,
                false
            );

            int endCount =
                BuildCrossSide(
                    zone,
                    endTilemap,
                    startTilemap,
                    endRenderer,
                    startRenderer,
                    topRenderer,
                    false,
                    endRoot.transform
                );

            if (endCount == 0)
            {
                DestroyUnityObject(
                    endRoot
                );
            }

            total +=
                endCount;

            return total;
        }

        private int BuildCrossSide(
            BiomeBlendZone2D zone,
            Tilemap baseTilemap,
            Tilemap oppositeTilemap,
            TilemapRenderer baseRenderer,
            TilemapRenderer oppositeRenderer,
            TilemapRenderer topRenderer,
            bool baseIsStartSide,
            Transform generatedRoot)
        {
            Bounds worldBounds =
                zone.GetWorldBounds();

            Vector3Int minimumCell =
                baseTilemap.WorldToCell(
                    worldBounds.min
                );

            Vector3Int maximumCell =
                baseTilemap.WorldToCell(
                    worldBounds.max
                );

            int generatedCount = 0;

            for (int x = minimumCell.x;
                 x <= maximumCell.x;
                 x++)
            {
                for (int y = minimumCell.y;
                     y <= maximumCell.y;
                     y++)
                {
                    Vector3Int cell =
                        new Vector3Int(
                            x,
                            y,
                            0
                        );

                    Sprite baseSprite =
                        baseTilemap.GetSprite(
                            cell
                        );

                    if (baseSprite == null)
                        continue;

                    Vector3 cellCenter =
                        baseTilemap
                            .GetCellCenterWorld(
                                cell
                            );

                    Vector3Int currentOppositeCell =
                        oppositeTilemap.WorldToCell(
                            cellCenter
                        );

                    Sprite currentOppositeSprite =
                        oppositeTilemap.GetSprite(
                            currentOppositeCell
                        );

                    if (currentOppositeSprite != null &&
                        !ShouldProcessVisibleBase(
                            baseRenderer,
                            oppositeRenderer,
                            baseIsStartSide))
                    {
                        continue;
                    }

                    SpritePixelData baseData =
                        GetSpritePixelData(
                            baseSprite
                        );

                    if (baseData == null)
                        continue;

                    Vector3Int oppositeSourceCell;

                    Sprite oppositeSprite =
                        currentOppositeSprite;

                    if (oppositeSprite != null)
                    {
                        oppositeSourceCell =
                            currentOppositeCell;
                    }
                    else
                    {
                        oppositeSprite =
                            FindOutsideSampleSprite(
                                oppositeTilemap,
                                zone,
                                cellCenter,
                                !baseIsStartSide,
                                out oppositeSourceCell
                            );
                    }

                    if (oppositeSprite == null)
                        continue;

                    SpritePixelData oppositeData =
                        GetSpritePixelData(
                            oppositeSprite
                        );

                    if (oppositeData == null)
                        continue;

                    SpritePixelData startData;
                    SpritePixelData endData;

                    if (baseIsStartSide)
                    {
                        startData =
                            baseData;

                        endData =
                            oppositeData;
                    }
                    else
                    {
                        startData =
                            oppositeData;

                        endData =
                            baseData;
                    }

                    Sprite generatedSprite =
                        CreateTransparentBlendTexture(
                            zone,
                            cell,
                            cellCenter,
                            baseSprite,
                            baseData,
                            startData,
                            endData,
                            baseIsStartSide,
                            baseTilemap.gameObject.name +
                            "_Cross_" +
                            oppositeTilemap.gameObject.name
                        );

                    if (generatedSprite == null)
                        continue;

                    Color overlayColor =
                        MultiplyColors(
                            oppositeTilemap.color,
                            oppositeTilemap.GetColor(
                                oppositeSourceCell
                            )
                        );

                    CreateGeneratedRenderer(
                        generatedRoot,
                        generatedSprite,
                        cellCenter,
                        topRenderer,
                        oppositeRenderer.sharedMaterial,
                        overlayColor,
                        "CrossBlend_" +
                        cell.x +
                        "_" +
                        cell.y
                    );

                    generatedCount++;
                }
            }

            return generatedCount;
        }

        private Sprite FindOutsideSampleSprite(
            Tilemap tilemap,
            BiomeBlendZone2D zone,
            Vector3 referenceWorldPoint,
            bool sampleStartSide,
            out Vector3Int foundCell)
        {
            float cellSize =
                GetApproximateCellSize(
                    tilemap
                );

            float[] paddingSteps =
            {
            0.55f,
            1.55f,
            2.55f,
            3.55f,
            4.55f
        };

            for (int i = 0;
                 i < paddingSteps.Length;
                 i++)
            {
                Vector3 sampleWorld =
                    zone.GetOutsideSamplePoint(
                        referenceWorldPoint,
                        sampleStartSide,
                        cellSize *
                        paddingSteps[i]
                    );

                Vector3Int sampleCell =
                    tilemap.WorldToCell(
                        sampleWorld
                    );

                Sprite sprite =
                    tilemap.GetSprite(
                        sampleCell
                    );

                if (sprite != null)
                {
                    foundCell =
                        sampleCell;

                    return sprite;
                }
            }

            foundCell =
                Vector3Int.zero;

            return null;
        }

        private Sprite CreateTransparentBlendTexture(
            BiomeBlendZone2D zone,
            Vector3Int cell,
            Vector3 cellCenter,
            Sprite baseSprite,
            SpritePixelData baseData,
            SpritePixelData startData,
            SpritePixelData endData,
            bool baseIsStartSide,
            string textureNamePrefix)
        {
            int width =
                baseData.width;

            int height =
                baseData.height;

            Color32[] outputPixels =
                new Color32[
                    width *
                    height
                ];

            Color32 transparentPixel =
                new Color32(
                    0,
                    0,
                    0,
                    0
                );

            for (int i = 0;
                 i < outputPixels.Length;
                 i++)
            {
                outputPixels[i] =
                    transparentPixel;
            }

            Vector3 cellWorldSize =
                GetSpriteWorldSize(
                    baseSprite
                );

            bool drewAnyOverlayPixel =
                false;

            for (int pixelY = 0;
                 pixelY < height;
                 pixelY++)
            {
                for (int pixelX = 0;
                     pixelX < width;
                     pixelX++)
                {
                    int index =
                        pixelY *
                        width +
                        pixelX;

                    float normalizedX =
                        (
                            pixelX +
                            0.5f
                        ) /
                        width;

                    float normalizedY =
                        (
                            pixelY +
                            0.5f
                        ) /
                        height;

                    Vector3 pixelWorld =
                        cellCenter +
                        new Vector3(
                            (
                                normalizedX -
                                0.5f
                            ) *
                            cellWorldSize.x,

                            (
                                normalizedY -
                                0.5f
                            ) *
                            cellWorldSize.y,

                            0f
                        );

                    if (!zone.ContainsWorldPoint(
                            pixelWorld))
                    {
                        continue;
                    }

                    float blend =
                        zone.GetBlend01(
                            pixelWorld
                        );

                    int blockSize =
                        zone.NoiseBlockSize;

                    int globalPixelX =
                        cell.x *
                        width +
                        pixelX;

                    int globalPixelY =
                        cell.y *
                        height +
                        pixelY;

                    int noiseX =
                        FloorDivide(
                            globalPixelX,
                            blockSize
                        );

                    int noiseY =
                        FloorDivide(
                            globalPixelY,
                            blockSize
                        );

                    float noise =
                        Hash01(
                            noiseX,
                            noiseY,
                            zone.Seed
                        );

                    float threshold =
                        Mathf.Lerp(
                            0.5f,
                            noise,
                            zone.Irregularity
                        );

                    bool chooseEndBiome =
                        blend >= threshold;

                    bool drawOppositeBiome;

                    if (baseIsStartSide)
                    {
                        drawOppositeBiome =
                            chooseEndBiome;
                    }
                    else
                    {
                        drawOppositeBiome =
                            !chooseEndBiome;
                    }

                    if (!drawOppositeBiome)
                        continue;

                    Color32 overlayPixel;

                    if (baseIsStartSide)
                    {
                        overlayPixel =
                            SampleNearest(
                                endData,
                                normalizedX,
                                normalizedY
                            );
                    }
                    else
                    {
                        overlayPixel =
                            SampleNearest(
                                startData,
                                normalizedX,
                                normalizedY
                            );
                    }

                    if (overlayPixel.a == 0)
                        continue;

                    outputPixels[index] =
                        overlayPixel;

                    drewAnyOverlayPixel = true;
                }
            }

            if (!drewAnyOverlayPixel)
                return null;

            Texture2D generatedTexture =
                new Texture2D(
                    width,
                    height,
                    TextureFormat.RGBA32,
                    false,
                    false
                );

            generatedTexture.name =
                "BiomeBlendTexture_" +
                textureNamePrefix +
                "_" +
                cell.x +
                "_" +
                cell.y;

            generatedTexture.filterMode =
                FilterMode.Point;

            generatedTexture.wrapMode =
                TextureWrapMode.Clamp;

            generatedTexture.SetPixels32(
                outputPixels
            );

            generatedTexture.Apply(
                false,
                false
            );

            Sprite generatedSprite =
                Sprite.Create(
                    generatedTexture,
                    new Rect(
                        0f,
                        0f,
                        width,
                        height
                    ),
                    new Vector2(
                        0.5f,
                        0.5f
                    ),
                    baseSprite.pixelsPerUnit,
                    0,
                    SpriteMeshType.FullRect
                );

            generatedSprite.name =
                "BiomeBlendSprite_" +
                textureNamePrefix +
                "_" +
                cell.x +
                "_" +
                cell.y;

            return generatedSprite;
        }

        private void CreateGeneratedRenderer(
            Transform parent,
            Sprite sprite,
            Vector3 worldPosition,
            TilemapRenderer sortingReference,
            Material material,
            Color color,
            string objectName)
        {
            GameObject cellObject =
                new GameObject(
                    objectName
                );

            cellObject.transform.SetParent(
                parent,
                true
            );

            cellObject.transform.position =
                worldPosition;

            SpriteRenderer spriteRenderer =
                cellObject.AddComponent<
                    SpriteRenderer
                >();

            spriteRenderer.sprite =
                sprite;

            spriteRenderer.sortingLayerID =
                sortingReference.sortingLayerID;

            spriteRenderer.sortingOrder =
                sortingReference.sortingOrder +
                sortingOrderOffset;

            spriteRenderer.sharedMaterial =
                material;

            spriteRenderer.color =
                color;
        }

        private static bool ShouldProcessVisibleBase(
            TilemapRenderer baseRenderer,
            TilemapRenderer oppositeRenderer,
            bool baseIsStartSide)
        {
            int comparison =
                CompareRendererPriority(
                    baseRenderer,
                    oppositeRenderer
                );

            if (comparison > 0)
                return true;

            if (comparison < 0)
                return false;

            return !baseIsStartSide;
        }

        private static int CompareRendererPriority(
            TilemapRenderer a,
            TilemapRenderer b)
        {
            int layerA =
                SortingLayer.GetLayerValueFromID(
                    a.sortingLayerID
                );

            int layerB =
                SortingLayer.GetLayerValueFromID(
                    b.sortingLayerID
                );

            if (layerA != layerB)
            {
                return layerA.CompareTo(
                    layerB
                );
            }

            if (a.sortingOrder !=
                b.sortingOrder)
            {
                return a.sortingOrder.CompareTo(
                    b.sortingOrder
                );
            }

            return 0;
        }

        private static TilemapRenderer GetHigherRenderer(
            TilemapRenderer a,
            TilemapRenderer b)
        {
            int comparison =
                CompareRendererPriority(
                    a,
                    b
                );

            if (comparison >= 0)
                return a;

            return b;
        }

        private SpritePixelData GetSpritePixelData(
            Sprite sprite)
        {
            if (sprite == null)
                return null;

            if (spriteCache.TryGetValue(
                    sprite,
                    out SpritePixelData cached))
            {
                return cached;
            }

            SpritePixelData data =
                ReadSpritePixelsWithGPU(
                    sprite
                );

            if (data != null)
            {
                spriteCache.Add(
                    sprite,
                    data
                );
            }

            return data;
        }

        private SpritePixelData ReadSpritePixelsWithGPU(
            Sprite sprite)
        {
            Texture2D sourceTexture =
                sprite.texture;

            if (sourceTexture == null)
                return null;

            Rect textureRect =
                sprite.textureRect;

            int width =
                Mathf.RoundToInt(
                    textureRect.width
                );

            int height =
                Mathf.RoundToInt(
                    textureRect.height
                );

            if (width <= 0 ||
                height <= 0)
            {
                return null;
            }

            FilterMode originalFilterMode =
                sourceTexture.filterMode;

            RenderTexture previousActive =
                RenderTexture.active;

            RenderTexture temporary =
                null;

            Texture2D readableTexture =
                null;

            try
            {
                sourceTexture.filterMode =
                    FilterMode.Point;

                temporary =
                    RenderTexture.GetTemporary(
                        width,
                        height,
                        0,
                        RenderTextureFormat.ARGB32,
                        RenderTextureReadWrite.Default
                    );

                temporary.filterMode =
                    FilterMode.Point;

                Vector2 scale =
                    new Vector2(
                        textureRect.width /
                        sourceTexture.width,

                        textureRect.height /
                        sourceTexture.height
                    );

                Vector2 offset =
                    new Vector2(
                        textureRect.x /
                        sourceTexture.width,

                        textureRect.y /
                        sourceTexture.height
                    );

                Graphics.Blit(
                    sourceTexture,
                    temporary,
                    scale,
                    offset
                );

                RenderTexture.active =
                    temporary;

                readableTexture =
                    new Texture2D(
                        width,
                        height,
                        TextureFormat.RGBA32,
                        false,
                        false
                    );

                readableTexture.ReadPixels(
                    new Rect(
                        0f,
                        0f,
                        width,
                        height
                    ),
                    0,
                    0,
                    false
                );

                readableTexture.Apply(
                    false,
                    false
                );

                SpritePixelData data =
                    new SpritePixelData
                    {
                        pixels =
                            readableTexture
                                .GetPixels32(),

                        width =
                            width,

                        height =
                            height,

                        pixelsPerUnit =
                            sprite.pixelsPerUnit
                    };

                return data;
            }
            finally
            {
                sourceTexture.filterMode =
                    originalFilterMode;

                RenderTexture.active =
                    previousActive;

                if (temporary != null)
                {
                    RenderTexture.ReleaseTemporary(
                        temporary
                    );
                }

                if (readableTexture != null)
                {
                    DestroyUnityObject(
                        readableTexture
                    );
                }
            }
        }

        private static Color32 SampleNearest(
            SpritePixelData data,
            float normalizedX,
            float normalizedY)
        {
            int x =
                Mathf.Clamp(
                    Mathf.FloorToInt(
                        normalizedX *
                        data.width
                    ),
                    0,
                    data.width - 1
                );

            int y =
                Mathf.Clamp(
                    Mathf.FloorToInt(
                        normalizedY *
                        data.height
                    ),
                    0,
                    data.height - 1
                );

            return data.pixels[
                y *
                data.width +
                x
            ];
        }

        private Vector3 GetSpriteWorldSize(
            Sprite sprite)
        {
            if (sprite == null ||
                sprite.pixelsPerUnit <= 0f)
            {
                return Vector3.one;
            }

            return new Vector3(
                sprite.rect.width /
                sprite.pixelsPerUnit,

                sprite.rect.height /
                sprite.pixelsPerUnit,

                0f
            );
        }

        private float GetApproximateCellSize(
            Tilemap targetTilemap)
        {
            if (targetTilemap == null)
                return 1f;

            GridLayout layoutGrid =
                targetTilemap.layoutGrid;

            if (layoutGrid == null)
                return 1f;

            Vector3 size =
                layoutGrid.cellSize;

            return Mathf.Max(
                Mathf.Abs(size.x),
                Mathf.Abs(size.y)
            );
        }

        private static float Hash01(
            int x,
            int y,
            int seed)
        {
            unchecked
            {
                uint hash =
                    (uint)seed;

                hash ^=
                    (uint)x *
                    374761393u;

                hash =
                    (
                        hash << 13
                    ) ^
                    hash;

                hash ^=
                    (uint)y *
                    668265263u;

                hash *=
                    1274126177u;

                hash ^=
                    hash >> 16;

                return
                    (
                        hash &
                        0x00FFFFFFu
                    ) /
                    16777215f;
            }
        }

        private static int FloorDivide(
            int value,
            int divisor)
        {
            int result =
                value /
                divisor;

            int remainder =
                value %
                divisor;

            if (remainder != 0 &&
                (
                    remainder < 0
                ) !=
                (
                    divisor < 0
                ))
            {
                result--;
            }

            return result;
        }

        private static Color MultiplyColors(
            Color a,
            Color b)
        {
            return new Color(
                a.r * b.r,
                a.g * b.g,
                a.b * b.b,
                a.a * b.a
            );
        }

        private static void DestroyUnityObject(
            Object target)
        {
            if (target == null)
                return;

            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
        }
    }
}