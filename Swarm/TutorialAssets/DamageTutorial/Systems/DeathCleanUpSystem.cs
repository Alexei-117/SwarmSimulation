using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

namespace DOTSTutorial.Damaging
{/*
    public class DeathCleanUpSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem ecbSystem;

        protected override void OnCreate()
        {
            ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();

            Entities.WithoutBurst().WithAll<Dead>().ForEach((Entity entity) =>
            {
                ecb.DestroyEntity(entity);
            }).Run();

            return default;
        }
    }*/
}
