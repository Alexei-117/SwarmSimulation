using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Swarm.Swarm
{
    [UpdateBefore(typeof(ConsumptionSystem))]
    public class TransferSystem : SystemBaseManageable
    {
        private BuildPhysicsWorld buildPhysicsWorld;
        private StepPhysicsWorld stepPhysicsWorld;

        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "Transfer";

            buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        //[BurstCompile]
        struct TransferJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<PotentialFieldAgent> potentialGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                if (!potentialGroup.HasComponent(entityA)
                    || !potentialGroup.HasComponent(entityB))
                {
                    return;
                }

                PotentialFieldAgent potentialA = potentialGroup[entityA];
                PotentialFieldAgent potentialB = potentialGroup[entityB];

                if (potentialA.Value < 0.01f)
                    potentialA.Value = 0;

                if (potentialB.Value < 0.01f)
                    potentialB.Value = 0;

                if (potentialA.Value > potentialB.Value)
                {
                    float difference = (potentialA.Value - potentialB.Value) * potentialA.TransferRate;
                    potentialA.Value -= difference;
                    potentialB.Value += difference;
                }

                if (potentialB.Value > potentialA.Value)
                {
                    float difference = (potentialB.Value - potentialA.Value) * potentialA.TransferRate;
                    potentialB.Value -= difference;
                    potentialA.Value += difference;
                }

                potentialGroup[entityA] = potentialA;
                potentialGroup[entityB] = potentialB;
            }
        }

        protected override void OnUpdate()
        {
            var transferJob = new TransferJob()
            {
                potentialGroup = GetComponentDataFromEntity<PotentialFieldAgent>()
            };

            Dependency = transferJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);

            // Finishes all accumulated events before going for the next task
            Dependency.Complete();
        }
    }
}