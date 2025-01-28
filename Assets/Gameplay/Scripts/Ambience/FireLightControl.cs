using UnityEngine;

namespace IV.Gameplay.Ambience
{
    [RequireComponent(typeof(Light))]
    public partial class FireLightControl : MonoBehaviour
    {
        [Range(0f, 100f)] [SerializeField] private float minIntensity = 1f;
        [Range(0f, 100f)] [SerializeField] private float maxIntensity = 10f;

        private float seed;

        private void OnEnable()
        {
            seed = Random.value;
        }

        // FireLight Blinking
        private void Update()
        {
            // todo: replace with baked texture of the noise for performance
            var noise = Mathf.PerlinNoise1D(Time.time + seed);
            light.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
    }
}