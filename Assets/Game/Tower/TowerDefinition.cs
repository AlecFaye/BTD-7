using UnityEngine;

namespace Assets.Game.Tower
{
    [CreateAssetMenu(fileName = "so_Tower", menuName = "Tower/Tower Definition")]
    public class TowerDefinition : ScriptableObject
    {
        [Header("Cost")]
        [SerializeField, Min(0)] private int _cost;
        [SerializeField, Range(0.1f, 1f)] private float _sellRatio = 0.8f;

        public int Cost => _cost;
        public float SellRatio => _sellRatio;

        [Header("Ranged")]
        [SerializeField, Min(1f)] private float _range = 1;
        [SerializeField, Min(0.1f)] private float _fireRate = 0.1f;

        public float Range => _range;
        public float FireRate => _fireRate;

        // TODO: Add damage profile
        // TODO: Add upgrade tree
        // TODO: add tags
    }
}
