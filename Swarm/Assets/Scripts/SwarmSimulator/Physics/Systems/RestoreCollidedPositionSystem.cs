using Swarm.Swarm;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Swarm.Movement
{
    [UpdateAfter(typeof(MoveForwardSystem))]
    public class RestoreCollidedPositionSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "RestoreCollidedPosition";
        }

        protected override void OnUpdate()
        {
            Dependency = Entities.WithAll<AgentTag>().ForEach((ref Translation t, ref Collision c, in PreviousTranslation pt) =>
            {
                if (c.Collided)
                {
                    t.Value -= math.normalize(c.CollisionDirection) * 0.1f;
                    c.Collided = false;
                }
            }).ScheduleParallel(Dependency);

            Dependency.Complete();
        }
    }
}
