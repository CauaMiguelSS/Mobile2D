using System.Collections;
using UnityEngine;
namespace Timbeaux.ModularGroundKit
{
    [RequireComponent(typeof(Camera))]
    public class DemoCameraTour2D : MonoBehaviour
    {
        [System.Serializable]
        public class CameraPoint
        {
            [Header("Position")]
            public Transform target;

            [Min(0f)]
            public float travelTime = 2f;

            [Min(0f)]
            public float pauseTime = 3f;

            [Header("Zoom")]
            public bool changeZoom = false;

            [Min(0.01f)]
            public float orthographicSize = 5f;

            [Header("Stop Sound")]
            [Tooltip(
                "Optional sound played when the camera reaches this stop."
            )]
            public AudioClip stopSound;

            [Range(0f, 1f)]
            public float stopSoundVolume = 1f;
        }

        [Header("Tour")]
        [SerializeField]
        private CameraPoint[] points;

        [SerializeField]
        private bool loop = true;

        [Header("Movement")]
        [SerializeField]
        private AnimationCurve movementCurve =
            AnimationCurve.EaseInOut(
                0f,
                0f,
                1f,
                1f
            );

        [Header("Pixel Snapping")]
        [Tooltip(
            "Pixels Per Unit used by the environment artwork."
        )]
        [SerializeField]
        private float pixelsPerUnit = 32f;

        [Header("Audio")]
        [Tooltip(
            "Optional AudioSource used for stop sounds. " +
            "If none is assigned, one will be created automatically."
        )]
        [SerializeField]
        private AudioSource stopAudioSource;

        private Camera targetCamera;
        private Coroutine tourRoutine;

        private void Awake()
        {
            targetCamera =
                GetComponent<Camera>();

            SetupAudioSource();
        }

        private void Start()
        {
            if (points == null ||
                points.Length == 0)
            {
                return;
            }

            tourRoutine =
                StartCoroutine(
                    RunTour()
                );
        }

        private void SetupAudioSource()
        {
            if (stopAudioSource != null)
                return;

            stopAudioSource =
                GetComponent<AudioSource>();

            if (stopAudioSource == null)
            {
                stopAudioSource =
                    gameObject.AddComponent<AudioSource>();
            }

            stopAudioSource.playOnAwake = false;
            stopAudioSource.loop = false;
            stopAudioSource.spatialBlend = 0f;
        }

        private IEnumerator RunTour()
        {
            do
            {
                for (int i = 0;
                     i < points.Length;
                     i++)
                {
                    CameraPoint point =
                        points[i];

                    if (point == null ||
                        point.target == null)
                    {
                        continue;
                    }

                    yield return
                        MoveToPoint(
                            point
                        );

                    PlayStopSound(
                        point
                    );

                    if (point.pauseTime > 0f)
                    {
                        yield return
                            new WaitForSeconds(
                                point.pauseTime
                            );
                    }
                }
            }
            while (loop);

            tourRoutine = null;
        }

        private IEnumerator MoveToPoint(
            CameraPoint point)
        {
            Vector3 startPosition =
                transform.position;

            Vector3 targetPosition =
                point.target.position;

            targetPosition.z =
                startPosition.z;

            float startZoom =
                targetCamera.orthographicSize;

            float targetZoom =
                point.changeZoom
                    ? point.orthographicSize
                    : startZoom;

            if (point.travelTime <= 0f)
            {
                transform.position =
                    SnapToPixelGrid(
                        targetPosition
                    );

                if (point.changeZoom)
                {
                    targetCamera.orthographicSize =
                        targetZoom;
                }

                yield break;
            }

            float elapsed = 0f;

            while (elapsed <
                   point.travelTime)
            {
                elapsed +=
                    Time.deltaTime;

                float normalizedTime =
                    Mathf.Clamp01(
                        elapsed /
                        point.travelTime
                    );

                float curvedTime =
                    movementCurve.Evaluate(
                        normalizedTime
                    );

                Vector3 interpolatedPosition =
                    Vector3.Lerp(
                        startPosition,
                        targetPosition,
                        curvedTime
                    );

                transform.position =
                    SnapToPixelGrid(
                        interpolatedPosition
                    );

                if (point.changeZoom)
                {
                    targetCamera.orthographicSize =
                        Mathf.Lerp(
                            startZoom,
                            targetZoom,
                            curvedTime
                        );
                }

                yield return null;
            }

            transform.position =
                SnapToPixelGrid(
                    targetPosition
                );

            if (point.changeZoom)
            {
                targetCamera.orthographicSize =
                    targetZoom;
            }
        }

        private void PlayStopSound(
            CameraPoint point)
        {
            if (point.stopSound == null ||
                stopAudioSource == null)
            {
                return;
            }

            stopAudioSource.PlayOneShot(
                point.stopSound,
                point.stopSoundVolume
            );
        }

        private Vector3 SnapToPixelGrid(
            Vector3 position)
        {
            if (pixelsPerUnit <= 0f)
                return position;

            float unitsPerPixel =
                1f /
                pixelsPerUnit;

            position.x =
                Mathf.Round(
                    position.x /
                    unitsPerPixel
                ) *
                unitsPerPixel;

            position.y =
                Mathf.Round(
                    position.y /
                    unitsPerPixel
                ) *
                unitsPerPixel;

            return position;
        }
    }
}