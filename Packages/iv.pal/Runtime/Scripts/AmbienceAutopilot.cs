using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace IV.PAL
{
    [Serializable]
    public struct AmbienceAutopilot : IDisposable
    {
        [SerializeField] private PALSettings settings;

        private CancellationTokenSource autopilotCancellation;

        private ILayersController controller;
        private ILayersState state;

        /// <summary>
        /// Automatically cross-fade between different layers. Autopilot uses properties of this component.
        /// </summary>
        /// <param name="controller">API to add or remove layers.</param>
        /// <param name="state">Provides currently loaded layers.</param>
        /// <param name="numInitLayers">Number of layers to play initially.</param>
        /// <param name="fadeInDuration">Duration of volume increase in seconds.</param>
        /// <param name="ignoreTimeScale">Duration of fade in ignores Time.timeScale.</param>
        /// <param name="cancellationToken">External cancellation token.</param>
        public async Awaitable StartAutopilotAsync(ILayersController controller, ILayersState state,
            int numInitLayers = -1, float fadeInDuration = -1f, bool ignoreTimeScale = true,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var prevCancellationSource = autopilotCancellation;

                autopilotCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var autopilotToken = autopilotCancellation.Token;

                // give a chance to previous autopilot to finish
                if (prevCancellationSource is { IsCancellationRequested: false })
                {
                    prevCancellationSource.Cancel();
                    prevCancellationSource.Dispose();

                    await Awaitable.NextFrameAsync(autopilotToken);
                }

                autopilotToken.ThrowIfCancellationRequested();

                this.controller = controller;
                this.state = state;

                numInitLayers = numInitLayers < 0 ? settings.numAutopilotLayers : numInitLayers;
                Assert.IsTrue(numInitLayers > 0, "Number of active autopilot layers must be positive.");

                await StartAutopilotAsync(numInitLayers, fadeInDuration, ignoreTimeScale, autopilotToken);

                StopAutopilot();
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }

        /// <summary>
        /// Stop automatically cross-fade layers.
        /// </summary>
        public void StopAutopilot()
        {
            controller = null;
            state = null;

            if (autopilotCancellation == null) return;

            autopilotCancellation.Cancel();
            autopilotCancellation.Dispose();
            autopilotCancellation = null;
        }

        private async Awaitable StartAutopilotAsync(int numInitLayers, float fadeInDuration,
            bool ignoreTimeScale, CancellationToken cancellationToken)
        {
            if (settings.IsVerboseDebug)
            {
                Debug.Log("AmbienceAutopilot: Starting autopilot.");
            }

            // If nothing is playing yet, play random layers.
            if (state.ActiveLayersCount == 0)
            {
                if (settings.IsVerboseDebug)
                {
                    Debug.Log($"AmbienceAutopilot: Nothing is playing. Starting {numInitLayers} layers.");
                }

                foreach (var layer in state.GetRandomLayers(numInitLayers))
                {
                    await controller.FadeInLayer(layer, fadeInDuration, ignoreTimeScale, cancellationToken);
                }
            }
            else if (settings.IsVerboseDebug)
            {
                Debug.Log($"AmbienceAutopilot: {state.ActiveLayersCount} layers are already playing.");
            }

            await LoopAutopilot(cancellationToken);
        }

        private async Awaitable LoopAutopilot(CancellationToken cancellationToken)
        {
            var timeUntilAction = settings.avgFadeTimeout +
                                  Random.Range(-settings.fadeTimeoutVariance, settings.fadeTimeoutVariance);

            while (settings.changeLayersAutomatically)
            {
                await Awaitable.WaitForSecondsAsync(timeUntilAction, cancellationToken);

                // Decide type of action.
                // TODO add randomness in random (at least set seed)
                var activeLayersCount = state.ActiveLayersCount;
                var minLayers = Mathf.Max(0, activeLayersCount - settings.layerNumberVariance);
                var maxLayers = Mathf.Min(state.TotalLayersCount, activeLayersCount + settings.layerNumberVariance);
                var nextCount = Random.Range(minLayers, maxLayers + 1); // +1 because Random.Range is exclusive

                if (settings.IsVerboseDebug)
                {
                    Debug.Log(
                        $"AmbienceAutopilot: Playing: {activeLayersCount}, next count: {nextCount}.");
                }

                if (nextCount > activeLayersCount)
                {
                    await FadeInLayer(cancellationToken);
                }
                else if (nextCount < activeLayersCount)
                {
                    await FadeOutLayer(cancellationToken);
                }
            }
        }

        /// Fade out a playing layer.
        private async Awaitable FadeOutLayer(CancellationToken cancellationToken = default)
        {
            if (TryFetchRandomLayer(out var layer, state.PlayingLayers))
            {
                await controller.FadeOutLayer(layer, cancellationToken: cancellationToken);
            }
        }

        /// Fade in an idle track.
        /// Start one of the first tracks in queue.
        private async Awaitable FadeInLayer(CancellationToken cancellationToken = default)
        {
            if (TryFetchRandomLayer(out var layer, state.StoppedLayers))
            {
                await controller.FadeInLayer(layer, cancellationToken: cancellationToken);
            }
        }

        // todo: get rid of linq
        private bool TryFetchRandomLayer(out LayerData layerData, IEnumerable<LayerData> layers)
        {
            if (!layers.Any())
            {
                layerData = null;
                return false;
            }

            var randomIndex = Random.Range(0, layers.Count());
            var skipCount = Mathf.Max(0, randomIndex - 1);
            layerData = layers.Skip(skipCount).First();
            return true;
        }

        void IDisposable.Dispose()
        {
            StopAutopilot();
        }
    }
}