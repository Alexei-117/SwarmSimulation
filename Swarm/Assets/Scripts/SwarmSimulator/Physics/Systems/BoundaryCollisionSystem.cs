using Swarm.Swarm;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Swarm.Movement
{
    [UpdateAfter(typeof(MoveForwardSystem))]
    [UpdateBefore(typeof(RestoreCollidedPositionSystem))]
    public class BoundaryCollisionSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "BoundaryCollision";
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<AgentTag>().ForEach((ref Collision c, in Translation t, in HorizontalLimits limits) =>
            {
                if (t.Value.x < limits.x1
                    || t.Value.x > limits.x2
                    || t.Value.z < limits.z1
                    || t.Value.z > limits.z2)
                {
                    c.Collided = true;
                }
            }).ScheduleParallel();
        }
    }
}