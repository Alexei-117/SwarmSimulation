using Swarm.Scenario;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Swarm.Swarm
{
    public class GatherSystem : SystemBase
    {
        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            // Initialise variables
            NativeArray<float3> lightsPositions = GetEntityQuery(ComponentType.ReadOnly<Light>(), ComponentType.ReadOnly<Translation>())
                                                        .ToComponentDataArray<Translation>(Allocator.TempJob)
                                                        .Reinterpret<float3>();

            NativeArray<float> lightsSizes = GetEntityQuery(ComponentType.ReadOnly<Light>(), ComponentType.ReadOnly<Translation>())
                                                        .ToComponentDataArray<Light>(Allocator.TempJob)
                                                        .Reinterpret<float>();

            Entities.WithoutBurst().WithAll<AgentTag>().ForEach((ref PotentialFieldAgent potentialField, in Gather gather, in Translation t) =>
            {
                // Retrieve data
                for (int i = 0; i < lightsPositions.Length; i++)
                {
                    if (math.distance(new float2(t.Value.x, t.Value.z), new float2(lightsPositions[i].x, lightsPositions[i].z)) <= lightsSizes[i])
                    {
                        potentialField.Value += gather.Value * Time.DeltaTime;
                    }
                }
            }).Run();

            // Dispose native arrays
            lightsPositions.Dispose();
            lightsSizes.Dispose();
        }
    }
}