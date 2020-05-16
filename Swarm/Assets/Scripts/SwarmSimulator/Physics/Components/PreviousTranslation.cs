using Unity.Entities;
using Unity.Mathematics;

namespace Swarm.Movement
{
    public struct PreviousTranslation : IComponentData
    {
        public float3 Value;
    }
}