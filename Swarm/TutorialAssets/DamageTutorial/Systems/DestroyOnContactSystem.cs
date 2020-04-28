using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DOTSTutorial.Damaging
{/*
    public class DestroyOnContactSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem ecbSystem;
        private BuildPhysicsWorld buildPhysicsWorld;
        private StepPhysicsWorld stepPhysicsWorld;

        private struct DestroyTriggerJob : ITriggerEventsJob
        {
            public EntityCommandBuffer ecb;
            [ReadOnly] public ComponentDataFromEntity<DestroyOnContact> destroyOnContactGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                if (destroyOnContactGroup.HasComponent(triggerEvent.Entities.EntityA))
                {
                    ecb.DestroyEntity(triggerEvent.Entities.EntityA);
                }

                if (destroyOnContactGroup.HasComponent(triggerEvent.Entities.EntityB))
                {
                    ecb.DestroyEntity(triggerEvent.Entities.EntityB);
                }
            }
        }

        private struct DestroyCollisionJob : ICollisionEventsJob
        {
            public EntityCommandBuffer ecb;
            [ReadOnly] public ComponentDataFromEntity<DestroyOnContact> destroyOnContactGroup;

            public void Execute(CollisionEvent collisionEvent)
            {
                if (destroyOnContactGroup.HasComponent(collisionEvent.Entities.EntityA))
                {
                    ecb.DestroyEntity(collisionEvent.Entities.EntityA);
                }

                if (destroyOnContactGroup.HasComponent(collisionEvent.Entities.EntityB))
                {
                    ecb.DestroyEntity(collisionEvent.Entities.EntityB);
                }
            }
        }

        protected override void OnCreate()
        {
            ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var destroyOnContactGroup = GetComponentDataFromEntity<DestroyOnContact>(true);
            EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();

            DestroyTriggerJob destroyTriggerJob = new DestroyTriggerJob
            {
                ecb = ecb,
                destroyOnContactGroup = destroyOnContactGroup
            };

            DestroyCollisionJob destroyCollisionJob = new DestroyCollisionJob
            {
                ecb = ecb,
                destroyOnContactGroup = destroyOnContactGroup
            };

            destroyTriggerJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();
            destroyCollisionJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();

            return inputDeps;
        }
    }*/
}