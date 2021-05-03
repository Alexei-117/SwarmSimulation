using Swarm.Movement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Collider = Unity.Physics.Collider;
using Collision = Swarm.Movement.Collision;
using Material = UnityEngine.Material;
using Random = Unity.Mathematics.Random;

namespace Swarm.Swarm
{
    public class SwarmSpawner : MonoBehaviour
    {
        [Header("Spawn Area")]
        [SerializeField] private float2 initialPoint;
        [SerializeField] private float2 endPoint;


        [Header("Agent data")]
        [SerializeField] private float gatheringSpeed;
        [SerializeField] private float agentSpeed;
        [SerializeField] private float colliderSize;
        [SerializeField] private float communicationDistance;
        [SerializeField] private float transferRate;
        [SerializeField] private float consumptionRate;
        [SerializeField] private GameObject agentWithPhysics;

        [Header("Rendering")]
        [SerializeField] private Mesh agentMesh;
        [SerializeField] private Material agentMaterial;
        [SerializeField] private Mesh communcationAreaMesh;
        [SerializeField] private Material communicationAreaMaterial;
        [SerializeField] private Mesh collisionAreaMesh;
        [SerializeField] private Material collisionAreaMaterial;

        private int numberOfAgents;
        private float gridWidth;
        private float gridHeight;

        private Entity entityWithPhysics;
        private EntityManager entityManager;
        private BlobAssetStore asset;


        public void Initialize()
        {
            SpawnAgents();
        }

        private void SpawnAgents()
        {
            //[COMMENT] Should I use blob assets for the common variables that they will all access? since they won't be different
            asset = new BlobAssetStore();
            entityWithPhysics = GameObjectConversionUtility.ConvertGameObjectHierarchy(agentWithPhysics,
                GameObjectConversionSettings.FromWorld(entityManager.World, asset ));

            Random random = new Random();
            random.InitState((uint)UnityEngine.Random.Range(1, 100000));

            for (int cont = 0; cont < numberOfAgents; cont++)
            {
                CreateAgent(random.NextFloat(initialPoint.x, endPoint.x), random.NextFloat(initialPoint.y, endPoint.y), random.NextUInt(), cont);
            }
        }

        private Entity CreateAgent(float x, float z, uint seed, int index)
        {
            Entity entity = entityManager.Instantiate(entityWithPhysics);

            entityManager.SetComponentData<Translation>(entity, new Translation
            {
                Value = new float3(x, 0f, z)
            });

            CreateCommunicationArea(x, z, index);
            CreateCollisionArea(x, z, index);

            entityManager.AddComponentData<AgentTag>(entity, new AgentTag());
            entityManager.AddComponentData<MoveForward>(entity, new MoveForward());
            entityManager.AddComponentData<RenderBounds>(entity, new RenderBounds());

            entityManager.AddComponentData<PreviousTranslation>(entity, new PreviousTranslation
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

            entityManager.AddComponentData<Gather>(entity, new Gather
            {
                Value = gatheringSpeed
            });

            entityManager.AddComponentData<PotentialFieldAgent>(entity, new PotentialFieldAgent
            {
                Value = 0.0f,
                TransferRate = transferRate
            });

            entityManager.AddComponentData<HighestPotentialAgent>(entity, new HighestPotentialAgent
            {
                Potential = 0.0f,
                Direction = float3.zero
            });

            entityManager.AddComponentData<Collision>(entity, new Collision
            {
                Radius = colliderSize
            });

            entityManager.AddComponentData<Consumption>(entity, new Consumption
            {
                Value = consumptionRate
            });

            return entity;
        }

        private void CreateCommunicationArea(float x, float z, int index)
        {

            ComponentType[] components = GenericInformation.GetGenericComponents();
            components = GenericInformation.AddComponents(components, new ComponentType[]
            {
                typeof(CommunicationAreaTag)
            });

            Entity entity = entityManager.CreateEntity(entityManager.CreateArchetype(components));
            
            entityManager.SetComponentData<CommunicationAreaTag>(entity, new CommunicationAreaTag
            {
                AgentIndex = index
            });

            entityManager.SetComponentData<Translation>(entity, new Translation
            {
                Value = new float3(x, 5.0f, z)
            });

            entityManager.SetSharedComponentData<RenderMesh>(entity, new RenderMesh
            {
                mesh = communcationAreaMesh,
                material = communicationAreaMaterial
            });

            /// Communication distance is defined as the distance till you reach another agent.
            /// A few assumptios are made:
            /// 1. All agents have their Communication device on.
            /// 2. Communication reach is circular.
            /// From 1 and 2 we can infere that the trigger system works in a straight line from robot to robot,
            /// this means that to replicate the results in Heiko's work, we need to take into account also the
            /// size of the robots. We will assume that the communication device is located at the center of the
            /// agents, so if the distance is 3.5m between robots, then the communication area is 1.75m effectively.
            entityManager.AddComponentData<Scale>(entity, new Scale
            {
                Value = communicationDistance
            });
        }

        private void CreateCollisionArea(float x, float z, int index)
        {

            ComponentType[] components = GenericInformation.GetGenericComponents();
            components = GenericInformation.AddComponents(components, new ComponentType[]
            {
                typeof(CollisionAreaTag)
            });

            Entity entity = entityManager.CreateEntity(entityManager.CreateArchetype(components));

            entityManager.SetComponentData<CollisionAreaTag>(entity, new CollisionAreaTag
            {
                AgentIndex = index
            });

            entityManager.SetComponentData<Translation>(entity, new Translation
            {
                Value = new float3(x, 2.0f, z)
            });

            entityManager.SetSharedComponentData<RenderMesh>(entity, new RenderMesh
            {
                mesh = collisionAreaMesh,
                material = collisionAreaMaterial
            });

            entityManager.AddComponentData<Scale>(entity, new Scale
            {
                Value = colliderSize
            });
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

        public void SetNumberOfAgents(int num)
        {
            numberOfAgents = num;
        }

        /// Disposes of asset on application end
        private void OnDisable()
        {
            asset.Dispose();
        }
    }
}