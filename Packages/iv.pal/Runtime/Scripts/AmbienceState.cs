using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;

namespace IV.PAL
{
    [Serializable]
    public class AmbienceState : ILayersState
    {
        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private string volumeParameter;

        private readonly List<LayerData> layers = new(8);

        // todo: get rid of LINQ
        public int ActiveLayersCount => PlayingLayers.Count();
        
        public int TotalLayersCount => layers.Count;

        public bool IsMuted
        {
            get
            {
                audioMixer.GetFloat(volumeParameter, out var volume);
                return volume <= -80f;
            }
            set
            {
                var volume = value ? -80f : 0f;
                audioMixer.SetFloat(volumeParameter, volume);
            }
        }

        public float MasterVolume
        {
            get
            {
                audioMixer.GetFloat(volumeParameter, out var volume);
                // todo convert to linear scale
                return volume;
            }
            set
            {
                // todo convert to log scale
                var logScale = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20f;
                audioMixer.SetFloat(volumeParameter, logScale);
            }
        }

        // todo: get rid of LINQ
        public IEnumerable<LayerData> PlayingLayers => layers.Where(layer => layer.IsPlaying);
        public IEnumerable<LayerData> StoppedLayers => layers.Where(layer => !layer.IsPlaying);

        public AmbienceState(string volumeParameter)
        {
            this.volumeParameter = volumeParameter;
        }

        public void AddLayer(AudioCue layerCue)
        {
            layers.Add(new LayerData(layerCue));
        }

        public IEnumerable<LayerData> GetRandomLayers(int targetCount)
        {
            // todo implement random selection
            return layers.Take(targetCount);
        }
    }
}