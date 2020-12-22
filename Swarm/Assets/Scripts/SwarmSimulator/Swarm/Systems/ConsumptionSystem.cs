using Unity.Entities;

namespace Swarm.Swarm
{
    [UpdateBefore(typeof(FindHighestGradientSystem))]
    public class ConsumptionSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "Consumption";
        }

        protected override void OnUpdate()
        {
            this.Dependency = Entities.ForEach((ref PotentialFieldAgent potential, in Consumption c) =>
            {
                potential.Value -= potential.Value * c.Value;
            }).ScheduleParallel(this.Dependency);

            this.Dependency.Complete();
        }
    }
}