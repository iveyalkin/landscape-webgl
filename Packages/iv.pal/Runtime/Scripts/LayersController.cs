using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using IV.Core.Pools;

namespace IV.PAL
{
    public sealed class LayersController : MonoBehaviour, ILayersController
    {
        [SerializeField] private AmbienceState state = new(
            volumeParameter: "ambienceVolume");

        [SerializeField] private PALSettings settings;

        [SerializeField]
        private GameObjectPool<AudioSource> audioSourcePool = new();

        [SerializeField]
        private AmbienceAutopilot autopilot;

        /// <returns>Ambient Master volume</returns>
        public float Volume
        {
            get => state.MasterVolume;
            set => state.MasterVolume = value;
        }

        private void Awake()
        {
            // Initialize layers
            foreach (var layerCue in settings.layerCues)
            {
                state.AddLayer(layerCue);
            }
        }

        private async void OnEnable()
        {
            try
            {
                if (!settings.fadeInOnStart) return;

                if (settings.enableAutopilot)
                    await autopilot.StartAutopilotAsync(this, state);
                else
                    await FadeInAllAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnDisable()
        {
            autopilot.StopAutopilot();
        }

        public async Awaitable FadeInAllAsync(float duration = -1f, CancellationToken cancellationToken = default)
        {
            duration = duration < 0f ? settings.masterFadeDuration : duration;
            cancellationToken.ThrowIfCancellationRequested();

            var startTime = Time.time;
            var startVolume = state.MasterVolume;

            while (Time.time - startTime < duration)
            {
                var t = (Time.time - startTime) / duration;
                state.MasterVolume = Mathf.Lerp(0, startVolume, t);
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Play all layers.
        /// </summary>
        public async Awaitable PlayAllAsync(CancellationToken cancellationToken = default)
        {
            await PlayAllAsync(fadeInDuration: 0, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Play all layers that are on the same GameObject as this component.
        /// </summary>
        /// <param name="fadeInDuration">Duration of volume increase in seconds.</param>
        /// <param name="ignoreTimeScale">Duration of fade in ignores Time.timeScale.</param>
        /// <param name="cancellationToken">External cancellation token.</param>
        public async Awaitable PlayAllAsync(float fadeInDuration = -1f, bool ignoreTimeScale = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            RandomizeLocations();

            foreach (var layer in state.StoppedLayers)
            {
                await FadeInLayer(layer, fadeInDuration, ignoreTimeScale, cancellationToken);
            }
        }

        /// <summary>
        /// Stop all currently playing layers.
        /// </summary>
        public void StopAll()
        {
            autopilot.StopAutopilot();

            foreach (var layer in state.PlayingLayers)
            {
                var audioSource = layer.Stop();
                audioSourcePool.Return(audioSource);
            }
        }

        /// <summary>
        /// Stop all currently playing layers.
        /// </summary>
        /// <param name="fadeOutDuration">Duration of volume decrease in seconds.</param>
        /// <param name="ignoreTimeScale">Duration of fade out ignores Time.timeScale.</param>
        /// <param name="cancellationToken">External cancellation token.</param>
        public async Awaitable StopAllAsync(float fadeOutDuration, bool ignoreTimeScale = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            autopilot.StopAutopilot();

            foreach (var layer in state.PlayingLayers)
            {
                /*await in bulk*/ FadeOutLayer(layer, fadeOutDuration, ignoreTimeScale, cancellationToken);
            }
        }

        /// <summary>
        /// Toggle between on and off of all layers.
        /// </summary>
        /// <param name="fadeInOut">Gradually change volume when starting/stopping sounds.</param>
        public void ToggleAll(bool fadeInOut = true)
        {
            // var stopFound = false;
            // foreach (AudioSource layer in tracks)
            // {
            //     stopFound = stopFound || layer.isPlaying;
            // }
            //
            // if (stopFound)
            // {
            //     StopAll(fadeInOut ? masterFadeDuration : 0);
            //     StopCoroutine(AutopilotCoroutine());
            // }
            // else
            // {
            //     PlayAll(fadeInOut ? masterFadeDuration : 0);
            // }
        }

        /// <summary>
        /// Gradually decrease volume of all currently playing layers.
        /// </summary>
        /// <param name="duration">Duration of fade out in seconds.</param>
        /// <param name="ignoreTimeScale">Duration of fade out ignores Time.timeScale.</param>
        /// <param name="cancellationToken">External cancellation token.</param>
        public async Awaitable FadeOutAll(float duration = -1f, bool ignoreTimeScale = true,
            CancellationToken cancellationToken = default)
        {
            duration = duration < 0f ? settings.masterFadeDuration : duration;
            await FadeMaster(false, duration, ignoreTimeScale, cancellationToken);
        }

        /// <summary>
        /// Linearly fade in a specific layer.
        /// </summary>
        /// <param name="layer">AudioSource of the new layer.</param>
        /// <param name="duration">Duration of fade in.</param>
        /// <param name="ignoreTimeScale">Duration of fade in ignores Time.timeScale.</param>
        /// <param name="cancellationToken">External cancellation token.</param>
        public async Awaitable FadeInLayer(LayerData layer, float duration, bool ignoreTimeScale,
            CancellationToken cancellationToken)
        {
            duration = duration < 0f ? settings.layerFadeInDuration : duration;
            if (settings.IsVerboseDebug)
            {
                Debug.Log($"AmbienceController: Fading in {layer.ClipName}.");
            }

            await FadeLayer(layer, true, duration, ignoreTimeScale, cancellationToken);
        }

        /// <summary>
        /// Linearly fade out a specific layer.
        /// </summary>
        /// <param name="layer">AudioSource of the layer.</param>
        /// <param name="duration">Duration of fade out.</param>
        /// <param name="ignoreTimeScale">Duration of fade out ignores Time.timeScale.</param>
        /// <param name="cancellationToken">External cancellation token.</param>
        public async Awaitable FadeOutLayer(LayerData layer, float duration, bool ignoreTimeScale,
            CancellationToken cancellationToken)
        {
            duration = duration < 0f ? settings.layerFadeOutDuration : duration;
            if (settings.IsVerboseDebug)
            {
                Debug.Log($"AmbienceController: Fading out {layer.ClipName}.");
            }

            await FadeLayer(layer, false, duration, ignoreTimeScale, cancellationToken);
        }

        /// <summary>
        /// Put the play time into a random location of each layer.
        /// This is useful to randomize layers before starting playback.
        /// </summary>
        /// <param name="includePlaying"></param>
        public void RandomizeLocations(bool includePlaying = false)
        {
            // foreach (AudioSource layer in tracks)
            // {
            //     if (includePlaying || !layer.isPlaying)
            //     {
            //         layer.time = Random.Range(0, layer.clip.length - 1);
            //     }
            // }
        }

        /// <summary>
        /// Play a number of random layers. If anything is playing already, it will be fadeout unless
        /// it's selected for a random play.
        /// </summary>
        /// <param name="layersCount">Number of layers to play.</param>
        /// <param name="fadeInDuration">Duration of linear fade in of the playback in seconds.</param>
        /// <param name="ignoreTimeScale">Duration of fade in ignores Time.timeScale.</param>
        /// <param name="cancellationToken">External cancellation token.</param>
        public async Awaitable PlayRandomLayers(int layersCount, float fadeInDuration = -1f, bool ignoreTimeScale = true,
            CancellationToken cancellationToken = default)
        {
            /*await*/
            FadeOutAll(cancellationToken: cancellationToken);

            // todo: there might be a race of those fading out and those fading in
            // resolve in favour of fade in!

            foreach (var layer in state.GetRandomLayers(layersCount))
            {
                /*await in bulk*/ FadeLayer(layer, true, fadeInDuration, ignoreTimeScale);
            }
        }

        private async Awaitable FadeMaster(bool fadeIn, float duration, bool ignoreTimeScale = true,
            CancellationToken cancellationToken = default)
        {
            await AnimateVolume(v => Volume = v, () => Volume, () => settings.ambienceMasterVolume,
                fadeIn, duration, ignoreTimeScale, cancellationToken);

            if (!fadeIn)
            {
                StopAll();

                Volume = settings.ambienceMasterVolume;
            }
        }

        private async Awaitable FadeLayer(LayerData layer, bool fadeIn, float duration, bool ignoreTimeScale,
            CancellationToken cancellationToken = default)
        {
            Assert.IsTrue(fadeIn || layer.IsPlaying, "Trying to fade out a layer that is not playing");

            cancellationToken.ThrowIfCancellationRequested();

            if (fadeIn && !layer.IsPlaying)
            {
                var audioSource = audioSourcePool.Get();
                layer.Play(audioSource, 0f);
            }

            await AnimateVolume((v) => layer.Volume = v, () => layer.Volume, () => layer.BaseVolume,
                fadeIn, duration, ignoreTimeScale, cancellationToken);

            if (!fadeIn)
            {
                var audioSource = layer.Stop();
                audioSourcePool.Return(audioSource);
            }
        }

        private async Awaitable AnimateVolume(Action<float> volumeSetter, Func<float> volumeGetter, Func<float> targetVolumeGetter,
            bool fadeIn, float duration, bool ignoreTimeScale, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentTime = ignoreTimeScale ? Time.unscaledTime : Time.time;
            var startTime = currentTime;
            var startVolume = fadeIn ? 0f : targetVolumeGetter();
            var endVolume = fadeIn ? targetVolumeGetter() : 0f;
            var progress = volumeGetter() / targetVolumeGetter();

            // take into account current volume
            startTime -= (fadeIn ? progress : (1 - progress)) * duration;
            var elapsedTime = currentTime - startTime;

            while (elapsedTime < duration)
            {
                cancellationToken.ThrowIfCancellationRequested();

                currentTime = ignoreTimeScale ? Time.unscaledTime : Time.time;
                elapsedTime = currentTime - startTime;
                var volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / duration);
                volumeSetter(volume);

                await Awaitable.NextFrameAsync(cancellationToken);
            }

            if (fadeIn)
            {
                volumeSetter(endVolume);
            }
        }
    }
}