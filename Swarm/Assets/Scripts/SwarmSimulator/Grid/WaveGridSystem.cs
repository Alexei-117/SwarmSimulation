using Swarm.Movement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Swarm.Grid
{
    [UpdateBefore(typeof(MoveForwardSystem))]
    public class WaveGridSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float ElapsedTime = (float)Time.ElapsedTime;

            Entities.WithAll<GridDotTag>().ForEach((ref MoveForward moveForward, in Translation t, in Wave waveData) =>
            {
                moveForward.direction = new float3(0, waveData.amplitude * math.sin((float)ElapsedTime + t.Value.x * waveData.xOffset + t.Value.z * waveData.zOffset), 0);
            }).ScheduleParallel();
        }
    }
}