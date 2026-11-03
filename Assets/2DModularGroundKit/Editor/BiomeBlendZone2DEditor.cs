using Timbeaux.ModularGroundKit;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Timbeaux.ModularGroundKit.Editor
{
    [CustomEditor(typeof(BiomeBlendZone2D))]
    public class BiomeBlendZone2DEditor : UnityEditor.Editor
    {
        private readonly BoxBoundsHandle boundsHandle =
            new BoxBoundsHandle();

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Select this object in the Scene view and drag the handles to resize the biome blend zone.",
                MessageType.Info
            );
        }

        private void OnSceneGUI()
        {
            BiomeBlendZone2D zone =
                (BiomeBlendZone2D)target;

            if (zone == null)
                return;

            BoxCollider2D collider =
                zone.BoundsCollider;

            if (collider == null)
                return;

            using (
                new Handles.DrawingScope(
                    zone.GizmoColor,
                    zone.transform.localToWorldMatrix
                )
            )
            {
                boundsHandle.center =
                    collider.offset;

                boundsHandle.size =
                    collider.size;

                EditorGUI.BeginChangeCheck();

                boundsHandle.DrawHandle();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(
                        collider,
                        "Resize Biome Blend Zone"
                    );

                    collider.offset =
                        boundsHandle.center;

                    Vector3 handleSize =
                        boundsHandle.size;

                    collider.size =
                        new Vector2(
                            Mathf.Max(
                                0.01f,
                                handleSize.x
                            ),
                            Mathf.Max(
                                0.01f,
                                handleSize.y
                            )
                        );

                    EditorUtility.SetDirty(
                        collider
                    );

                    EditorUtility.SetDirty(
                        zone
                    );
                }
            }

            DrawDirectionLabel(
                zone
            );
        }

        private void DrawDirectionLabel(
            BiomeBlendZone2D zone)
        {
            if (!zone.ShowDirection)
                return;

            BoxCollider2D collider =
                zone.BoundsCollider;

            if (collider == null)
                return;

            Vector3 localPosition =
                new Vector3(
                    collider.offset.x,
                    collider.offset.y -
                    collider.size.y * 0.5f -
                    0.25f,
                    0f
                );

            Vector3 worldPosition =
                zone.transform.TransformPoint(
                    localPosition
                );

            GUIStyle labelStyle =
                new GUIStyle(
                    EditorStyles.boldLabel
                );

            labelStyle.alignment =
                TextAnchor.MiddleCenter;

            Handles.Label(
                worldPosition,
                "Biome Blend: " +
                GetDirectionText(
                    zone.Direction
                ),
                labelStyle
            );
        }

        private string GetDirectionText(
            BiomeBlendZone2D.BlendDirection direction)
        {
            switch (direction)
            {
                case BiomeBlendZone2D.BlendDirection.RightToLeft:
                    return "Right To Left";

                case BiomeBlendZone2D.BlendDirection.BottomToTop:
                    return "Bottom To Top";

                case BiomeBlendZone2D.BlendDirection.TopToBottom:
                    return "Top To Bottom";

                default:
                    return "Left To Right";
            }
        }
    }
}