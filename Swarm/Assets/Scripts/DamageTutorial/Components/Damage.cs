using Unity.Entities;

namespace DOTSTutorial.Damaging
{

    // One instance for the same component can exist in an entity, so if we want in the same frame deal 2 different damages
    // we need a buffer element, so they can be added to a buffer and processed individually
    public struct Damage : IBufferElementData
    {
        public int Value;
    }
}