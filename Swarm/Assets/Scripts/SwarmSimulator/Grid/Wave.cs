using Unity.Entities;

namespace Swarm.Grid
{
    public struct Wave : IComponentData
    {
        public float amplitude;
        public float xOffset;
        public float zOffset;
    }
}