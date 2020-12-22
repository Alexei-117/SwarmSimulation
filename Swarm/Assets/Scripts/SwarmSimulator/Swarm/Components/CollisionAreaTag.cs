using Unity.Entities;

[GenerateAuthoringComponent]
public struct CollisionAreaTag : IComponentData
{
    public int AgentIndex;
}
