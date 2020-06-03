using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Swarm.Movement;
using System.Collections.Generic;

namespace Swarm.Grid
{
    public class GridSpawner : MonoBehaviour
    {
        /* Grid metadata */
        [SerializeField] private int columns;
        [SerializeField] private int rows;
        [SerializeField] private float gridInitialHeight;

        /* Grid dots data */
        [SerializeField] private Material dotMaterial;

        /* Movement data */
        [SerializeField] private float speed;

        /* Generic metadata */
        private EntityManager entityManager;
        private EntityArchetype gridDotArchetype;

        /* Grid metadata */
        private float gridWidth;
        private float gridHeight;

        public void Initialize()
        {
            SetupGridDotArchetype();
            CreateGridGameObject();
        }

        private void CreateGridGameObject()
        {
            GameObject grid = new GameObject("GridPlane");
            grid.tag = "GridPlane";

            MeshFilter meshFilter = grid.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = grid.AddComponent<MeshRenderer>();
            meshRenderer.material = dotMaterial;

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            int horizontalVertices = columns + 1;
            int verticalVertices = rows + 1;

            float uvFactorX = 1.0f / columns;
            float uvFactorZ = 1.0f / rows;
            float scaleX = gridWidth / columns;
            float scaleZ = gridHeight / rows;

            float vertexIndex = 0;
            for(int z = 0; z < verticalVertices; z++)
            {
                for(int x = 0; x < horizontalVertices; x++)
                {
                    float xPos = x * scaleX;
                    float zPos = z * scaleZ;
                    vertices.Add(new Vector3(xPos, gridInitialHeight, zPos));
                    uvs.Add(new Vector2(x * uvFactorX, z * uvFactorZ));

                    MakeGridDot(vertexIndex++, xPos, zPos);
                }
            }

            for (int z = 0; z < rows; z++)
            {
                for (int x = 0; x < columns; x++)
                {
                    triangles.Add(z * horizontalVertices + x);
                    triangles.Add((z + 1) * horizontalVertices + x);
                    triangles.Add(z * horizontalVertices + x + 1);

                    triangles.Add((z + 1) * horizontalVertices + x);
                    triangles.Add((z + 1) * horizontalVertices + x + 1);
                    triangles.Add(z * horizontalVertices + x + 1);
                }
            }

            Mesh mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                uv = uvs.ToArray(),
                triangles = triangles.ToArray()
            };

            meshFilter.mesh = mesh;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.MarkDynamic(); // Optimyze for constant updates
        }

        private Entity MakeGridDot(float index, float x, float z)
        {
            Entity entity = entityManager.CreateEntity(gridDotArchetype);

            entityManager.AddComponentData<GridDotTag>(entity, new GridDotTag
            {
                Index = index
            });

            entityManager.AddComponentData<Translation>(entity, new Translation
            {
                Value = new float3(x, gridInitialHeight, z)
            });

            entityManager.AddComponentData<PlotMetadata>(entity, new PlotMetadata
            {
                InitialHeight = gridInitialHeight,
                dotBoundaries = new float4(
                    x - (gridWidth / columns) * 0.5f, 
                    x + (gridWidth / columns) * 0.5f, 
                    z - (gridHeight / rows) * 0.5f, 
                    z + (gridHeight / rows) * 0.5f)
            });

            entityManager.AddComponentData<Speed>(entity, new Speed
            {
                Value = speed
            });

            entityManager.AddComponentData<AccumulatedAgents>(entity, new AccumulatedAgents
            {
                Value = 0
            });

            return entity;
        }

        private void SetupGridDotArchetype()
        {
            ComponentType[] components = GenericInformation.GetGenericComponents();
            components = GenericInformation.AddComponents(components, new ComponentType[]
            {
                typeof(GridDotTag),
                typeof(Speed),
                typeof(MoveForward),
                typeof(AccumulatedAgents),
                typeof(PlotMetadata)
            });

            gridDotArchetype = entityManager.CreateArchetype(components);
        }

        public void SetLayoutLimits(float width, float height)
        {
            gridWidth = width;
            gridHeight = height;
        }

        public void SetEntityManager(EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }
    }
}
