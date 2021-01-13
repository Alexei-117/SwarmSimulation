using Swarm.Movement;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Swarm.Grid
{
    [UpdateAfter(typeof(AccumulateAgentsSystem))]
    public class PlotGridSystem : SystemBaseManageable
    {
        private Mesh gridMesh;
        private NativeArray<Vector3> meshVertices;

        public override void InitializeData()
        {
            gridMesh = GameObject.FindGameObjectWithTag("GridPlane").GetComponent<MeshFilter>().mesh;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "PlotGrid";
        }

        protected override void OnUpdate()
        {
            Dependency = Entities.WithAll<GridDotTag>().ForEach((ref MoveForward moveForward, in Translation t, in AccumulatedAgents agents, in PlotMetadata plotMeta) =>
            {
                float yMovement = agents.Value + plotMeta.InitialHeight < t.Value.y ? -1 : 1;
                yMovement = math.abs(agents.Value + plotMeta.InitialHeight - t.Value.y) < 0.05 ? 0 : yMovement;
                moveForward.Direction = new float3(0, yMovement, 0);
            }).ScheduleParallel(Dependency);

            Dependency.Complete();

            meshVertices = new NativeArray<Vector3>(gridMesh.vertexCount, Allocator.TempJob);
            Entities.WithoutBurst().ForEach((in GridDotTag gridDot, in Translation t) =>
            {
                meshVertices[gridDot.Index] = t.Value;
            }).Run();

            gridMesh.vertices = meshVertices.ToArray();
            meshVertices.Dispose();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
