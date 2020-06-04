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
        [SerializeField] private Color color1;
        [SerializeField] private float radius1;
        [SerializeField] private float2 position1;

        [Header("Right light")]
        [SerializeField] private Color color2;
        [SerializeField] private float radius2;
        [SerializeField] private float2 position2;

        private EntityManager entityManager;
        private EntityArchetype agentArchetype;

        public void Initialize()
        {
            SetupLightArchetype();
            SpawnLight();
        }

        private void SpawnLight()
        {
            // Create 2
            int lightIndex = 0;
            CreateLightGameObject(lightIndex++, position1, radius1, color1);
            CreateLightGameObject(lightIndex++, position2, radius2, color2);

            CreateLight(position1, radius1);
            CreateLight(position2, radius2);
        }

        private void CreateLightGameObject(int lightIndex, float2 position, float lightRadius, Color color)
        {
            // Create light gameobject
            GameObject lightGameObject = new GameObject("Light" + lightIndex);
            Transform lightTransform = lightGameObject.GetComponent<Transform>();
            lightTransform.position = new Vector3(position.x, lightRadius, position.y);

            //Lights that point downwards
            lightTransform.rotation = Quaternion.LookRotation(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));

            UnityEngine.Light light = lightGameObject.AddComponent<UnityEngine.Light>();
            light.type = LightType.Spot;
            light.color = color;
            light.areaSize = new Vector2(lightRadius, lightRadius);
            light.intensity = 30;
            light.spotAngle = 90;
            light.range = 30;
        }

        private Entity CreateLight(float2 position, float lightRadius)
        {
            // Create entity
            Entity entity = entityManager.CreateEntity(agentArchetype);

            entityManager.AddComponentData<Translation>(entity, new Translation
            {
                Value = new float3(position.x, 0f, position.y)
            });

            entityManager.AddComponentData<Light>(entity, new Light
            {
                Radius = lightRadius
            });

            return entity;
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

        public void SetEntityManager(EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }
    }
}
