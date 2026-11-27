using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.UI
{
    public class ScreenManager : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Transform _screenRoot;         // Usually your Canvas
        [SerializeField] private UIScreen[] _screenPrefabs;     // One prefab per screen type
        [SerializeField] private UIScreen _initialScreen;       // Prefab to show on start (optional)

        private readonly Stack<UIScreen> _stack = new();
        private readonly Dictionary<Type, UIScreen> _prefabLookup = new();
        private readonly Dictionary<Type, UIScreen> _instances = new();

        public int Depth => _stack.Count;
        public UIScreen Current => _stack.Count > 0 ? _stack.Peek() : null;

        private void Awake()
        {
            foreach (UIScreen prefab in _screenPrefabs)
                _prefabLookup[prefab.GetType()] = prefab;
        }

        private void Start()
        {
            if (_initialScreen != null)
                Push(_initialScreen.GetType());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Back();
        }

        public T Push<T>(object args = null) where T : UIScreen => (T)Push(typeof(T), args);

        public UIScreen Push(Type type, object args = null)
        {
            UIScreen screen = GetOrCreate(type);
            if (screen == null) 
                return null;

            if (_stack.Contains(screen))
            {
                Debug.LogWarning($"{type.Name} is already on the stack.");
                return screen;
            }

            if (_stack.Count > 0)
            {
                UIScreen previous = _stack.Peek();
                previous.SetInteractable(false);
                previous.OnPause();
            }

            _stack.Push(screen);
            screen.transform.SetAsLastSibling();

            RefreshVisibility();

            screen.OnEnter(args);
            return screen;
        }

        /// <summary>Close the top screen and resume the one beneath it.</summary>
        public void Pop()
        {
            if (_stack.Count == 0) 
                return;

            UIScreen top = _stack.Pop();
            top.OnExit();
            top.SetVisible(false);

            RefreshVisibility();

            if (_stack.Count > 0)
                _stack.Peek().OnResume();
        }

        /// <summary>Replace the top screen (e.g. Login -> MainMenu) without resuming the one beneath.</summary>
        public T Replace<T>(object args = null) where T : UIScreen
        {
            if (_stack.Count > 0)
            {
                UIScreen top = _stack.Pop();
                top.OnExit();
                top.SetVisible(false);
            }

            return Push<T>(args);
        }

        /// <summary>Pop everything except the bottom screen.</summary>
        public void PopToRoot()
        {
            while (_stack.Count > 1)
            {
                UIScreen top = _stack.Pop();
                top.OnExit();
                top.SetVisible(false);
            }

            RefreshVisibility();

            if (_stack.Count > 0)
                _stack.Peek().OnResume();
        }

        /// <summary>Back button handler: lets the screen consume it first.</summary>
        public void Back()
        {
            if (_stack.Count == 0) return;
            if (_stack.Peek().OnBackPressed()) return;
            if (_stack.Count > 1) Pop();
        }

        private UIScreen GetOrCreate(Type type)
        {
            if (_instances.TryGetValue(type, out UIScreen existing))
                return existing;

            if (!_prefabLookup.TryGetValue(type, out UIScreen prefab))
            {
                Debug.LogError($"No prefab registered for {type.Name}.");
                return null;
            }

            UIScreen instance = Instantiate(prefab, _screenRoot);
            instance.Initialize(this);
            _instances[type] = instance;
            return instance;
        }

        private void RefreshVisibility()
        {
            bool covered = false;
            bool isTop = true;

            foreach (UIScreen screen in _stack)  
            {
                screen.SetVisible(!covered);
                screen.SetInteractable(isTop);

                if (!screen.IsOverlay) 
                    covered = true;
                isTop = false;
            }
        }
    }
}
