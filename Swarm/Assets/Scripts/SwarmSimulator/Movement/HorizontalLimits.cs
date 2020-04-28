using Unity.Entities;

namespace Swarm.Movement
{
    public struct HorizontalLimits : IComponentData
    {
        public float x1;
        public float x2;
        public float z1;
        public float z2;
    }
}