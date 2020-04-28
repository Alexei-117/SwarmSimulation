using Unity.Entities;
using Unity.Jobs;

namespace DOTSTutorial.Damaging
{/*
    [UpdateBefore(typeof(DeathCleanUpSystem))]
    public class ResolveDamageSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem ecbSystem;

        protected override void OnCreate()
        {
            ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();

            //?? How to do this with burst? What is the Dynamic buffer?
            Entities.WithoutBurst().WithNone<Dead>().ForEach((Entity entity, ref DynamicBuffer<Damage> damageBuffer, ref Health health) =>
            {
                foreach(var damage in damageBuffer)
                {
                    health.Value -= damage.Value;

                    if (health.Value <= 0)
                    {
                        health.Value = 0;
                        ecb.AddComponent<Dead>(entity);
                        break;
                    }
                }

                damageBuffer.Clear();
            }).Run();

            //!! It's better practice to return default if we don't care about input dependencies because we don't use it
            return default;
        }
    }*/
}