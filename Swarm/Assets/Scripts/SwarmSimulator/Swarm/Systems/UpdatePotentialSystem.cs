using Unity.Entities;
using UnityEngine;

namespace Swarm.Swarm
{
    [UpdateBefore(typeof(FindHighestGradientSystem))]
    public class UpdatePotentialSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "UpdatePotential";
        }

        protected override void OnUpdate()
        {
            float gatherRate = GenericInformation.Gather;
            float consumptionRate = GenericInformation.ConsumptionRate;
            Entities.WithAll<AgentTag>().ForEach((ref PotentialValue potential, ref Gather gather) =>
            {
                // Update rule
                potential.Value -= potential.Value * consumptionRate;
                potential.Value += gather.Value ? gatherRate : 0.0f;
                //potential.Value += potential.TransferValue;

                if (potential.Value < 0.0f)
                {
                    potential.Value = 0.0f;
                }

                // Clean values for new timestep
                gather.Value = false;
            }).ScheduleParallel();

            Dependency.Complete();
        }
    }
}