using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Presentation.SFX
{
    [DefaultExecutionOrder(100)]
    public class SFXManager : MonoBehaviour
    {
        public static SFXManager Instance { get; private set; }

        [SerializeField] private SFXDefinition[] _prewarmDefinitions;
        [SerializeField] private bool _dontDestroyOnLoad = true;

        private readonly Dictionary<SFXDefinition, SFXPool> _pools = new();
        private readonly List<SFXPool> _poolList = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (_dontDestroyOnLoad)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

            if (_prewarmDefinitions != null)
            {
                for (int i = 0; i < _prewarmDefinitions.Length; i++)
                {
                    if (_prewarmDefinitions[i] != null)
                        GetPool(_prewarmDefinitions[i]);
                }
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _poolList.Count; i++)
                _poolList[i].Tick();
        }

        public SFXHandle Play(SFXDefinition definition)
        {
            if (definition == null)
            {
                Debug.LogWarning("SFXManager.Play: definition is null.", definition);
                return default;
            }

            SFXInstance instance = GetPool(definition).Spawn();

            return instance != null
                ? new SFXHandle(instance)
                : default;
        }

        public void StopAll(SFXDefinition definition = null)
        {
            if (definition != null)
            {
                if (_pools.TryGetValue(definition, out SFXPool pool))
                    pool.StopAll();
                return;
            }

            for (int i = 0; i < _poolList.Count; i++)
                _poolList[i].StopAll();
        }

        private SFXPool GetPool(SFXDefinition definition)
        {
            if (_pools.TryGetValue(definition, out SFXPool pool))
                return pool;

            pool = new(definition, transform);
            pool.Prewarm();

            _pools.Add(definition, pool);
            _poolList.Add(pool);

            return pool;
        }
    }
}
