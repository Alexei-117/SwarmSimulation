using Unity.Entities;

namespace Swarm.Movement
{
    public struct Collision : IComponentData
    {
        public float Radius;
        public bool Collided;
    }
}
