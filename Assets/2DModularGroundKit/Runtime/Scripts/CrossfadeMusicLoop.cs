using UnityEngine;
using UnityEngine.Audio;
namespace Timbeaux.ModularGroundKit
{


    public class CrossfadeMusicLoop : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private AudioClip loopClip;
        [SerializeField, Range(0.01f, 1f)] private float crossfadeSeconds = 0.15f;

        [Header("Audio Options")]
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField] private bool mute;
        [SerializeField, Range(0, 255)] private int priority = 128;
        [SerializeField] private AudioMixerGroup outputMixerGroup;

        private readonly AudioSource[] sources = new AudioSource[2];
        private int current;
        private int previous;
        private double currentStart;
        private double transitionEnd;
        private bool nextScheduled;

        private void Start()
        {
            if (loopClip == null)
            {
                Debug.LogError("Assign a Loop Clip.", this);
                enabled = false;
                return;
            }

            crossfadeSeconds = Mathf.Min(crossfadeSeconds, loopClip.length * 0.25f);

            for (int i = 0; i < 2; i++)
            {
                sources[i] = gameObject.AddComponent<AudioSource>();
                sources[i].clip = loopClip;
                sources[i].playOnAwake = false;
                sources[i].loop = false;
                sources[i].spatialBlend = 0f;
                sources[i].priority = priority;
                sources[i].outputAudioMixerGroup = outputMixerGroup;
                sources[i].mute = mute;
                sources[i].volume = 0f;
            }

            current = 0;
            previous = 1;
            currentStart = AudioSettings.dspTime + 0.2;
            sources[current].volume = volume;
            sources[current].PlayScheduled(currentStart);
        }

        private void Update()
        {
            double now = AudioSettings.dspTime;
            double nextStart = currentStart + loopClip.length - crossfadeSeconds;

            foreach (AudioSource source in sources)
            {
                source.priority = priority;
                source.outputAudioMixerGroup = outputMixerGroup;
                source.mute = mute;
            }

            if (!nextScheduled && now >= nextStart - 0.5)
            {
                int next = 1 - current;
                sources[next].volume = 0f;
                sources[next].PlayScheduled(nextStart);
                nextScheduled = true;
            }

            if (nextScheduled && now >= nextStart)
            {
                previous = current;
                current = 1 - current;
                currentStart = nextStart;
                transitionEnd = nextStart + crossfadeSeconds;
                nextScheduled = false;
            }

            if (now < transitionEnd)
            {
                float t = Mathf.Clamp01((float)((now - currentStart) / crossfadeSeconds));
                sources[previous].volume = volume * (1f - t);
                sources[current].volume = volume * t;
            }
            else
            {
                sources[previous].volume = 0f;
                sources[current].volume = volume;
            }
        }

        private void OnDisable()
        {
            foreach (AudioSource source in sources)
                if (source != null) source.Stop();
        }
    }
}