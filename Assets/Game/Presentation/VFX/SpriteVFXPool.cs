using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Presentation.VFX
{
    internal sealed class SpriteVFXPool
    {
        private readonly SpriteVFXDefinition _definition;
        private readonly Transform _root;
        private readonly Material _material;
        private readonly int _sortingLayerId;

        private readonly Stack<SpriteVFXInstance> _inactive = new();
        private readonly List<SpriteVFXInstance> _active = new();

        private int _created;

        public SpriteVFXPool(SpriteVFXDefinition definition, Transform root, Material material)
        {
            _definition = definition;
            _root = root;
            _material = material;
            _sortingLayerId = SortingLayer.NameToID(definition.SortingLayerName);
        }

        public void Prewarm()
        {
            int target = Mathf.Min(_definition.Prewarm, _definition.MaxInstances);

            while (_created < target)
                _inactive.Push(Create());
        }

        private SpriteVFXInstance Create()
        {
            GameObject go = new(_definition.name);
            go.transform.SetParent(_root, false);

            SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
            if (_material != null) 
                spriteRenderer.sharedMaterial = _material;
            spriteRenderer.sortingLayerID = _sortingLayerId;
            spriteRenderer.sortingOrder = _definition.SortingOrder;
            spriteRenderer.enabled = false;

            _created++;
            return new SpriteVFXInstance(go.transform, spriteRenderer);
        }

        public SpriteVFXInstance Spawn(Vector3 position, float rotationZ, float scale, Transform follow)
        {
            SpriteVFXInstance instance;

            if (_inactive.Count > 0)
            {
                instance = _inactive.Pop();
            }
            else if (_created < _definition.MaxInstances)
            {
                instance = Create();
            }
            else if (_definition.Overflow == VFXOverflowPolicy.RecycleOldest && _active.Count > 0)
            {
                instance = _active[0];
                _active.RemoveAt(0);
                instance.ResetForPool();
            }
            else
            {
                return null;
            }

            instance.Begin(_definition, position, rotationZ, scale, follow);
            _active.Add(instance);
            return instance;
        }

        public void Tick(float deltaTime, float unscaledDeltaTime)
        {
            float step = _definition.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
            int count = _active.Count;
            int write = 0;

            for (int read = 0; read < count; read++)
            {
                SpriteVFXInstance instance = _active[read];
                if (instance.Tick(step))
                {
                    _active[write++] = instance;
                }
                else
                {
                    instance.ResetForPool();
                    _inactive.Push(instance);
                }
            }

            if (write < count)
                _active.RemoveRange(write, count - write);
        }

        public void StopAll()
        {
            for (int i = 0; i < _active.Count; i++)
                _active[i].Stop();
        }
    }
}
