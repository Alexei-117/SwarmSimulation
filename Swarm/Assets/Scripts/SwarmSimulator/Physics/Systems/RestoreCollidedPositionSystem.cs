using Swarm.Swarm;
using Unity.Entities;
using Unity.Transforms;

namespace Swarm.Movement
{
    [UpdateAfter(typeof(MoveForwardSystem))]
    public class RestoreCollidedPositionSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.WithAll<AgentTag>().ForEach((ref Translation t, in Collision c, in PreviousTranslation pt) =>
            {
                if (c.Collided)
                {
                    t.Value = pt.Value;
                }
            }).ScheduleParallel();
        }
    }
}
