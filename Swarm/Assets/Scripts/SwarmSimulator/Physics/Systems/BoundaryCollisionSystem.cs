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
            Dependency = Entities.WithAll<AgentTag>().ForEach((ref Collision c, in Translation t, in HorizontalLimits limits) =>
            {
                if (t.Value.x < limits.x1)
                {
                    c.Collided = true;
                    c.CollisionDirection = new float3(limits.x1, 0.0f, 0.0f);
                }

                if (t.Value.x > limits.x2)
                {
                    c.Collided = true;
                    c.CollisionDirection = new float3(limits.x2, 0.0f, 0.0f);
                }

                if (t.Value.z < limits.z1)
                {
                    c.Collided = true;
                    c.CollisionDirection = new float3(0.0f, 0.0f, limits.z1);
                }

                if (t.Value.z > limits.z2)
                {
                    c.Collided = true;
                    c.CollisionDirection = new float3(0.0f, 0.0f, limits.z2);
                }
            }).ScheduleParallel(Dependency);

            Dependency.Complete();
        }
    }
}