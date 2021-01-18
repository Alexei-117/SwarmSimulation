using Swarm.Movement;
using Swarm.Swarm;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Swarm.Grid
{
    [UpdateAfter(typeof(RestoreCollidedPositionSystem))]
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
                GenericInformation.NumberOfAgents, Allocator.Persistent);

            float widthOfGrid = GridSpawner.gridTileWidth;
            float heightOfGrid = GridSpawner.gridTileHeight;
            int horizontalVertices = GridSpawner.horizontalVertices;

            Dependency = Entities.WithAll<AgentTag>().ForEach((in Translation t) =>
            {
                int posX = (int) math.floor(t.Value.x / widthOfGrid + 0.5f);
                int posZ = (int) math.floor(t.Value.z / heightOfGrid + 0.5f);

                if (posX >= 0 && posZ >= 0)
                {
                    agents.Add(posZ * horizontalVertices + posX, t);
                }
            }).Schedule(Dependency);

            Dependency = Entities.WithAll<GridDotTag>().ForEach((ref AccumulatedAgents accumulatedAgents, in Translation t) =>
            {
                accumulatedAgents.Value = 0;
                int hash = ((int)(t.Value.z / heightOfGrid) * horizontalVertices) + (int)(t.Value.x / widthOfGrid);

                if (agents.ContainsKey(hash))
                {
                    accumulatedAgents.Value += agents.CountValuesForKey(hash);
                }
            }).Schedule(Dependency);

            Dependency.Complete();

            agents.Dispose();
        }
    }
}
