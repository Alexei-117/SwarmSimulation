using UnityEngine;

namespace Unity.Entities
{
    [ExecuteAlways]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class SwarmSimulatorSystemGroup : ComponentSystemGroup
    {
        public SwarmSimulatorSystemGroup() {
        }
    }
}