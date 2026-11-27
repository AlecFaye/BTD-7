using UnityEngine;

namespace Assets.Game.Presentation.SFX
{
    internal sealed class SFXInstance
    {
        public int Generation { get; private set; }
        public bool IsPlaying => _audioSource.isPlaying;

        private readonly AudioSource _audioSource;

        private SFXDefinition _definition;

        public SFXInstance(AudioSource audioSource)
        {
            _audioSource = audioSource;
        }

        public void Begin(SFXDefinition definition)
        {
            _definition = definition;

            Generation++;

            _audioSource.clip = _definition.AudioClip;
            _audioSource.bypassEffects = _definition.BypassEffects;
            _audioSource.bypassListenerEffects = _definition.BypassListenerEffects;
            _audioSource.bypassReverbZones = _definition.BypassReverbEffects;
            _audioSource.panStereo = _definition.StereoPan;
            _audioSource.priority = _definition.Priority;
            _audioSource.reverbZoneMix = _definition.ReverbZoneMix;

            _audioSource.volume = Random.Range(_definition.VolumeRange.x, _definition.VolumeRange.y);
            _audioSource.pitch = Random.Range(_definition.PitchRange.x, _definition.PitchRange.y);

            _audioSource.enabled = true;

            _audioSource.Play();
        }

        public bool Tick()
        {
            if (!_audioSource.enabled)
                return false;

            return IsPlaying;
        }

        public void Stop()
        {
            if (!IsPlaying)
                return;

            _audioSource.Stop();
        }

        public void ResetForPool()
        {
            if (IsPlaying)
                _audioSource.Stop();

            _definition = null;
            _audioSource.enabled = false;
        }
    }
}
