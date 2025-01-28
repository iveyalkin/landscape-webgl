using System;
using UnityEngine;

namespace IV.Gameplay.Ambience
{
    [RequireComponent(typeof(Light))]
    public partial class DayNightCycle : MonoBehaviour
    {
        [Range(0, 24)] [SerializeField] private int cycleLength = 24;

        [SerializeField] private CelestialBody sun;
        [SerializeField] private CelestialBody moon;

        private void Update()
        {
            var time = Time.time;

            if (sun.Tick(light, time, cycleLength))
            {
                light.enabled = true;
                return;
            }

            if (moon.Tick(light, time, cycleLength))
            {
                light.enabled = true;
                return;
            }

            light.enabled = false;
        }


        [Serializable]
        public struct CelestialBody
        {
            [Range(1000f, 20_000f)] public float minColorTemperature;
            [Range(1000f, 20_000f)] public float maxColorTemperature;
            [Range(0f, 1f)] public float phase;
            [Range(0f, 1f)] public float upTime;
            public AnimationCurve colorTemperatureCurve;
            public AnimationCurve intensityCurve;

            public void SetIntensity(Light light, float timeNormalized)
            {
                light.colorTemperature = Mathf.Lerp(minColorTemperature, maxColorTemperature,
                    colorTemperatureCurve.Evaluate(timeNormalized));
                light.intensity = intensityCurve.Evaluate(timeNormalized);
            }

            public bool Tick(Light light, float time, float cycleLength)
            {
                time = (time + cycleLength * phase) % cycleLength;

                var timeNormalized = time / cycleLength;

                if (timeNormalized >= upTime) return false;

                timeNormalized = Mathf.Clamp01(timeNormalized / upTime);

                var angle = timeNormalized * 180f;
                light.transform.rotation = Quaternion.Euler(angle, 0, 0);

                SetIntensity(light, timeNormalized);

                return true;
            }
        }
    }
}