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
    public class TransferSystem : SystemBase
    {

        private BuildPhysicsWorld buildPhysicsWorld;
        private StepPhysicsWorld stepPhysicsWorld;

        protected override void OnCreate()
        {
            buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

            Enabled = false;
        }

        struct TransferJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<PotentialFieldAgent> potentialGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                if (!potentialGroup.HasComponent(triggerEvent.Entities.EntityA)
                    || !potentialGroup.HasComponent(triggerEvent.Entities.EntityB))
                {
                    return;
                }

                PotentialFieldAgent potential = potentialGroup[triggerEvent.Entities.EntityA];
                PotentialFieldAgent potentialB = potentialGroup[triggerEvent.Entities.EntityB];

                float difference = (potential.Value - potentialB.Value) * potential.TransferRate;
                potential.Value += difference;
                potentialB.Value -= difference;

                potentialGroup[triggerEvent.Entities.EntityA] = potential;
                potentialGroup[triggerEvent.Entities.EntityB] = potentialB;
            }
        }

        protected override void OnUpdate()
        {
            var TransferJob = new TransferJob()
            {
                potentialGroup = GetComponentDataFromEntity<PotentialFieldAgent>()
            };

            this.Dependency = TransferJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, this.Dependency);
        }
    }
}