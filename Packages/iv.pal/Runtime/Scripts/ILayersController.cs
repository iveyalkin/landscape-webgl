using System.Threading;
using UnityEngine;

namespace IV.PAL
{
    public interface ILayersController
    {
        Awaitable FadeInLayer(LayerData layer, float duration = -1f, bool ignoreTimeScale = true,
            CancellationToken cancellationToken = default);

        Awaitable FadeOutLayer(LayerData layer, float duration = -1f, bool ignoreTimeScale = true,
            CancellationToken cancellationToken = default);
    }
}