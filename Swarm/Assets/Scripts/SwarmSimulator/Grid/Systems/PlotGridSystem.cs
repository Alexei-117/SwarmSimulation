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
        private NativeArray<Color> meshColors;

        public override void InitializeData()
        {
            gridMesh = GameObject.Find("GridPlane").GetComponent<MeshFilter>().mesh;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "PlotGrid";
        }

        protected override void OnUpdate()
        {
            float gridInitialHeight = GenericInformation.GridInitialHeight;
            Dependency = Entities.WithAll<GridDotTag>().ForEach((ref MoveForward moveForward, in Translation t, in AccumulatedAgents agents) =>
            {
                moveForward.Direction = new float3(0.0f, ((float) agents.Value) + gridInitialHeight - t.Value.y, 0.0f);
            }).ScheduleParallel(Dependency);

            Dependency.Complete();

            meshVertices = new NativeArray<Vector3>(gridMesh.vertexCount, Allocator.TempJob);
            meshColors = new NativeArray<Color>(gridMesh.vertexCount, Allocator.TempJob);
            Entities.WithoutBurst().ForEach((in GridDotTag gridDot, in Translation t) =>
            {
                meshVertices[gridDot.Index] = t.Value;

                /// Paint dot to reflect number of agents. From Green (0 agents) --> To Red (9 agents), in HSV for easier computation
                /// where green is 0.32f approximately and 0.0f is red
                meshColors[gridDot.Index] = Color.HSVToRGB( Mathf.Max(1.0f - 0.68f - ((t.Value.y - 10.0f )/ 32.0f), 0.0f), 0.8f, 0.8f);
            }).Run();

            gridMesh.vertices = meshVertices.ToArray();
            gridMesh.colors = meshColors.ToArray();
            gridMesh.RecalculateNormals();
            gridMesh.RecalculateBounds();

            meshVertices.Dispose();
            meshColors.Dispose();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
