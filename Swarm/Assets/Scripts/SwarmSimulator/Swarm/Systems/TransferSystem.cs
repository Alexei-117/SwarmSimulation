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

        struct TransferJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<PotentialFieldAgent> potentialGroup;

            public void Execute(TriggerEvent triggerEvent)
            {

                Entity entityA = triggerEvent.Entities.EntityA;
                Entity entityB = triggerEvent.Entities.EntityB;

                if (!potentialGroup.Exists(entityA)
                    || !potentialGroup.Exists(entityB))
                {
                    return;
                }

                PotentialFieldAgent potentialA = potentialGroup[entityA];
                PotentialFieldAgent potentialB = potentialGroup[entityB];

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

                if (potentialA.Value < 0)
                    potentialA.Value = 0;

                if (potentialB.Value < 0)
                    potentialB.Value = 0;

                potentialGroup[entityA] = potentialA;
                potentialGroup[entityB] = potentialB;

                //Debug.Log("Post: " + potentialA.Value + " - " + potentialB.Value + ". Difference: " + difference);
            }
        }

        protected override void OnUpdate()
        {
            var transferJob = new TransferJob()
            {
                potentialGroup = GetComponentDataFromEntity<PotentialFieldAgent>()
            };

            this.Dependency = transferJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, this.Dependency);

            // Finishes all accumulated events before going for the next task
            Dependency.Complete();
        }
    }
}

/* 
 Así que, el sistema de eventos:

-> Cojo la posición de un objeto, y guardo el entity en un array de una matriz de arrays.
-> Comparo mis posiciones solamente con las posiciones adyacentes que contienen ese array.
-> Cada array cubre de área un diámetro entero de uno de los robots desde su punto central.

-> Se puede hacer con hasheo antes (?) A lo mejor a eso iba la ecuación?
-> Se puede hacer un NativeHashMaps y asignar un int / hash a cada array?

[][][B][][]
[][C][X-Y-Z][A][]
[][][D][][]
[][][][][]
 
 */