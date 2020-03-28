using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace DOTSTutorial.Rotating
{

    public class RotateSystem : JobComponentSystem
    {
        [BurstCompile] // Allows for burst compiler to work on this job system component
        private struct RotateJob : IJobForEach<Rotate, RotationEulerXYZ>
        {
            public float deltaTime;

            public void Execute(ref Rotate rot, ref RotationEulerXYZ euler)
            {
                euler.Value.y += rot.radiansPerSecond * deltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new RotateJob { deltaTime = Time.DeltaTime };
            return job.Schedule(this, inputDeps);
        }
    }

    // One thread system
    /*public class RotateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Rotate rot, ref RotationEulerXYZ euler) =>
            {
                euler.Value.y += rot.radiansPerSecond * Time.DeltaTime;
            });
        }
    }*/
}