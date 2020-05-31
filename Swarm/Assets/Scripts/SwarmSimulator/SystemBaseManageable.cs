using Unity.Entities;

namespace Swarm
{
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
