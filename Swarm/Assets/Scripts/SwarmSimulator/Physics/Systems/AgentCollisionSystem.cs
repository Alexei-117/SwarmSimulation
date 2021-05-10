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
            [ReadOnly] public ComponentDataFromEntity<Translation> translationGroup;
            public ComponentDataFromEntity<Collision> collisionGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                if (!collisionGroup.HasComponent(entityA)
                    || !collisionGroup.HasComponent(entityB))
                {
                    return;
                }

                Translation translationA = translationGroup[entityA];
                Translation translationB = translationGroup[entityB];

                Collision collisionA = collisionGroup[entityA];
                Collision collisionB = collisionGroup[entityB];
                
                /// Inside the trigger collision (Communication area) there is collision area, which is the collision distance to another agent + radius of the other agent.
                if (math.distance(translationA.Value, translationB.Value) <= collisionA.Radius + collisionB.Radius)
                {
                    collisionA.Collided = true;
                    collisionB.Collided = true;

                    collisionA.CollisionDirection = translationB.Value - translationA.Value;
                    collisionB.CollisionDirection = translationA.Value - translationB.Value;

                    collisionGroup[entityA] = collisionA;
                    collisionGroup[entityB] = collisionB;
                }
            }
        }

        protected override void OnUpdate()
        {
            var agentCollisionJob = new AgentCollisionJob()
            {
                translationGroup = GetComponentDataFromEntity<Translation>(true),
                collisionGroup = GetComponentDataFromEntity<Collision>()
            };

            Dependency = agentCollisionJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);

            Dependency.Complete();
        }
    }
}