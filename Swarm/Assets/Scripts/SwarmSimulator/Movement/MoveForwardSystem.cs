using Unity.Entities;
using Unity.Transforms;

namespace Swarm.Movement
{
    public class MoveForwardSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Translation t, in Speed speed, in MoveForward moveForward) =>
            {
                t.Value += moveForward.direction * speed.Value * deltaTime;
            }).ScheduleParallel();
        }
    }
}
