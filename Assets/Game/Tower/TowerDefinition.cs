using UnityEngine;

namespace Assets.Game.Tower
{
    public abstract class TowerDefinition : ScriptableObject
    {
        [Header("Cost")]
        [SerializeField, Min(0)] private int _cost;
        [SerializeField, Range(0.1f, 1f)] private float _sellRatio = 0.8f;

        public int Cost => _cost;
        public float SellRatio => _sellRatio;

        // TODO: Add damage profile
        // TODO: Add upgrade tree
        // TODO: add tags
    }
}
