using UnityEngine;

namespace Assets.Game.Presentation.SFX
{
    public enum SFXOverflowPolicy
    { 
        RecycleOldest,
        DropNew,
    }

    [CreateAssetMenu(fileName = "SFX", menuName = "SFX/SFX Definition")]
    public class SFXDefinition : ScriptableObject
    {
        [Header("Audio Source")]
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private bool _bypassEffects;
        [SerializeField] private bool _bypassListenerEffects;
        [SerializeField] private bool _bypassReverbEffects;
        [SerializeField, Range(0, 256)] private int _priority = 128;
        [SerializeField, Range(-1, 1)] private float _stereoPan = 1;
        [SerializeField, Range(0, 1.1f)] private float _reverbZoneMix = 1;

        public AudioClip AudioClip => _audioClip;
        public bool BypassEffects => _bypassEffects;
        public bool BypassListenerEffects => _bypassListenerEffects;
        public bool BypassReverbEffects => _bypassReverbEffects;
        public int Priority => _priority;
        public float StereoPan => _stereoPan;
        public float ReverbZoneMix => _reverbZoneMix;

        [Header("Variation")]
        [SerializeField] private Vector2 _volumeRange = Vector2.one;
        [SerializeField] private Vector2 _pitchRange = Vector2.one;

        public Vector2 VolumeRange => _volumeRange;
        public Vector2 PitchRange => _pitchRange;

        [Header("Pool")]
        [SerializeField, Min(0)] private int _prewarm = 8;
        [SerializeField, Min(1)] private int _maxInstances = 64;
        [SerializeField] private SFXOverflowPolicy _overflow = SFXOverflowPolicy.RecycleOldest;

        public int Prewarm => _prewarm;
        public int MaxInstances => _maxInstances;
        public SFXOverflowPolicy Overflow => _overflow;

        private void OnValidate()
        {
            if (_maxInstances < 1) _maxInstances = 1;
            if (_prewarm > _maxInstances) _prewarm = _maxInstances;

            _volumeRange.x = Mathf.Clamp(_volumeRange.x, 0, 1);
            if (_volumeRange.y < _volumeRange.x) _volumeRange.y = _volumeRange.x;
            _volumeRange.y = Mathf.Clamp(_volumeRange.y, 0, 1);

            _pitchRange.x = Mathf.Clamp(_pitchRange.x, -3, 3);
            if (_pitchRange.y < _pitchRange.x) _pitchRange.y = _pitchRange.x;
            _pitchRange.y = Mathf.Clamp(_pitchRange.y, -3, 3);
        }
    }
}
