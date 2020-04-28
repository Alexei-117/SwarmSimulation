using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Swarm
{
    public class GenericInformation : MonoBehaviour
    {
        public static ComponentType[] GetGenericComponents()
        {
            return new ComponentType[]
            {
                typeof(Translation),
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
    }
}