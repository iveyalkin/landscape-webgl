using UnityEngine;
using UnityEngine.Assertions;

namespace IV.PAL
{
    public class LayerData
    {
        public AudioSource Source { get; private set; }
        public AudioCue Cue { get; }

        public float BaseVolume => Cue.BaseVolume;
        public bool IsPlaying => Source != null && Source.isPlaying;
        public string ClipName => Cue.name;
        public float Volume
        {
            // todo convert to linear scale
            get => Source != null ? Source.volume : 0f;
            set
            {
                if (Source == null) return;

                // todo convert to log scale
                Source.volume = value;
            }
        }

        public bool IsMute
        {
            get => Source == null || Source.mute;
            set
            {
                if (Source == null) return;

                Source.mute = value;
            }
        }

        public LayerData(AudioCue cue)
        {
            Cue = cue;
        }

        public AudioSource Stop()
        {
            Assert.IsTrue(IsPlaying, "Trying to stop a layer that is not playing");

            var audioSource = Source;
            Source = null;

            audioSource.Stop();
            return audioSource;
        }

        public void Play(AudioSource audioSource, float volume = -1f)
        {
            Assert.IsFalse(IsPlaying, "Trying to play a layer that is already playing");

            audioSource.loop = Cue.Loop;
            audioSource.clip = Cue.RandomClip;
            audioSource.volume = volume < 0f ? BaseVolume : volume;
            audioSource.Play();

            // todo: recycle when loop is false and clip ends

            Source = audioSource;
        }
    }
}