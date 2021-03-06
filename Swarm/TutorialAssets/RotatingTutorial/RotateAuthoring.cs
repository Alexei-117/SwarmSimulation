﻿using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSTutorial.Rotating
{
    public class RotateAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private float degreesPerSecond;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<Rotate>(entity, new Rotate { radiansPerSecond = math.radians(degreesPerSecond) });
            dstManager.AddComponentData<RotationEulerXYZ>(entity, new RotationEulerXYZ());
        }
    }
}