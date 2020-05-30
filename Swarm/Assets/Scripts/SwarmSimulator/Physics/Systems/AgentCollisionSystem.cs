using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Swarm.Movement
{
    [UpdateAfter(typeof(MoveForwardSystem))]
    [UpdateBefore(typeof(RestoreCollidedPositionSystem))]
    public class AgentCollisionSystem : SystemBaseManageable
    {
        private BuildPhysicsWorld buildPhysicsWorld;
        private StepPhysicsWorld stepPhysicsWorld;

        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "AgentCollision";

            buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        [BurstCompile]
        struct AgentCollisionJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<Translation> translationGroup;
            public ComponentDataFromEntity<Collision> collisionGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.Entities.EntityA;
                Entity entityB = triggerEvent.Entities.EntityB;

                if (!collisionGroup.Exists(entityA)
                    || !collisionGroup.Exists(entityB))
                {
                    return;
                }

                Translation translationA = translationGroup[entityA];
                Translation translationB = translationGroup[entityB];

                Collision collisionA = collisionGroup[entityA];
                Collision collisionB = collisionGroup[entityB];

                if (math.distance(translationA.Value, translationB.Value) <= collisionA.Radius)
                {
                    collisionA.Collided = true;
                    collisionB.Collided = true;

                    collisionGroup[entityA] = collisionA;
                    collisionGroup[entityB] = collisionB;
                }
            }
        }

        protected override void OnUpdate()
        {
            var agentCollisionJob = new AgentCollisionJob()
            {
                translationGroup = GetComponentDataFromEntity<Translation>(),
                collisionGroup = GetComponentDataFromEntity<Collision>()
            };

            this.Dependency = agentCollisionJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, this.Dependency);

            Dependency.Complete();
        }
    }
}