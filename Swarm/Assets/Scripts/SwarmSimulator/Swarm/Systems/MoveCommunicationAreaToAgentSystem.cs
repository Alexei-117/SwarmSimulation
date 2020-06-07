using Swarm;
using Swarm.Movement;
using Swarm.Swarm;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MoveForwardSystem))]
public class MoveCommunicationAreaToAgentSystem : SystemBaseManageable
{
    protected override void OnCreate()
    {
        base.OnCreate();
        Name = "MoveCommunicationAreaToAgent";
    }

    [BurstCompile]
    struct MoveCommunicationAreaToAgentJob : IJobForEachWithEntity<CommunicationAreaTag, Translation>
    {
        [ReadOnly]
        public NativeArray<float3> agents;

        public void Execute(Entity entity, int index, [ReadOnly] ref CommunicationAreaTag tag, ref Translation t)
        {
            t.Value = agents[index];
        }
    }

    protected override void OnUpdate()
    {
        var agentTranslations = GetEntityQuery(ComponentType.ReadOnly<AgentTag>(), ComponentType.ReadOnly<Translation>())
                .ToComponentDataArray<Translation>(Allocator.TempJob);

        var job = new MoveCommunicationAreaToAgentJob()
        {
            agents = agentTranslations.Reinterpret<float3>()
        };

        JobHandle handle = job.Schedule(this, Dependency);

        handle.Complete();

        agentTranslations.Dispose();
    }
}
