using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
/*
namespace DOTSTutorial.Spawning
{
    public class SpawnerSystem : JobComponentSystem
    {
        //!! Stores in a buffer all the commands that you want to run once a frame is processed (a simulation is ended).
        //!! Used to avoid overlapping processes that need to happen in a certain order, in this case after the logic
        // of 1 frame is processed and before the render
        // [TOEXPLAIN]
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        private struct SpawnerJob : IJobForEachWithEntity<Spawner, LocalToWorld>
        {
            private EntityCommandBuffer.Concurrent entityCommandBuffer;
            private Random random;
            private readonly float deltaTime;

            public SpawnerJob(EntityCommandBuffer.Concurrent entityCommandBuffer, Random random, float deltaTime)
            {
                this.entityCommandBuffer = entityCommandBuffer;
                this.random = random;
                this.deltaTime = deltaTime;
            }

            //?? why the index thingy? why forEachWithEntity?
            public void Execute(Entity entity, int index, ref Spawner spawner, [ReadOnly] ref LocalToWorld localToWorld)
            {
                spawner.secsToNextSpawn -= deltaTime;

                if (spawner.secsToNextSpawn >= 0) return;

                spawner.secsToNextSpawn += spawner.secsBetweenSpawns;

                Entity instance = entityCommandBuffer.Instantiate(index, spawner.prefab);
                entityCommandBuffer.SetComponent(index, instance, new Translation
                {
                    Value = localToWorld.Position + random.NextFloat3Direction() * random.NextFloat() * spawner.maxDistanceFromSpawner
                });
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            SpawnerJob spawnerJob = new SpawnerJob(
                //?? Why to Concurrent? What is the difference with the normal commandbuffer system?
                endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
                new Random((uint)UnityEngine.Random.Range(0, int.MaxValue)),
                Time.DeltaTime
            );

            //!! Schedule puts the job to work in the schedule system. Returns a Handle for further manipulation
            // of the Job, like finishing the job and running more things in parallel
            //?? why I am creating a second job handle? factory pattern?
            JobHandle jobHandle = spawnerJob.Schedule(this, inputDeps);
            //?? What is the producer? the thing that creates entities? Prefabs?
            endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

            return jobHandle;
        }
    }
}*/