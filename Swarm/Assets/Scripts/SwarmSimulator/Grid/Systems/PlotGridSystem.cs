using Swarm.Movement;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Swarm.Grid
{
    [UpdateAfter(typeof(AccumulateAgentsSystem))]
    [UpdateBefore(typeof(MoveForwardSystem))]
    public class PlotGridSystem : SystemBaseManageable
    {
        private Mesh gridMesh;

        public override void InitializeData()
        {
            gridMesh = GameObject.FindGameObjectWithTag("GridPlane").GetComponent<MeshFilter>().mesh;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "PlotGrid";
        }

        public struct ChangeGridMeshJob : IJobParallelFor
        {
            public void Execute(int index)
            {

            }
        }

        protected override void OnUpdate()
        {
            /*Entities.WithAll<GridDotTag>().ForEach((ref MoveForward moveForward, in Translation t, in AccumulatedAgents agents, in PlotMetadata plotMeta) =>
            {
                float yMovement = agents.Value + plotMeta.InitialHeight < t.Value.y ? -1 : 1;
                yMovement = math.abs(agents.Value + plotMeta.InitialHeight - t.Value.y) < 0.05 ? 0 : yMovement;
                moveForward.Direction = new float3(0, yMovement, 0);
            }).ScheduleParallel();*/
        }
    }
}
