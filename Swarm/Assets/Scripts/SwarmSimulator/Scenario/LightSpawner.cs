using Swarm;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Swarm.Scenario
{
    public class LightSpawner : MonoBehaviour
    {
        [Header("Left light")] 
        [SerializeField] private float leftLightRadius;
        [SerializeField] private float2 leftLightPosition;

        [Header("Right light")] 
        [SerializeField] private float rightLightRadius;
        [SerializeField] private float2 rightLightPosition;

        /* Grid dots data */
        [Header("Rendering")]
        [SerializeField] private Mesh agentMesh;
        [SerializeField] private Material agentMaterial;

        private EntityManager entityManager;
        private EntityArchetype agentArchetype;


        void Start()
        {
            SetupEntityManager();
            SetupLightArchetype();
            SpawnLight();
        }

        private void SpawnLight()
        {
            // Create 2 
            CreateLight(leftLightPosition, leftLightRadius);
            CreateLight(rightLightPosition, rightLightRadius);
        }

        private Entity CreateLight(float2 position, float lightRadius)
        {
            Entity entity = entityManager.CreateEntity(agentArchetype);

            entityManager.AddComponentData<Translation>(entity, new Translation
            {
                Value = new float3(position.x, 0f, position.y)
            });

            entityManager.AddComponentData<Light>(entity, new Light
            {
                Radius = lightRadius
            });

            entityManager.AddSharedComponentData<RenderMesh>(entity, new RenderMesh
            {
                mesh = agentMesh,
                material = agentMaterial
            });

            return entity;
        }

        private void SetupEntityManager()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        private void SetupLightArchetype()
        {
            ComponentType[] components = GenericInformation.GetGenericComponents();
            components = GenericInformation.AddComponents(components, new ComponentType[]
            {
                typeof(Light)
            });

            agentArchetype = entityManager.CreateArchetype(components);
        }
    }
}
