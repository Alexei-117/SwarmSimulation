using Swarm.Movement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
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
            random.InitState((uint)((int)Time.time * 100000.0f + 1));

            for (int cont = 0; cont < numberOfAgents; cont++)
            {
                CreateAgent(random.NextFloat(0, gridWidth), random.NextFloat(0, gridHeight), random.NextUInt());
            }
        }

        private Entity CreateAgent(float x, float z, uint seed)
        {
            BlobAssetReference<Collider> sourceCollider = entityManager.GetComponentData<PhysicsCollider>(entityWithPhysics).Value;

            Entity entity = entityManager.Instantiate(entityWithPhysics);

            entityManager.SetComponentData<Translation>(entity, new Translation
            {
                Value = new float3(x, 0f, z)
            });

            CreateCommunicationArea(x, z);
            CreateCollisionArea(x, z);

            entityManager.SetComponentData<PhysicsCollider>(entity, new PhysicsCollider
            {
                Value = sourceCollider
            });

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

        private void CreateCommunicationArea(float x, float z)
        {

            ComponentType[] components = GenericInformation.GetGenericComponents();
            components = GenericInformation.AddComponents(components, new ComponentType[]
            {
                typeof(CommunicationAreaTag)
            });

            Entity entity = entityManager.CreateEntity(entityManager.CreateArchetype(components));

            entityManager.SetComponentData<Translation>(entity, new Translation
            {
                Value = new float3(x, 0f, z)
            });

            entityManager.SetSharedComponentData<RenderMesh>(entity, new RenderMesh
            {
                mesh = communcationAreaMesh,
                material = communicationAreaMaterial
            });

            entityManager.AddComponentData<Scale>(entity, new Scale
            {
                Value = 3.5f
            });
        }

        private void CreateCollisionArea(float x, float z)
        {

            ComponentType[] components = GenericInformation.GetGenericComponents();
            components = GenericInformation.AddComponents(components, new ComponentType[]
            {
                typeof(CollisionAreaTag)
            });

            Entity entity = entityManager.CreateEntity(entityManager.CreateArchetype(components));

            entityManager.SetComponentData<Translation>(entity, new Translation
            {
                Value = new float3(x, 1.0f, z)
            });

            entityManager.SetSharedComponentData<RenderMesh>(entity, new RenderMesh
            {
                mesh = collisionAreaMesh,
                material = collisionAreaMaterial
            });

            entityManager.AddComponentData<Scale>(entity, new Scale
            {
                Value = 0.75f
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

        private void OnDisable()
        {
            asset.Dispose();
        }
    }
}