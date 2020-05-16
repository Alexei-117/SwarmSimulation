using Unity.Entities;
using Unity.Mathematics;

namespace Swarm.Movement
{
    public struct MoveForward : IComponentData
    {
        public float3 Direction;
    }
}
