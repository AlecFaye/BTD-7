using UnityEngine;

namespace Assets.Game.Core.VFX
{
    public enum VFXOverflowPolicy
    {
        RecycleOldest,
        DropNew,
    }

    [CreateAssetMenu(fileName = "Sprite VFX", menuName = "VFX/Sprite VFX Definition")]
    public sealed class SpriteVFXDefinition : ScriptableObject
    {
        [Header("Animation")]
        [SerializeField] private Sprite[] _frames;
        [SerializeField, Min(1f)] private float _framesPerSecond = 12f;
        [SerializeField] private bool _loop;
        [Tooltip("Loop mode only. 0 = loop until Stop() is called.")]
        [SerializeField] private float _loopDuration;
        [SerializeField] private bool _useUnscaledTime;

        public Sprite[] Frames => _frames;
        public float FramesPerSecond => _framesPerSecond;
        public bool Loop => _loop;
        public float LoopDuration => _loopDuration;
        public bool UseUnscaledTime => _useUnscaledTime;

        [Header("Rendering")]
        [SerializeField] private string _sortingLayerName = "Default";
        [SerializeField] private int _sortingOrder;
        [SerializeField] private Material _materialOverride;
        [SerializeField] private Gradient _colourOverLifetime = new();

        public string SortingLayerName => _sortingLayerName;
        public int SortingOrder => _sortingOrder;
        public Material MaterialOverride => _materialOverride;
        public Gradient ColourOverLifetime => _colourOverLifetime;

        [Header("Variation")]
        [SerializeField] private Vector2 _scaleRange = Vector2.one;
        [SerializeField] private Vector2 _randomRotationRange = Vector2.one;
        [SerializeField] private bool _randomFlipX;
        [SerializeField] private bool _randomFlipY;

        public Vector2 ScaleRange => _scaleRange;
        public Vector2 RandomRotationRange => _randomRotationRange;
        public bool RandomFlipX => _randomFlipX;
        public bool RandomFlipY => _randomFlipY;

        [Header("Pool")]
        [SerializeField, Min(0)] private int _prewarm = 8;
        [SerializeField, Min(1)] private int _maxInstances = 64;
        [SerializeField] private VFXOverflowPolicy _overflow = VFXOverflowPolicy.RecycleOldest;

        public int Prewarm => _prewarm;
        public int MaxInstances => _maxInstances;
        public VFXOverflowPolicy Overflow => _overflow;

        private void OnValidate()
        {
            if (_maxInstances < 1) _maxInstances = 1;
            if (_prewarm > _maxInstances) _prewarm = _maxInstances;
            if (_scaleRange.y < _scaleRange.x) _scaleRange.y = _scaleRange.x;
            if (_randomRotationRange.y < _randomRotationRange.x) _randomRotationRange.y = _randomRotationRange.x;
        }
    }
}
