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

            Dependency = Entities.WithAll<AgentTag>().ForEach((ref PotentialFieldAgent potentialField, in Translation t, in Gather gather) =>
            {
                for (int i = 0; i < lightsPositions.Length; i++)
                {
                    if (math.distance(new float2(t.Value.x, t.Value.z), new float2(lightsPositions[i].x, lightsPositions[i].z)) <= lightsSizes[i])
                    {
                        potentialField.Value += gather.Value;
                    }
                }
            }).Schedule(Dependency);

            Dependency.Complete();
        }
    }
}