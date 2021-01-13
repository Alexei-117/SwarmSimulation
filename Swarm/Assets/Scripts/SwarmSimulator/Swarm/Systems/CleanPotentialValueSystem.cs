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
            Dependency = Entities.ForEach((ref MoveForward moveForward, ref HighestPotentialAgent highestPotentialAgent) =>
            {
                highestPotentialAgent.Potential = 0;
                highestPotentialAgent.Direction = float3.zero;
            }).ScheduleParallel(Dependency);

            Dependency.Complete();
        }
    }
}