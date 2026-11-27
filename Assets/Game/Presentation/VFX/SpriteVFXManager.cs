using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Presentation.VFX
{
    [DefaultExecutionOrder(100)]
    public sealed class SpriteVFXManager : MonoBehaviour
    {
        public static SpriteVFXManager Instance { get; private set; }

        [SerializeField] private Material _defaultMaterial;
        [SerializeField] private SpriteVFXDefinition[] _prewarmDefinitions;
        [SerializeField] private bool _dontDestroyOnLoad = true;

        private readonly Dictionary<SpriteVFXDefinition, SpriteVFXPool> _pools = new();
        private readonly List<SpriteVFXPool> _poolList = new();

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
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;

            for (int i = 0; i < _poolList.Count; i++)
                _poolList[i].Tick(deltaTime, unscaledDeltaTime);
        }

        public SpriteVFXHandle Play(
            SpriteVFXDefinition definition, 
            Vector3 position, 
            float rotationZ = 0f, 
            float scale = 1f, 
            Transform follow = null)
        {
            if (definition == null || definition.Frames == null || definition.Frames.Length == 0)
            {
                Debug.LogWarning("VFXManager.Play: definition is null or has no frames.", definition);
                return default;
            }

            SpriteVFXInstance instance = GetPool(definition).Spawn(position, rotationZ, scale, follow);
            return instance != null 
                ? new SpriteVFXHandle(instance) 
                : default;
        }

        public void StopAll(SpriteVFXDefinition definition = null)
        {
            if (definition != null)
            {
                if (_pools.TryGetValue(definition, out SpriteVFXPool pool)) 
                    pool.StopAll();
                return;
            }

            for (int i = 0; i < _poolList.Count; i++)
                _poolList[i].StopAll();
        }

        private SpriteVFXPool GetPool(SpriteVFXDefinition definition)
        {
            if (_pools.TryGetValue(definition, out SpriteVFXPool pool)) 
                return pool;

            Material mat = definition.MaterialOverride != null ? definition.MaterialOverride : _defaultMaterial;
            pool = new(definition, transform, mat);
            pool.Prewarm();

            _pools.Add(definition, pool);
            _poolList.Add(pool);

            return pool;
        }
    }
}
