using Swarm.Scenario;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Light = Swarm.Scenario.Light;

namespace Swarm.Swarm
{
    [UpdateBefore(typeof(TransferSystem))]
    public class GatherSystem : SystemBase
    {
        public NativeArray<Translation> lightsTranslations;
        public NativeArray<Light> lights;

        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            // Initialise variables
            NativeArray<float3> lightsPositions = lightsTranslations.Reinterpret<float3>();

            NativeArray<float> lightsSizes = lights.Reinterpret<float>();

            float time = Time.DeltaTime;
            this.Dependency = Entities.WithAll<AgentTag>().ForEach((ref PotentialFieldAgent potentialField, in Translation t, in Gather gather) =>
            {
                // Retrieve data
                for (int i = 0; i < lightsPositions.Length; i++)
                {
                    if (math.distance(new float2(t.Value.x, t.Value.z), new float2(lightsPositions[i].x, lightsPositions[i].z)) <= lightsSizes[i])
                    {
                        potentialField.Value += gather.Value * time;
                    }
                }
            }).ScheduleParallel(this.Dependency);
        }
    }
}