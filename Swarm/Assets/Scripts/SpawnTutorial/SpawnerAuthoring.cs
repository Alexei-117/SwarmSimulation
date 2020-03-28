using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace DOTSTutorial.Spawning
{
    // IDeclareReferencedPrefabs is assigned to be able to create prefabs from this entity
    public class SpawnerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private float spawnRate;
        [SerializeField] private float maxDistanceFromSpawn;
        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(prefab);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<Spawner>(entity, new Spawner
            {
                prefab = conversionSystem.GetPrimaryEntity(prefab),
                maxDistanceFromSpawner = maxDistanceFromSpawn,
                secsBetweenSpawns = spawnRate,
                secsToNextSpawn = spawnRate 
            });
        }
    }
}