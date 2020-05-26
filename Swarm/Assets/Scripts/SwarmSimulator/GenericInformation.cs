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
        [SerializeField] public float TimeStep;

        [Header("Agents data")]
        [SerializeField] public float AgentSize;

        [Header("Layout data")]
        [SerializeField] public float LayoutWidth;
        [SerializeField] public float LayoutHeight;

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

        public float GetLayoutWidth()
        {
            return LayoutWidth * AgentSize;
        }

        public float GetLayoutHeight()
        {
            return LayoutHeight * AgentSize;
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

            agentsTranslations = entityManager.CreateEntityQuery(ComponentType.ReadOnly<AgentTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<PotentialFieldAgent>())
                .ToComponentDataArray<Translation>(Allocator.Persistent);
            agentsPotentials = entityManager.CreateEntityQuery(ComponentType.ReadOnly<AgentTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<PotentialFieldAgent>())
                .ToComponentDataArray<PotentialFieldAgent>(Allocator.Persistent);

            lightTranslations = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Light>(), ComponentType.ReadOnly<Translation>())
                .ToComponentDataArray<Translation>(Allocator.Persistent);
            lights = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Light>(), ComponentType.ReadOnly<Translation>())
                .ToComponentDataArray<Light>(Allocator.Persistent);
        }

        public void OnDisable()
        {
            agentsTranslations.Dispose();
            agentsPotentials.Dispose();

            lightTranslations.Dispose();
            lights.Dispose();
        }

        /*Generic*/
        private EntityManager entityManager;

        /*Data*/
        private bool isDataSet;

        private NativeArray<Translation> agentsTranslations;
        public NativeArray<Translation> GetAgentsTranslations
        {
            get
            {
                SetData();
                return agentsTranslations;
            }
        }

        private NativeArray<PotentialFieldAgent> agentsPotentials;
        public NativeArray<PotentialFieldAgent> GetAgentsPotentials
        {
            get
            {
                SetData();
                return agentsPotentials;
            }
        }

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