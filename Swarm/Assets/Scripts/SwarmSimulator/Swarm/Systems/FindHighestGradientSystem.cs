﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace Swarm.Swarm
{
    [UpdateAfter(typeof(ConsumptionSystem))]
    public class FindHighestGradientSystem : SystemBaseManageable
    {
        private BuildPhysicsWorld buildPhysicsWorld;
        private StepPhysicsWorld stepPhysicsWorld;

        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "FindHighestGradient";

            buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        [BurstCompile]
        struct GradientJob : ITriggerEventsJob
        {
            // Optimizing with ReadOnly
            [ReadOnly] public ComponentDataFromEntity<Translation> translationGroup;
            [ReadOnly] public ComponentDataFromEntity<PotentialFieldAgent> potentialGroup;
            public ComponentDataFromEntity<HighestPotentialAgent> highestPotentialtGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                if (!potentialGroup.HasComponent(entityA)
                    || !potentialGroup.HasComponent(entityB))
                {
                    return;
                }

                Translation translationA = translationGroup[entityA];
                Translation translationB = translationGroup[entityB];

                HighestPotentialAgent highestPotentialAgentA = highestPotentialtGroup[entityA];
                HighestPotentialAgent highestPotentialAgentB = highestPotentialtGroup[entityB];

                PotentialFieldAgent potentialA = potentialGroup[entityA];
                PotentialFieldAgent potentialB = potentialGroup[entityB];
                
                if (potentialB.Value > highestPotentialAgentA.Potential)
                {
                    highestPotentialAgentA.Potential = potentialB.Value;
                    highestPotentialAgentA.Direction = math.normalize( translationB.Value - translationA.Value );

                    highestPotentialtGroup[entityA] = highestPotentialAgentA;
                }

                if (potentialA.Value > highestPotentialAgentB.Potential)
                {
                    highestPotentialAgentB.Potential = potentialA.Value;
                    highestPotentialAgentB.Direction = math.normalize( translationA.Value - translationB.Value );

                    highestPotentialtGroup[entityB] = highestPotentialAgentB;
                }
            }
        }

        protected override void OnUpdate()
        {
            var gradientJob = new GradientJob()
            {
                translationGroup = GetComponentDataFromEntity<Translation>(true),
                potentialGroup = GetComponentDataFromEntity<PotentialFieldAgent>(true),
                highestPotentialtGroup = GetComponentDataFromEntity<HighestPotentialAgent>()
            };

            Dependency = gradientJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);

            Dependency.Complete();
        }
    }
}
