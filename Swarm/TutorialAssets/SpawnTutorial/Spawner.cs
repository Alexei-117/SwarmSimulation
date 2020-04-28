using Unity.Entities;

namespace DOTSTutorial.Spawning
{
    public struct Spawner : IComponentData
    {
        public Entity prefab;
        public float maxDistanceFromSpawner;
        public float secsBetweenSpawns;
        public float secsToNextSpawn;
    }
}