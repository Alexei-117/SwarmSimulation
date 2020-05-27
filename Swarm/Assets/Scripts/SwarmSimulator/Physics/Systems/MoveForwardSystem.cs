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
            Entities.ForEach((ref Translation t, ref PreviousTranslation pt, in Speed speed, in MoveForward moveForward) =>
            {
                pt.Value = t.Value;
                t.Value += moveForward.Direction * speed.Value * deltaTime;
            }).ScheduleParallel();
        }
    }
}
