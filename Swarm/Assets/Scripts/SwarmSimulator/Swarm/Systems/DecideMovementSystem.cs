using Swarm.Movement;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Swarm.Swarm
{
    [UpdateBefore(typeof(MoveForwardSystem))]
    public class DecideMovementSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "DecideMovement";
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((ref MoveForward moveForward, ref RandomData brownianMotion, ref HighestPotentialAgent highestPotentialAgent) =>
            {
                if (highestPotentialAgent.Potential == 0)
                {
                    float2 direction = brownianMotion.random.NextFloat2Direction();

                    moveForward.Direction = new float3(direction.x, 0, direction.y);
                }
                else
                {
                    moveForward.Direction = highestPotentialAgent.Direction;
                }

                highestPotentialAgent.Potential = 0;
                highestPotentialAgent.Direction = float3.zero;
            }).ScheduleParallel();
        }
    }
}