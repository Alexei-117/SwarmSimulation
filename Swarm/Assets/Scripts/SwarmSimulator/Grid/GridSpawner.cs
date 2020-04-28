using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Swarm.Movement;

namespace Swarm.Grid
{
    public class GridSpawner : MonoBehaviour
    {
        /* Grid metadata */
        [SerializeField] private float gridWidth;
        [SerializeField] private float gridHeight;
        [SerializeField] private float gridSpacing;
        [SerializeField] private float gridInitialHeight;

        /* Grid dots data */
        [SerializeField] private Mesh dotMesh;
        [SerializeField] private Material dotMaterial;

        /* Movement data */
        [SerializeField] private float speed;

        private EntityManager entityManager;
        private EntityArchetype gridDotArchetype;

        void Start()
        {
            SetupEntityManager();
            SetupGridDotArchetype();
            MakeGrid();
        }

        private void MakeGrid()
        {
            for(int x = 0; x < gridWidth; x++)
            {
                for(int z = 0; z < gridHeight; z++)
                {
                    MakeGridDot(x * gridSpacing, z * gridSpacing);
                }
            }
        }

        private Entity MakeGridDot(float x, float z)
        {
            Entity entity = entityManager.CreateEntity(gridDotArchetype);

            entityManager.AddComponentData<Translation>(entity, new Translation
            {
                Value = new float3(x + gridSpacing * 0.5f, gridInitialHeight, z + gridSpacing * 0.5f)
            });

            entityManager.AddComponentData<PlotMetadata>(entity, new PlotMetadata
            {
                InitialHeight = gridInitialHeight,
                dotBoundaries = new float4(x, x + gridSpacing, z, z + gridSpacing)
            });

            entityManager.AddSharedComponentData<RenderMesh>(entity, new RenderMesh
            {
                mesh = dotMesh,
                material = dotMaterial
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

        private void SetupEntityManager()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
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
    }
}
