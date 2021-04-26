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
        private Mesh gridMeshInverted;
        private NativeArray<Vector3> meshVertices;
        private NativeArray<Color> meshColors;
        private NativeArray<Vector3> meshVerticesInverted;

        public override void InitializeData()
        {
            gridMesh = GameObject.Find("GridPlane").GetComponent<MeshFilter>().mesh;
            gridMeshInverted = GameObject.Find("GridPlaneInverted").GetComponent<MeshFilter>().mesh;
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
            meshVerticesInverted = new NativeArray<Vector3>(gridMeshInverted.vertexCount, Allocator.TempJob);
            meshColors = new NativeArray<Color>(gridMesh.vertexCount, Allocator.TempJob);
            Entities.WithoutBurst().ForEach((in GridDotTag gridDot, in Translation t) =>
            {
                meshVertices[gridDot.Index] = t.Value;
                meshVerticesInverted[gridDot.Index] = new float3(t.Value.x, t.Value.y - 0.001f, t.Value.z);
                
                /// Paint dot to reflect number of agents. From Green (0 agents) --> To Red (16 agents), in HSV for easier computation
                meshColors[gridDot.Index] = Color.HSVToRGB( ((t.Value.y - 10.0f )/ 16.0f) / 10.0f + 0.2f, 0.5f, 0.5f);
            }).Run();

            gridMesh.vertices = meshVertices.ToArray();
            gridMesh.colors = meshColors.ToArray();
            gridMesh.RecalculateNormals();
            gridMesh.RecalculateBounds();

            gridMeshInverted.vertices = meshVerticesInverted.ToArray();
            gridMeshInverted.colors = meshColors.ToArray();
            gridMeshInverted.RecalculateNormals();
            gridMeshInverted.RecalculateBounds();

            meshVertices.Dispose();
            meshVerticesInverted.Dispose();
            meshColors.Dispose();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
