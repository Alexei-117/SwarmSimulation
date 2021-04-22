using Swarm.Movement;
using Swarm.Swarm;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Light = Swarm.Scenario.Light;

namespace Swarm
{
    public class GenericInformation : MonoBehaviour
    {
        [Header("Meta data")]
        [SerializeField] public float TimeStep = 1.0f;

        [Header("Agents data")]
        [SerializeField] public float AgentSize = 1.0f;
        [SerializeField] public int NumberOfAgents = 375;

        [Header("Layout data")]
        [SerializeField] public float LayoutWidth = 80.0f;
        [SerializeField] public float LayoutHeight = 40.0f;

        public static ComponentType[] GetGenericComponents()
        {
            return new ComponentType[]
            {
                typeof(Translation),
                typeof(PreviousTranslation),
                typeof(Rotation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            };
        }

        public static ComponentType[] AddComponent(ComponentType[] components, ComponentType component)
        {
            ComponentType[] returnComponents = new ComponentType[components.Length + 1];
            returnComponents.SetValue(component, components.Length);
            return returnComponents;
        }

        public static ComponentType[] AddComponents(ComponentType[] components, ComponentType[] addedComponents)
        {
            ComponentType[] returnComponents = new ComponentType[components.Length + addedComponents.Length];
            for (int i = 0; i < components.Length; i++)
            {
                returnComponents.SetValue(components[i], i);
            }

            for (int i = 0; i < addedComponents.Length; i++)
            {
                returnComponents.SetValue(addedComponents[i], i + components.Length);
            }

            return returnComponents;
        }

        public void SetEntityManager(EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }

        public void SetData()
        {
            if (isDataSet)
                return;

            isDataSet = true;

            lightTranslations = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Light>(), ComponentType.ReadOnly<Translation>())
                .ToComponentDataArray<Translation>(Allocator.Persistent);
            lights = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Light>(), ComponentType.ReadOnly<Translation>())
                .ToComponentDataArray<Light>(Allocator.Persistent);
        }

        public void OnDisable()
        {
            lightTranslations.Dispose();
            lights.Dispose();
        }

        /*Generic*/
        private EntityManager entityManager;

        /*Data*/
        private bool isDataSet;

        private NativeArray<Translation> lightTranslations;
        public NativeArray<Translation> GetLightTranslations
        {
            get
            {
                SetData();
                return lightTranslations;
            }
        }

        private NativeArray<Light> lights;
        public NativeArray<Light> GetLights
        {
            get
            {
                SetData();
                return lights;
            }
        }
    }
}