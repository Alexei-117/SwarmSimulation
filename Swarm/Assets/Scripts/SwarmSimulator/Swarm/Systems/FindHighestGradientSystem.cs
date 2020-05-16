using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Swarm.Swarm
{
    [UpdateBefore(typeof(DecideMovementSystem))]
    public class FindHighestGradientSystem : SystemBase
    {
        private BuildPhysicsWorld buildPhysicsWorld;
        private StepPhysicsWorld stepPhysicsWorld;

        protected override void OnCreate()
        {
            buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

            Enabled = false;
        }

        [BurstCompile]
        struct GradientJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<Translation> translationGroup;
            public ComponentDataFromEntity<PotentialFieldAgent> potentialGroup;
            public ComponentDataFromEntity<HighestPotentialAgent> highestPotentialtGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                Translation translationA = translationGroup[triggerEvent.Entities.EntityA];
                HighestPotentialAgent highestPotentialAgentA = highestPotentialtGroup[triggerEvent.Entities.EntityA];

                Translation translationB = translationGroup[triggerEvent.Entities.EntityB];
                PotentialFieldAgent potentialB = potentialGroup[triggerEvent.Entities.EntityB];

                if (potentialB.Value > highestPotentialAgentA.Potential)
                {
                    highestPotentialAgentA.Potential = potentialB.Value;
                    highestPotentialAgentA.Direction = translationB.Value - translationA.Value;

                    highestPotentialtGroup[triggerEvent.Entities.EntityA] = highestPotentialAgentA;
                }
            }
        }

        protected override void OnUpdate()
        {
            var gradientJob = new GradientJob()
            {
                translationGroup = GetComponentDataFromEntity<Translation>(),
                potentialGroup = GetComponentDataFromEntity<PotentialFieldAgent>(),
                highestPotentialtGroup = GetComponentDataFromEntity<HighestPotentialAgent>()
            };

            this.Dependency = gradientJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, this.Dependency);
        }
    }
}
