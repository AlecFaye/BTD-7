using UnityEngine;

namespace Assets.Game.Presentation.VFX
{
    public readonly struct SpriteVFXHandle
    {
        private readonly SpriteVFXInstance _instance;
        private readonly int _generation;

        internal SpriteVFXHandle(SpriteVFXInstance instance)
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

        public void Follow(Transform target)
        {
            if (IsPlaying) 
                _instance.SetFollow(target, _instance.Transform.position);
        }

        public void Flip()
        {
            Vector3 localScale = _instance.Transform.localScale;
            localScale.x = -localScale.x;
            _instance.Transform.localScale = localScale;
        }
    }
}
