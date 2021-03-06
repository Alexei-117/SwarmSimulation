﻿using Swarm.Movement;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Swarm.Grid
{
    public class GridSpawner : MonoBehaviour
    {
        [Header("Grid plotting")]
        [SerializeField] private int columns;
        [SerializeField] private int rows;
        [SerializeField] private float gridInitialHeight;
        [SerializeField] private float speed;
        [SerializeField] private Material dotMaterial;

        /* Generic metadata */
        private GenericInformation genericInformation;
        private EntityManager entityManager;
        private EntityArchetype gridDotArchetype;

        /* Grid metadata */
        public static float gridTileWidth;
        public static float gridTileHeight;
        public static int horizontalVertices;
        public static int verticalVertices;
        private float gridWidth;
        private float gridHeight;

        public void Initialize()
        {
            SetupGridDotArchetype();
            CreateGridGameObject();
        }

        private GameObject CreateGridGameObject()
        {
            GameObject grid = new GameObject("GridPlane");
            grid.tag = "GridPlane";

            MeshFilter meshFilter = grid.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = grid.AddComponent<MeshRenderer>();
            meshRenderer.material = dotMaterial;

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            horizontalVertices = columns + 1;
            verticalVertices = rows + 1;

            float uvFactorX = 1.0f / columns;
            float uvFactorZ = 1.0f / rows;
            gridTileWidth = gridWidth / columns;
            gridTileHeight = gridHeight / rows;

            int vertexIndex = 0;
            for(int z = 0; z < verticalVertices; z++)
            {
                for(int x = 0; x < horizontalVertices; x++)
                {
                    float xPos = x * gridTileWidth;
                    float zPos = z * gridTileHeight;
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
                triangles = triangles.ToArray(),
            };

            meshFilter.mesh = mesh;
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.MarkDynamic(); // Optimyze for constant updates

            return grid;
        }

        private Entity MakeGridDot(int index, float x, float z)
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
                typeof(AccumulatedAgents)
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

        public void SetGenericInformation(GenericInformation genericInformation)
        {
            this.genericInformation = genericInformation;
        }
    }
}
