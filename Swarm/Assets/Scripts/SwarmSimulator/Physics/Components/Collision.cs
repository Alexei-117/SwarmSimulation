using Unity.Entities;
using Unity.Mathematics;

namespace Swarm.Movement
{
    public struct Collision : IComponentData
    {
        public float Radius;
        public bool Collided;
        public float3 CollisionDirection;
    }
}
