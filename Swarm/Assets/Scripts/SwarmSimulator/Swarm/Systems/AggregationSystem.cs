using Swarm.Movement;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Swarm.Swarm
{
    [UpdateAfter(typeof(RestoreCollidedPositionSystem))]
    public class AggregationSystem : SystemBaseManageable
    {
        public Text leftLightText;
        public Text rightLightText;
        protected override void OnCreate()
        {
            base.OnCreate();
            Name = "Aggregation";
        }

        public override void InitializeData()
        {
            leftLightText = GameObject.FindGameObjectsWithTag("Counter")[0].GetComponent<Text>();
            rightLightText = GameObject.FindGameObjectsWithTag("Counter")[1].GetComponent<Text>();
        }

        protected override void OnUpdate()
        {
            NativeArray<float3> lightsPositions = GenericInformation.GetLightTranslations.Reinterpret<float3>();
            NativeArray<float> lightsSizes = GenericInformation.GetLights.Reinterpret<float>();
            float agentSize = GenericInformation.AgentSize * 0.5f;
            int leftLightAgents = 0;
            int rightLightAgents = 0;
            
            Entities.WithAll<AgentTag>().ForEach((ref CompositeScale size, in Translation t) =>
            {
                size.Value = float4x4.Scale(0.75f, 1.0f, 0.75f);
                for (int i = 0; i < lightsPositions.Length; i++)
                {
                    if (math.distance(new float2(t.Value.x, t.Value.z), new float2(lightsPositions[i].x, lightsPositions[i].z)) <= 10.0f)
                    {
                        size.Value = float4x4.Scale(0.3f, 2.0f, 0.3f);
                        if ( i == 0)
                        {
                            leftLightAgents++;
                        } else {
                            rightLightAgents++;
                        }
                    }
                }
            }).Run();

            // Display into text
            leftLightText.text = leftLightAgents.ToString();
            rightLightText.text = rightLightAgents.ToString();
        }
    }
}