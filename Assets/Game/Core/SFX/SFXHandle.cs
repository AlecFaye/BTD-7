namespace Assets.Game.Core.SFX
{
    public readonly struct SFXHandle
    {
        private readonly SFXInstance _instance;
        private readonly int _generation;

        internal SFXHandle(SFXInstance instance)
        {
            _instance = instance;
            _generation = instance.Generation;
        }

        public bool IsPlaying => _instance != null && _instance.Generation == _generation && _instance.IsPlaying;

        public void Stop()
        {
            if (IsPlaying)
                _instance.Stop();
        }
    }
}
