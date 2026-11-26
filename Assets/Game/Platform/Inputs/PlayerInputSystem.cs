using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Game.Platform.Inputs
{
    public class PlayerInputSystem : MonoBehaviour
    {
        public event EventHandler OnSelected = delegate { };

        public Vector2 Move { get; private set; }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
                Move = context.ReadValue<Vector2>();
            else if (context.canceled)
                Move = Vector2.zero;

        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.canceled)
                OnSelected?.Invoke(this, EventArgs.Empty);
        }
    }
}
