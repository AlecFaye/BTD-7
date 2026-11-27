using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.UI
{
    public class ConfirmDialog : UIScreen
    {
        public readonly struct Args
        {
            public readonly string Message;
            public readonly System.Action OnYes;

            public Args(string message, System.Action onYes) 
            { 
                Message = message; 
                OnYes = onYes; 
            }
        }

        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        private System.Action _onYes;

        private void Start()
        {
            _yesButton.onClick.AddListener(() => 
            { 
                Manager.Pop(); 
                _onYes?.Invoke(); 
            });

            _noButton.onClick.AddListener(Manager.Pop);
        }

        public override void OnEnter(object args)
        {
            Args a = (Args)args;
            _messageText.text = a.Message;
            _onYes = a.OnYes;
        }
    }
}
