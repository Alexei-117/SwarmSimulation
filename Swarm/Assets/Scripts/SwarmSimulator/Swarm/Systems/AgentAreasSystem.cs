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
public class AgentAreasSystem : SystemBaseManageable
{
    protected override void OnCreate()
    {
        base.OnCreate();
        Name = "AgentAreas";
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

    [BurstCompile]
    struct MoveCollisionAreaToAgentJob : IJobForEachWithEntity<CollisionAreaTag, Translation>
    {
        [ReadOnly]
        public NativeArray<float3> agents;

        public void Execute(Entity entity, int index, [ReadOnly] ref CollisionAreaTag tag, ref Translation t)
        {
            t.Value = agents[index] + new float3(0.0f, 0.01f, 0.0f);
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

        var secondJob = new MoveCollisionAreaToAgentJob()
        {
            agents = agentTranslations.Reinterpret<float3>()
        };

        JobHandle handle = job.Schedule(this, Dependency);

        handle.Complete();

        JobHandle secondHandle = secondJob.Schedule(this, Dependency);

        secondHandle.Complete();

        agentTranslations.Dispose();
    }
}
