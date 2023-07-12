using System.Collections.Generic;

namespace IV.PAL
{
    public interface ILayersState
    {
        int ActiveLayersCount { get; }
        int TotalLayersCount { get; }
        IEnumerable<LayerData> PlayingLayers { get; }
        IEnumerable<LayerData> StoppedLayers { get; }
        IEnumerable<LayerData> GetRandomLayers(int targetCount);
    }
}