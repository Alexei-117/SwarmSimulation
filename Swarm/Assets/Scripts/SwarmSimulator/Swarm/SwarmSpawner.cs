using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Swarm.Movement;

using Random = Unity.Mathematics.Random;
using Swarm.Grid;

namespace Swarm.Swarm
{
    public class SwarmSpawner : MonoBehaviour
    {
        /* Swarm metadata */
        [Header("Swarm data")]
        [SerializeField] private int numberOfAgents;
        [SerializeField] private float gridWidth;
        [SerializeField] private float gridHeight;
        [SerializeField] private float gridSpacing;

        /* Agent data */
        [SerializeField] private float gatheringSpeed;
        [SerializeField] private float agentSpeed;

        /* Grid dots data */
        [Header("Rendering")]
        [SerializeField] private Mesh agentMesh;
        [SerializeField] private Material agentMaterial;

        private EntityManager entityManager;
        private EntityArchetype agentArchetype;

        void Start()
        {
            SetupEntityManager();
            SetupAgentArchetype();
            SpawnAgents();

            AccumulateAgentsSystem system = new AccumulateAgentsSystem();
        }

        private void SpawnAgents()
        {
            Random random = new Random();
            random.InitState((uint) 200);

            for (int cont = 0; cont < numberOfAgents; cont++)
            {
                CreateAgent(random.NextFloat(0, gridWidth * gridSpacing), random.NextFloat(0, gridHeight * gridSpacing), random.NextUInt());
            }
        }

        private Entity CreateAgent(float x, float z, uint seed)
        {
            Entity entity = entityManager.CreateEntity(agentArchetype);

            entityManager.AddComponentData<Translation>(entity, new Translation
            {
                Value = new float3(x, 0f, z)
            });

            entityManager.AddComponentData<Speed>(entity, new Speed
            {
                Value = agentSpeed
            });

            entityManager.AddSharedComponentData<RenderMesh>(entity, new RenderMesh
            {
                mesh = agentMesh,
                material = agentMaterial
            });

            Random agentRandom = new Random();
            agentRandom.InitState(seed);

            entityManager.AddComponentData<RandomData>(entity, new RandomData
            {
                random = agentRandom
            });

            entityManager.AddComponentData<HorizontalLimits>(entity, new HorizontalLimits
            {
                x1 = 0,
                x2 = gridWidth,
                z1 = 0,
                z2 = gridHeight
            });

            entityManager.AddComponentData<Gather>(entity, new Gather
            {
                Value = gatheringSpeed
            });

            entityManager.AddComponentData<PotentialFieldAgent>(entity, new PotentialFieldAgent
            {
                Value = 0
            });

            return entity;
        }

        private void SetupEntityManager()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void SetupAgentArchetype()
        {
            ComponentType[] components = GenericInformation.GetGenericComponents();
            components = GenericInformation.AddComponents(components, new ComponentType[]
            {
                typeof(AgentTag),
                typeof(Speed),
                typeof(MoveForward),
                typeof(RandomData),
                typeof(HorizontalLimits),
                typeof(Gather),
                typeof(PotentialFieldAgent)
            });

            agentArchetype = entityManager.CreateArchetype(components);
        }
    }
}
