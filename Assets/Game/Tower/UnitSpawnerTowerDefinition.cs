using UnityEngine;

namespace Assets.Game.Tower
{
    [CreateAssetMenu(fileName = "so_UnitSpawnerTower", menuName = "Tower/Unit Spawner Tower Definition")]
    public class UnitSpawnerTowerDefinition : TowerDefinition
    {
        [Header("Spawn")]
        [SerializeField, Range(0, 10)] private int _initialSpawnCount = 3;
        [SerializeField, Range(1, 10)] private int _maxSpawnCount = 3;
        [SerializeField, Range(1f, 10f)] private float _respawnDelay = 5f;

        public int InitialSpawnCount => _initialSpawnCount;
        public int MaxSpawnCount => _maxSpawnCount;
        public float RespawnDelay => _respawnDelay;

        // TODO: Add unit spawn definition
    }
}
