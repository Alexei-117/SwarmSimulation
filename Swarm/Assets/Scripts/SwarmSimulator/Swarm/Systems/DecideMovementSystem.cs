using Swarm.Movement;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Swarm.Swarm
{
    [UpdateBefore(typeof(CleanPotentialValueSystem))]
    public class DecideMovementSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "DecideMovement";
        }

        protected override void OnUpdate()
        {
            float aggregationThreshold = GenericInformation.agreggationThreshhold;
            Dependency = Entities.ForEach((ref MoveForward moveForward, ref RandomData brownianMotion, in HighestPotentialAgent highestPotentialAgent, in PotentialFieldAgent potentialFieldAgent) =>
            {
                if (highestPotentialAgent.Potential == 0)
                {
                    float2 direction = brownianMotion.random.NextFloat2Direction();

                    moveForward.Direction = new float3(direction.x, 0, direction.y);
                }
                else
                {
                    float probabilityGoingForHighest = Mathf.Max( Mathf.Min( (potentialFieldAgent.Value - aggregationThreshold ) * 0.001f, 0.75f) , 0.0f);
                    if (probabilityGoingForHighest >= brownianMotion.random.NextFloat(0.0f, 1.0f))
                    {
                        moveForward.Direction = highestPotentialAgent.Direction;
                    } else {
                        float2 direction = brownianMotion.random.NextFloat2Direction();

                        moveForward.Direction = new float3(direction.x, 0, direction.y);
                    }
                }

                
            }).ScheduleParallel(Dependency);

            Dependency.Complete();
        }
    }
}