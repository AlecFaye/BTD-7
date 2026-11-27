using UnityEngine;

namespace Assets.Game.Tower
{
    [CreateAssetMenu(fileName = "Ranged Tower", menuName = "Tower/Ranged Tower Definition")]
    public class RangedTowerDefinition : TowerDefinition
    {
        [Header("Ranged")]
        [SerializeField, Min(1f)] private float _range;
        [SerializeField, Min(0.1f)] private float _fireRate;

        public float Range => _range;
        public float FireRate => _fireRate;

        // TODO: Add projectile definition
        // TODO: Add targeting modes
    }
}
