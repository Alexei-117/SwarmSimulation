using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Swarm.Movement
{
    [UpdateAfter(typeof(MoveForwardSystem))]
    public class BoundaryCorrectorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            
            Entities.ForEach((ref Translation t, in HorizontalLimits limits) =>
            {
                t.Value = new float3(
                    math.clamp(t.Value.x, limits.x1, limits.x2),
                    0,
                    math.clamp(t.Value.z, limits.z1, limits.z2)
                    );
            }).ScheduleParallel();
        }
    }
}