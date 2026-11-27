using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Core.SFX
{
    internal sealed class SFXPool
    {
        private readonly SFXDefinition _definition;
        private readonly Transform _root;

        private readonly Stack<SFXInstance> _inactive = new();
        private readonly List<SFXInstance> _active = new();

        private int _created;

        public SFXPool(SFXDefinition definition, Transform root)
        {
            _definition = definition;
            _root = root;
        }

        public void Prewarm()
        {
            int target = Mathf.Min(_definition.Prewarm, _definition.MaxInstances);

            while (_created < target)
                _inactive.Push(Create());
        }

        private SFXInstance Create()
        {
            GameObject go = new(_definition.name);
            go.transform.SetParent(_root, false);

            AudioSource audioSource = go.AddComponent<AudioSource>();

            _created++;

            return new SFXInstance(audioSource);
        }

        public SFXInstance Spawn()
        {
            SFXInstance instance;

            if (_inactive.Count > 0)
            {
                instance = _inactive.Pop();
            }
            else if (_created < _definition.MaxInstances)
            {
                instance = Create();
            }
            else if (_definition.Overflow == SFXOverflowPolicy.RecycleOldest && _active.Count > 0)
            {
                instance = _active[0];
                _active.RemoveAt(0);
                instance.ResetForPool();
            }
            else
            {
                return null;
            }

            instance.Begin(_definition);
            _active.Add(instance);
            return instance;
        }

        public void Tick()
        {
            int count = _active.Count;
            int write = 0;

            for (int read = 0; read < count; read++)
            {
                SFXInstance instance = _active[read];
                if (instance.Tick())
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
