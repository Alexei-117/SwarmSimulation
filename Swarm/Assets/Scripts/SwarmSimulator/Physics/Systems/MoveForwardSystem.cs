using Unity.Entities;
using Unity.Transforms;

namespace Swarm.Movement
{
    public class MoveForwardSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "MoveForward";
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            float timeStep = GenericInformation.TimeStep;

            Dependency = Entities.ForEach((ref Translation t, in Speed speed, in MoveForward moveForward) =>
            {
                t.Value += moveForward.Direction * speed.Value / timeStep * deltaTime;
            }).ScheduleParallel(Dependency);

            Dependency.Complete();
        }
    }
}
