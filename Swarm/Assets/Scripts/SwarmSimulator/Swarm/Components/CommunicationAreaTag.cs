using Unity.Entities;

[GenerateAuthoringComponent]
public struct CommunicationAreaTag : IComponentData
{
    public int AgentIndex;
}
