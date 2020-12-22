using Swarm.Movement;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Swarm.Swarm
{
    [UpdateBefore(typeof(MoveForwardSystem))]
    public class CleanPotentialValueSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "CleanPotentialValue";
        }

        protected override void OnUpdate()
        {
            this.Dependency = Entities.ForEach((ref MoveForward moveForward, ref HighestPotentialAgent highestPotentialAgent) =>
            {
                highestPotentialAgent.Potential = 0;
            }).ScheduleParallel(this.Dependency);

            this.Dependency.Complete();
        }
    }
}