using Unity.Entities;

namespace DOTSTutorial.Rotating
{
    //!! Adds the capacity to drag it in the inspector. If it is a component added by the system then there is no need for this.
    [GenerateAuthoringComponent]
    public struct RotateApplier : IComponentData {}
}
