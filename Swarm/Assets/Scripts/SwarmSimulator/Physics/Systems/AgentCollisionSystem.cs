using Swarm.Swarm;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Swarm.Movement
{
    [UpdateAfter(typeof(MoveForwardSystem))]
    [UpdateBefore(typeof(RestoreCollidedPositionSystem))]
    public class AgentCollisionSystem : SystemBase
    {
        [BurstCompile]
        struct AgentCollisionJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<float3> agentPositions;

            [ReadOnly]
            public float3 agentPosition;

            [ReadOnly]
            public float collisionSize;

            public NativeArray<bool> collided;

            public void Execute(int index)
            {
                if (!agentPosition.Equals(agentPositions[index])
                    && math.distance(agentPosition, agentPositions[index]) <= collisionSize)
                {
                    collided[index] = true;
                }
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            // Initialise variables
            NativeArray<float3> agentsPositions = GetEntityQuery(ComponentType.ReadOnly<AgentTag>(), ComponentType.ReadOnly<Translation>())
                                                  .ToComponentDataArray<Translation>(Allocator.TempJob)
                                                  .Reinterpret<float3>();

            Entities.WithoutBurst().ForEach((ref Collision c, in Translation t) =>
            {
                NativeArray<bool> anyCollision = new NativeArray<bool>(agentsPositions.Length, Allocator.TempJob);

                // Create and run job
                AgentCollisionJob job = new AgentCollisionJob()
                {
                    agentPositions = agentsPositions,
                    agentPosition = t.Value,
                    collisionSize = c.Radius,
                    collided = anyCollision
                };

                JobHandle jobHandle = job.Schedule(agentsPositions.Length, 64);

                jobHandle.Complete();

                for(int i = 0; i < anyCollision.Length; i++)
                {
                    if (anyCollision[i])
                    {
                        c.Collided = true;
                    }
                }

                anyCollision.Dispose();
            }).Run();

            agentsPositions.Dispose();
        }
    }
}