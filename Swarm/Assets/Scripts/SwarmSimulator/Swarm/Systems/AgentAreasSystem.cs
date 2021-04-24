using Swarm;
using Swarm.Movement;
using Swarm.Swarm;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(RestoreCollidedPositionSystem))]
public class AgentAreasSystem : SystemBaseManageable
{
    protected override void OnCreate()
    {
        base.OnCreate();
        Name = "AgentAreas";
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
    }

    protected override void OnUpdate()
    {
        var agentsTranslations = GetEntityQuery(ComponentType.ReadOnly<AgentTag>(), ComponentType.ReadOnly<Translation>()).ToComponentDataArray<Translation>(Allocator.TempJob);

        Dependency = Entities.ForEach((ref Translation t, in CommunicationAreaTag c) =>
        {
            t.Value = agentsTranslations[c.AgentIndex].Value + new float3(0.0f, 0.01f, 0.0f);
        }).Schedule(Dependency);

        Dependency = Entities.ForEach((ref Translation t, in CollisionAreaTag c) =>
        {
            t.Value = agentsTranslations[c.AgentIndex].Value + new float3(0.0f, 0.02f, 0.0f);
        }).Schedule(Dependency);

        Dependency.Complete();

        agentsTranslations.Dispose();
    }
}
