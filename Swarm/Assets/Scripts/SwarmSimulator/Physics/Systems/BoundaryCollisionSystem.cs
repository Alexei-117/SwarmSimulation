﻿using Swarm.Swarm;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Swarm.Movement
{
    [UpdateAfter(typeof(MoveForwardSystem))]
    [UpdateBefore(typeof(RestoreCollidedPositionSystem))]
    public class BoundaryCollisionSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "BoundaryCollision";
        }

        protected override void OnUpdate()
        {
            float layoutWidth = GenericInformation.LayoutWidth;
            float layoutHeight = GenericInformation.LayoutHeight;
            float agentSize = GenericInformation.AgentSize;

            Dependency = Entities.WithAll<AgentTag>().ForEach((ref Collision c, in Translation t) =>
            {
                if (t.Value.x < agentSize * 0.5f)
                {
                    c.Collided = true;
                    c.CollisionDirection = new float3(-1.0f, 0.0f, 0.0f);
                }

                if (t.Value.x > layoutWidth - agentSize * 0.5f)
                {
                    c.Collided = true;
                    c.CollisionDirection = new float3(1.0f, 0.0f, 0.0f);
                }

                if (t.Value.z < agentSize * 0.5f)
                {
                    c.Collided = true;
                    c.CollisionDirection = new float3(0.0f, 0.0f, -1.0f);
                }

                if (t.Value.z > layoutHeight - agentSize * 0.5f)
                {
                    c.Collided = true;
                    c.CollisionDirection = new float3(0.0f, 0.0f, 1.0f);
                }
            }).ScheduleParallel(Dependency);

            Dependency.Complete();
        }
    }
}