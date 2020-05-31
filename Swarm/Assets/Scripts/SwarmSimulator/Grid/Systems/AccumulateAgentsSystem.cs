using System.Collections;
using System.Diagnostics;
using System.Linq;
using Swarm.Swarm;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Swarm.Grid
{
    public class AccumulateAgentsSystem : SystemBaseManageable
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "AccumulateAgents";
        }

        protected override void OnUpdate()
        {
            NativeMultiHashMap<int, Translation> agents = new NativeMultiHashMap<int, Translation>(
                (int)(GenericInformation.LayoutWidth * GenericInformation.LayoutHeight), Allocator.TempJob);

            int widthOfMap = (int)GenericInformation.LayoutWidth;

            JobHandle handle = Entities.WithAll<AgentTag>().ForEach((in Translation t) =>
            {
                int posX = (int) math.floor(t.Value.x);
                int posZ = (int) math.floor(t.Value.z);

                agents.Add(posZ * widthOfMap + posX, t);
            }).Schedule(Dependency);

            JobHandle secondHandle = Entities.WithAll<GridDotTag>().ForEach((ref AccumulatedAgents accumulatedAgents, in PlotMetadata dotData) =>
            {
                accumulatedAgents.Value = 0;
                int hash = ((int)dotData.dotBoundaries.z * widthOfMap) + (int)dotData.dotBoundaries.x;

                if (agents.ContainsKey(hash))
                {
                    accumulatedAgents.Value += agents.CountValuesForKey(hash);
                }
            }).Schedule(handle);

            secondHandle.Complete();

            agents.Dispose();
        }
    }
}
