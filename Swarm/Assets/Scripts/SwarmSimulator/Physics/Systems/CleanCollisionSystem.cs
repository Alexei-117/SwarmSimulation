using Swarm.Movement;
using Unity.Entities;

namespace Swarm.Swarm
{
    [UpdateAfter(typeof(RestoreCollidedPositionSystem))]
    public class CleanCollisionSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "CleanCollision";
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<AgentTag>().ForEach((ref Collision c) =>
            {
                c.Collided = false;
            }).ScheduleParallel();
        }
    }
}