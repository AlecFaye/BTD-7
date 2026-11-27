using UnityEngine;

namespace Assets.Game.Tower
{
    [CreateAssetMenu(fileName = "so_RangedTower", menuName = "Tower/Ranged Tower Definition")]
    public class RangedTowerDefinition : TowerDefinition
    {
        [Header("Ranged")]
        [SerializeField, Min(1f)] private float _range = 1;
        [SerializeField, Min(0.1f)] private float _fireRate = 0.1f;

        public float Range => _range;
        public float FireRate => _fireRate;

        // TODO: Add projectile definition
        // TODO: Add targeting modes
    }
}
