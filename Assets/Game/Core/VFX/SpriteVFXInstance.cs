using UnityEngine;

namespace Assets.Game.Core.VFX
{
    internal sealed class SpriteVFXInstance
    {
        public readonly Transform Transform;

        public Transform FollowTarget { get; private set; }
        public int Generation { get; private set; }
        public bool IsPlaying => _isActive;

        private readonly SpriteRenderer _renderer;

        private SpriteVFXDefinition _definition;
        private bool _isActive;
        private Vector3 _followOffset;
        private float _elapsed;
        private int _frame;

        public SpriteVFXInstance(Transform transform, SpriteRenderer renderer)
        {
            Transform = transform;
            _renderer = renderer;
        }

        public void Begin(
            SpriteVFXDefinition definition, 
            Vector3 position, 
            float rotationZ, 
            float scale, 
            Transform follow)
        {
            _definition = definition;
            Generation++;
            _isActive = true;
            _elapsed = 0f;
            _frame = 0;

            SetFollow(follow, position);

            float rotation = rotationZ + Random.Range(definition.RandomRotationRange.x, definition.RandomRotationRange.y);
            Transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 0f, rotation));

            float randomScale = scale * Random.Range(definition.ScaleRange.x, definition.ScaleRange.y);
            Transform.localScale = new Vector3(randomScale, randomScale, 1f);

            _renderer.flipX = definition.RandomFlipX && Random.value < 0.5f;
            _renderer.flipY = definition.RandomFlipY && Random.value < 0.5f;
            _renderer.sprite = definition.Frames[0];
            _renderer.color = definition.ColourOverLifetime.Evaluate(0f);
            _renderer.enabled = true;
        }

        public void SetFollow(Transform target, Vector3 currentWorldPosition)
        {
            FollowTarget = target;
            _followOffset = target != null ? currentWorldPosition - target.position : Vector3.zero;
        }

        public bool Tick(float deltaTime)
        {
            if (!_isActive) 
                return false;

            if (FollowTarget != null)
                Transform.position = FollowTarget.position + _followOffset;

            _elapsed += deltaTime;

            SpriteVFXDefinition definition = _definition;
            Sprite[] frames = definition.Frames;

            int count = frames.Length;
            int frame = (int)(_elapsed * definition.FramesPerSecond);
            float time;

            if (definition.Loop)
            {
                if (definition.LoopDuration > 0f && _elapsed >= definition.LoopDuration) 
                    return false;

                frame %= count;

                time = definition.LoopDuration > 0f ? _elapsed / definition.LoopDuration : 0f;
            }
            else
            {
                if (frame >= count) return false;
                time = _elapsed / (count / definition.FramesPerSecond);
            }

            if (frame != _frame)
            {
                _frame = frame;
                _renderer.sprite = frames[frame];
            }

            _renderer.color = definition.ColourOverLifetime.Evaluate(time);

            return true;
        }

        public void Stop()
        {
            if (!_isActive) 
                return;
            _isActive = false;

            _renderer.enabled = false;
        }

        public void ResetForPool()
        {
            _isActive = false;
            FollowTarget = null;
            _definition = null;
            _renderer.enabled = false;
            _renderer.sprite = null;
        }
    }
}
