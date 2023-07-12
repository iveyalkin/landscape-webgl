using System.Collections.Generic;
using UnityEngine;

namespace IV.PAL
{
    [CreateAssetMenu(fileName = "PALSettings", menuName = "PAL/PALSettings", order = 0)]
    public class PALSettings : ScriptableObject
    {
        [Header("General")]
        [Tooltip("Average fade in duration in seconds")]
        [Min(0f)]
        public float layerFadeInDuration = 3f;

        [Tooltip("Average fade out duration in seconds")]
        [Min(0f)]
        public float layerFadeOutDuration = 3f;

        [Header("Ambience Autopilot")]
        [Tooltip("Should the controller automatically fade in/out layers to make change the music? If false, all layers will be played")]
        public bool enableAutopilot = true;

        [Tooltip("Only matters when enableAutopilot is true")]
        [Min(0)]
        public int numAutopilotLayers = 2;

        [Tooltip("Average fade timeout in seconds")]
        [Min(0f)]
        public float avgFadeTimeout = 10f;

        [Tooltip("Variance of fade timeout in seconds")]
        [Min(0f)]
        public float fadeTimeoutVariance = 2f;

        [Tooltip("Variance of ;ayer number")]
        [Min(0)]
        public int layerNumberVariance = 1;

        [Tooltip("Change layers automatically")]
        public bool changeLayersAutomatically = true;

        [Tooltip("Show detailed info")]
        public bool showDetailedInfo = true;

        [Header("Ambience Controller")]
        [Tooltip("Fade in on start")]
        public bool fadeInOnStart = true;

        [Tooltip("Ambience Master fade in/out duration")]
        [Min(0f)]
        public float masterFadeDuration = 3f;

        [Tooltip("Ambience Master volume")]
        [Range(0f, 1f)]
        public float ambienceMasterVolume = 1f;

        [Header("Layers")]
        public AudioCue[] layerCues;

        [Header("Debug")]
        public bool IsVerboseDebug = false;
    }
}