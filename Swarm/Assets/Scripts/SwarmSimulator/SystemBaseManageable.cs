using Unity.Entities;

namespace Swarm
{
    // Systems run inside SimulationSystemGroup, but there is TransformSystemGroup
    // that runs after the movement has been decided and before the collision is restored.
    // To avoid errors, we're going to update all our frames together.
    [UpdateInGroup(typeof(SwarmSimulatorSystemGroup))]
    public abstract class SystemBaseManageable : SystemBase
    {
        public GenericInformation GenericInformation { get; set; }
        public string Name { get; set; }

        public bool WillRunPerTimeStep { get; set; }

        public virtual void InitializeData()
        {

        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
            SystemManager.AddSystem(this);
        }
    }
}
