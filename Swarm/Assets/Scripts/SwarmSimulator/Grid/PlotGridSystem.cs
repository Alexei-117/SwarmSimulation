using Swarm.Movement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Swarm.Grid
{
    [UpdateAfter(typeof(AccumulateAgentsSystem))]
    [UpdateBefore(typeof(MoveForwardSystem))]
    public class PlotGridSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<GridDotTag>().ForEach((ref MoveForward moveForward, in Translation t, in AccumulatedAgents agents, in PlotMetadata plotMeta) =>
            {
                float yMovement = agents.Value + plotMeta.InitialHeight < t.Value.y ? -1 : 1;
                yMovement = math.abs(agents.Value + plotMeta.InitialHeight - t.Value.y) < 0.1 ? 0 : yMovement;
                moveForward.direction = new float3(0, yMovement, 0);
            }).ScheduleParallel();
        }
    }
}
