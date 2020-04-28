using System.Collections;
using Swarm.Swarm;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Swarm.Grid
{
    public class AccumulateAgentsSystem : SystemBase
    {
        [BurstCompile]
        struct AccumulateAgentsJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<float3> agentPositions;

            [ReadOnly]
            public float4 gridDotDetectionBoundary;

            public NativeArray<bool> nearTheGridDot;

            public void Execute(int index)
            {
                if ( gridDotDetectionBoundary.x <= agentPositions[index].x
                    && gridDotDetectionBoundary.y > agentPositions[index].x
                    && gridDotDetectionBoundary.z <= agentPositions[index].z
                    && gridDotDetectionBoundary.w > agentPositions[index].z)
                {
                    nearTheGridDot[index] = true;
                }
            }
        }

        protected override void OnUpdate()
        {
            // Initialise variables
            NativeArray<float3> agentsPositions = GetEntityQuery(ComponentType.ReadOnly<AgentTag>(), ComponentType.ReadOnly<Translation>())
                                                  .ToComponentDataArray<Translation>(Allocator.TempJob)
                                                  .Reinterpret<float3>();

            Entities.WithoutBurst().WithAll<GridDotTag>().ForEach((ref AccumulatedAgents agents, in PlotMetadata dotData) =>
            {

                NativeArray<bool> nearTheGridDot = new NativeArray<bool>(agentsPositions.Length, Allocator.TempJob);
                int totalAgents = 0;

                // Create and run job
                AccumulateAgentsJob job = new AccumulateAgentsJob()
                {
                    agentPositions = agentsPositions,
                    gridDotDetectionBoundary = dotData.dotBoundaries,
                    nearTheGridDot = nearTheGridDot
                };

                JobHandle jobHandle = job.Schedule(agentsPositions.Length, 64);

                jobHandle.Complete();

                // Retrieve data
                for (int i = 0; i < nearTheGridDot.Length; i++)
                {
                    if (nearTheGridDot[i])
                    {
                        totalAgents++;
                    }
                }

                agents.Value = totalAgents;

                // Dispose native arrays
                nearTheGridDot.Dispose();
            }).Run();

            agentsPositions.Dispose();
        }
    }
}
