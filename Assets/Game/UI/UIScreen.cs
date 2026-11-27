using UnityEngine;

namespace Assets.Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreen : MonoBehaviour
    {
        [Tooltip("Overlay screens (dialogs, popups) leave the screen beneath them visible.")]
        [SerializeField] private bool _isOverlay;

        private CanvasGroup _canvasGroup;

        public bool IsOverlay => _isOverlay;
        protected ScreenManager Manager { get; private set; }

        internal void Initialize(ScreenManager manager)
        {
            Manager = manager;
            _canvasGroup = GetComponent<CanvasGroup>();
            SetVisible(false);
        }

        /// <summary>Screen was pushed onto the stack.</summary>
        public virtual void OnEnter(object args) { }

        /// <summary>Another screen was pushed on top of this one.</summary>
        public virtual void OnPause() { }

        /// <summary>The screen above was popped; this one is on top again.</summary>
        public virtual void OnResume() { }

        /// <summary>Screen was popped off the stack.</summary>
        public virtual void OnExit() { }

        /// <summary>Back button / Escape. Return true if you handled it yourself.</summary>
        public virtual bool OnBackPressed() => false;

        internal void SetVisible(bool visible)
        {
            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.blocksRaycasts = visible;
            gameObject.SetActive(visible);
        }

        internal void SetInteractable(bool interactable)
        {
            _canvasGroup.interactable = interactable;
            _canvasGroup.blocksRaycasts = interactable || IsOverlay == false;
        }
    }
}
