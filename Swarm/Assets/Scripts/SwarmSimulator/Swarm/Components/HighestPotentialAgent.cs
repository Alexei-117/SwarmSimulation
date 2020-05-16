using Unity.Entities;
using Unity.Mathematics;

namespace Swarm.Swarm
{
    public struct HighestPotentialAgent : IComponentData
    {
        public float Potential;
        public float3 Direction;
    }
}
