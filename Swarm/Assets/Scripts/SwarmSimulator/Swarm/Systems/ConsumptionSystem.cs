using Unity.Entities;

namespace Swarm.Swarm
{
    [UpdateBefore(typeof(FindHighestGradientSystem))]
    public class ConsumptionSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            this.Dependency = Entities.ForEach((ref PotentialFieldAgent potential, in Consumption c) =>
            {
                potential.Value -= potential.Value * c.Value;
            }).ScheduleParallel(this.Dependency);
        }
    }
}