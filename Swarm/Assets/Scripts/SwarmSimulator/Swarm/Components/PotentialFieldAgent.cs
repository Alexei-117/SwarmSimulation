using Unity.Entities;

namespace Swarm.Swarm
{
    public struct PotentialFieldAgent : IComponentData
    {
        public float Value;
        public float TransferRate;
    }
}