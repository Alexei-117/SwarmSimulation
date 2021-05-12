using Swarm.Scenario;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Light = Swarm.Scenario.Light;

namespace Swarm.Swarm
{
    [UpdateBefore(typeof(TransferSystem))]
    public class GatherSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "Gather";
        }

        protected override void OnUpdate()
        {
            NativeArray<float3> lightsPositions = GenericInformation.GetLightTranslations.Reinterpret<float3>();
            NativeArray<float> lightsSizes = GenericInformation.GetLights.Reinterpret<float>();
            float agentSize = GenericInformation.AgentSize * 0.5f;

            Dependency = Entities.WithAll<AgentTag>().ForEach((ref Gather gather, ref CompositeScale size, in PotentialValue potentialField, in Translation t) =>
            {
                size.Value = float4x4.Scale(0.75f, 1.0f, 0.75f);
                for (int i = 0; i < lightsPositions.Length; i++)
                {
                    if (math.distance(new float2(t.Value.x, t.Value.z), new float2(lightsPositions[i].x, lightsPositions[i].z)) <= (lightsSizes[i] + agentSize))
                    {
                        gather.Value = true;
                    }
                    if (math.distance(new float2(t.Value.x, t.Value.z), new float2(lightsPositions[i].x, lightsPositions[i].z)) <= 10.0f)
                    {
                        size.Value = float4x4.Scale(0.3f, 2.0f, 0.3f);
                    }
                }
            }).Schedule(Dependency);

            Dependency.Complete();
        }
    }
}