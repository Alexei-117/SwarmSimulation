using Unity.Entities;

namespace DOTSTutorial.Damaging
{
    [GenerateAuthoringComponent]
    public struct DealDamage : IComponentData
    {
        public int Value;
    }
}