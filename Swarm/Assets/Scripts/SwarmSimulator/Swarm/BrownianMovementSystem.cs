using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Swarm.Movement;

namespace Swarm.Swarm
{
    [UpdateBefore(typeof(MoveForwardSystem))]
    public class BrownianMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref MoveForward moveForward, ref RandomData brownianMotion) =>
            {
                float2 direction = brownianMotion.random.NextFloat2Direction();

                moveForward.direction = new float3(direction.x, 0, direction.y);
            }).ScheduleParallel();
        }
    }
}