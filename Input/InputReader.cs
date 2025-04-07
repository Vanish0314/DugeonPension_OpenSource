using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "New InputReader",menuName ="InputReader")]
    public class InputReader : ScriptableObject, GameInput.IUIActions
    {
        private GameInput m_GameInput;

        private bool m_isBuilding;
        private void OnEnable()
        {
            m_isBuilding = false;
            if(m_GameInput == null)
            {
                m_GameInput = new GameInput();
                m_GameInput.UI.SetCallbacks(this);
                m_GameInput.UI.Enable();
            }
        }

        private void OnDisable()
        {
            if (m_GameInput != null)
            {
                m_GameInput.UI.Disable();
                m_GameInput = null;
            }
        }
        public event Action<Vector2> OnMouseMoveEvent;

        public event Action OnLeftPressBeginEvent;
        public event Action OnLeftPressEndEvent;
        
        public event Action OnRightPressBeginEvent;
        
        public event Action<float> OnScrollEvent;
        
        public event Action OnBuildEvent;
        public event Action OnBuildEndEvent;

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                OnMouseMoveEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                OnLeftPressBeginEvent?.Invoke();
            }

            if (context.phase == InputActionPhase.Canceled)
            {
                OnLeftPressEndEvent?.Invoke();
            }
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                OnRightPressBeginEvent?.Invoke();
            }
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                OnScrollEvent?.Invoke(context.ReadValue<Vector2>().y);
            }
        }

        public void OnB(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                if (m_isBuilding)
                    OnBuildEndEvent?.Invoke();
                else
                    OnBuildEvent?.Invoke();
            }

            if (context.phase == InputActionPhase.Canceled)
            {
                m_isBuilding = !m_isBuilding;
            }
        }
    }
}
