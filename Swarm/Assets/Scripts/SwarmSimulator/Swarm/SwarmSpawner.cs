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
using SphereCollider = Unity.Physics.SphereCollider;

namespace Swarm.Swarm
{
    public class SwarmSpawner : MonoBehaviour
    {
        /* Swarm metadata */
        [Header("Swarm data")]
        [SerializeField] private int numberOfAgents;
        [SerializeField] private float gridSpacing;

        /* Agent data */
        [Header("Agent data")]
        [SerializeField] private float gatheringSpeed;
        [SerializeField] private float agentSpeed;
        [SerializeField] private float colliderSize;
        [SerializeField] private float communicationDistance;
        [SerializeField] private float transferRate;
        [SerializeField] private float consumptionRate;

        /* Grid dots data */
        [Header("Rendering")]
        [SerializeField] private Mesh agentMesh;
        [SerializeField] private Material agentMaterial;

        private float gridWidth;
        private float gridHeight;

        private EntityManager entityManager;
        private EntityArchetype agentArchetype;
        private Entity agentEntity;

        public void Initialize()
        {
            GetEntityToClon();
            SetupAgentArchetype();
            SpawnAgents();
        }

        private void GetEntityToClon()
        {
            agentEntity = entityManager.CreateEntityQuery(ComponentType.ReadOnly<AgentTag>()).GetSingletonEntity();
        }

        private void SpawnAgents()
        {
            Random random = new Random();
            random.InitState((uint)200);

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

            /*entityManager.AddComponentData<PreviousTranslation>(entity, new PreviousTranslation
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

            /*SphereGeometry sphereGeometry = new SphereGeometry()
            {
                Center = new float3(0.0f),
                Radius = communicationDistance
            };

            Unity.Physics.Material material = Unity.Physics.Material.Default;
            material.Flags = Unity.Physics.Material.MaterialFlags.IsTrigger;

            BlobAssetReference<Collider> spCollider = SphereCollider.Create(sphereGeometry, CollisionFilter.Default, material);
            entityManager.AddComponentData<PhysicsCollider>(entity, new PhysicsCollider
            {
                Value = spCollider
            });

            unsafe
            {
                Collider* colliderPtr = (Collider*)spCollider.GetUnsafePtr();
                entityManager.AddComponentData<PhysicsMass>(entity, PhysicsMass.CreateDynamic(colliderPtr->MassProperties, 1));
            }

            entityManager.SetComponentData<PhysicsVelocity>(entity, new PhysicsVelocity
            {
                Linear = 0.0f,
                Angular = 0.00f
            });

            entityManager.SetComponentData<PhysicsDamping>(entity, new PhysicsDamping
            {
                Linear = 0.0f,
                Angular = 0.00f
            });

            entityManager.SetComponentData<PhysicsGravityFactor>(entity, new PhysicsGravityFactor
            {
                Value = 0.0f
            });*/

            entityManager.SetComponentData<PhysicsCollider>(entity,
                entityManager.GetComponentData<PhysicsCollider>(agentEntity));

            entityManager.SetComponentData<PhysicsMass>(entity,
                entityManager.GetComponentData<PhysicsMass>(agentEntity));

            entityManager.SetComponentData<PhysicsVelocity>(entity,
                entityManager.GetComponentData<PhysicsVelocity>(agentEntity));

            entityManager.SetComponentData<PhysicsGravityFactor>(entity,
                entityManager.GetComponentData<PhysicsGravityFactor>(agentEntity));

            return entity;
        }

        private void SetupAgentArchetype()
        {
            ComponentType[] components = GenericInformation.GetGenericComponents();
            components = GenericInformation.AddComponents(components, new ComponentType[]
            {
                /*typeof(AgentTag),
                typeof(Speed),
                typeof(MoveForward),
                typeof(RandomData),
                typeof(HorizontalLimits),
                typeof(Gather),
                typeof(PotentialFieldAgent),
                typeof(Collision),
                typeof(Consumption),*/
                typeof(PhysicsCollider),
                typeof(PhysicsVelocity),
                typeof(PhysicsMass),
                typeof(PhysicsGravityFactor)
                /*typeof(PhysicsDamping),
                typeof(HighestPotentialAgent)*/
            });
            
            agentArchetype = entityManager.CreateArchetype(components);
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