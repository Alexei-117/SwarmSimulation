using Unity.Entities;
using Unity.Mathematics;

namespace Swarm.Grid
{
    public struct PlotMetadata : IComponentData
    {
        public float InitialHeight;
        public float4 dotBoundaries;
    }
}
