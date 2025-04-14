using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "New InputReader",menuName ="InputReader")]
    public class InputReader : ScriptableObject, GameInput.IUIActions, GameInput.IPlaceActions
    {
        private GameInput m_GameInput;

        private bool m_isBuilding;
        private void OnEnable()
        {
            // 初始化输入系统
            if (m_GameInput == null)
            {
                m_GameInput = new GameInput();
                m_GameInput.UI.SetCallbacks(this);
                m_GameInput.Place.SetCallbacks(this);
                SetUIActions();
            }

            // 订阅场景加载事件
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // 取消订阅事件，避免内存泄漏
            SceneManager.sceneLoaded -= OnSceneLoaded;

            // 清理输入系统
            if (m_GameInput != null)
            {
                m_GameInput.UI.Disable();
                m_GameInput.Place.Disable();
                m_GameInput = null;
            }
        }

        public void SetUIActions()
        {
            m_GameInput.UI.Enable();
            m_GameInput.Place.Disable();
        }

        public void SetPlaceActions()
        {
            m_GameInput.UI.Disable();
            m_GameInput.Place.Enable();
        }
        // 场景加载时的回调
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 每次场景加载时重置状态
            m_isBuilding = false;
        }
        
        /// <summary>
        /// ui 
        /// </summary>
        /// <param name="context"></param>
        public event Action<Vector2> OnMouseMoveEvent;

        public event Action OnLeftPressBeginEvent;
        public event Action OnLeftPressEndEvent;
        
        public event Action OnRightPressBeginEvent;
        
        public event Action OnBuildEvent;
        public event Action OnBuildEndEvent;
        
        // 新增摄像机控制事件
        public event Action<Vector2> OnCameraMoveEvent;
        public event Action OnCameraMoveEndEvent;
        public event Action<float> OnCameraZoomEvent;
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

        public void OnOpenOrCloseForm(InputAction.CallbackContext context)
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

        public void OnCameraZoom(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Vector2 scrollValue = context.ReadValue<Vector2>();
                OnCameraZoomEvent?.Invoke(scrollValue.y);
            }
        }

        public void OnCameraMove(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                // 持续获取移动方向
                OnCameraMoveEvent?.Invoke(context.ReadValue<Vector2>());
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                OnCameraMoveEndEvent?.Invoke();
            }
        }

        /// <summary>
        /// game
        /// </summary>
        /// <param name="context"></param>
                
        // 陷阱旋转事件
        public event Action<float> OnTrapRotateEvent;
        
        public event Action<Vector2> OnPlacePositionEvent;
        public event Action OnTryPlaceEvent;
        public event Action OnCancelPlaceEvent;
        public void OnRotateTrap(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Vector2 scrollValue = context.ReadValue<Vector2>();
                OnTrapRotateEvent?.Invoke(scrollValue.y);
            }
        }

        public void OnTryPlacePosition(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                OnPlacePositionEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnTryPlace(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                OnTryPlaceEvent?.Invoke();
            }
        }

        public void OnCancelPlace(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                OnCancelPlaceEvent?.Invoke();
            }
        }
    }
}
