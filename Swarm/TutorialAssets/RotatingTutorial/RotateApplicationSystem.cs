using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DOTSTutorial.Rotating
{ 
    // [TODO] Optimyze system
    // [TODO] 
    public class RotateApplicationSystem : JobComponentSystem
    {
        private BuildPhysicsWorld buildPhysicsWorld;
        private StepPhysicsWorld stepPhysicsWorld;

        protected override void OnCreate()
        {
            buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        private struct ApplicationJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<RotateApplier> rotateApplierGroup;
            public ComponentDataFromEntity<Rotate> rotateGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                // [TOEXPLAIN] This is some tutorial bullshit that can be refactored far better
                if (rotateApplierGroup.HasComponent(triggerEvent.Entities.EntityA))
                {
                    if (rotateGroup.HasComponent(triggerEvent.Entities.EntityB))
                    {
                        Rotate rotate = rotateGroup[triggerEvent.Entities.EntityB];
                        rotate.radiansPerSecond = 25;
                        rotateGroup[triggerEvent.Entities.EntityB] = rotate;
                    }
                }

                if (rotateApplierGroup.HasComponent(triggerEvent.Entities.EntityB))
                {
                    if (rotateGroup.HasComponent(triggerEvent.Entities.EntityA))
                    {
                        Rotate rotate = rotateGroup[triggerEvent.Entities.EntityA];
                        rotate.radiansPerSecond = -25;
                        rotateGroup[triggerEvent.Entities.EntityA] = rotate;
                    }
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            ApplicationJob applicationJob = new ApplicationJob()
            {
                rotateApplierGroup = GetComponentDataFromEntity<RotateApplier>(),
                rotateGroup = GetComponentDataFromEntity<Rotate>()
            };

            //?? What is the Step physics world? what does it do? Same with the Build Physics world. And
            // What is the difference between them
            return applicationJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        }
    }
}
