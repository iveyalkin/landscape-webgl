using System.Collections.Generic;
using UnityEngine;

namespace IV.PAL
{
    [CreateAssetMenu(fileName = "AudioCue", menuName = "PAL/AudioCue", order = 0)]
    public class AudioCue : ScriptableObject
    {
        [SerializeField]
        private AudioClip[] clips;

        public IEnumerable<AudioClip> Clips => clips;

        [field: SerializeField]
        public float BaseVolume { get; private set; }

        [field: SerializeField]
        public bool Loop { get; private set; } = true;

        /// <summary>
        /// Default Random clip getter
        /// </summary>
        public AudioClip RandomClip =>
            clips.Length == 0 ? null : clips[Random.Range(0, clips.Length)];
    }
}