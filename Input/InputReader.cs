using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon.Evnents;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "New InputReader",menuName ="InputReader")]
    public class InputReader : ScriptableObject, GameInput.IUIActions, GameInput.IPlaceActions
    {
        private GameInput m_GameInput;
        [SerializeField] private Camera m_MainCamera;
        
        private bool m_isBuilding;
        private void OnEnable()
        {
            // 初始化输入系统
            if (m_GameInput == null)
            {
                m_GameInput = new GameInput();
                m_GameInput.UI.SetCallbacks(this);
                m_GameInput.Place.SetCallbacks(this);
                m_MainCamera = Camera.main;
                SetUIActions();
            }
        }

        public void Subscribe()
        {
            // 订阅场景加载事件
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnSwitchedToMetroplisProcedureEvent.EventId,OnSceneLoaded);
        }

        private void OnDisable()
        {
            // 取消订阅事件，避免内存泄漏
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnSwitchedToMetroplisProcedureEvent.EventId,OnSceneLoaded);

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
        private void OnSceneLoaded(object sender, GameEventArgs gameEventArgs)
        {
            // 每次场景加载时重置状态
            m_MainCamera = Camera.main;
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
        
        public event Action OnOpenTargetUIEvent;
        public event Action OnCloseTargetUIEvent;
        
        // 新增摄像机控制事件
        public event Action<Vector2> OnCameraMoveEvent;
        public event Action OnCameraMoveEndEvent;
        public event Action<float> OnCameraZoomEvent;
        
        // 经营场景事件
        public event Action<GameObject> OnBuildingClickedEvent;
        public event Action<GameObject> OnHeroClickedEvent;
        public event Action OnNoBuildingClickedEvent;
        public event Action OnNoHeroClickedEvent;
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
                DetectClickedObject();
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
                    OnCloseTargetUIEvent?.Invoke();
                else
                    OnOpenTargetUIEvent?.Invoke();
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

        private void DetectClickedObject()
        {
            // 检查是否点击在UI上
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 进一步确认是否真的是UI
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Pointer.current.position.ReadValue();
        
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
        
                bool isRealUI = results.Any(r => r.gameObject.GetComponent<Graphic>() != null);
        
                if(isRealUI) {
                    Debug.Log("点击在真实UI上");
                    return;
                }
            }
            
            if (m_MainCamera == null )
                return;
            
            Vector2 clickPos = Mouse.current.position.ReadValue();
            Vector2 worldPos = m_MainCamera.ScreenToWorldPoint(clickPos);

            // 获取所有2D碰撞体（优化GC）
            RaycastHit2D[] hits = new RaycastHit2D[10];
            int hitCount = Physics2D.RaycastNonAlloc(worldPos, Vector2.zero, hits, 
                Mathf.Infinity);

            // 点击空白区域
            if (hitCount == 0)
            {
                OnNoBuildingClickedEvent?.Invoke(); 
                OnNoHeroClickedEvent?.Invoke();
                return;
            }


            // 找出最顶层的物体
            RaycastHit2D topHit = hits[0];
            float topPriority = GetRenderPriority(topHit.collider);

            for (int i = 1; i < hitCount; i++)
            {
                float currentPriority = GetRenderPriority(hits[i].collider);
                if (currentPriority > topPriority)
                {
                    topHit = hits[i];
                    topPriority = currentPriority;
                }
            }

            // 触发事件
            GameObject clickedObj = topHit.collider.gameObject;
            if (clickedObj.layer == LayerMask.NameToLayer("MetropolisBuilding"))
            {
                OnBuildingClickedEvent?.Invoke(clickedObj);
                OnNoHeroClickedEvent?.Invoke();
            }
            else if (clickedObj.layer == LayerMask.NameToLayer("MetropolisHero"))
            {
                OnHeroClickedEvent?.Invoke(clickedObj);
                OnNoBuildingClickedEvent?.Invoke();
            }
            else
            {
                OnNoBuildingClickedEvent?.Invoke();
                OnNoHeroClickedEvent?.Invoke();
            }
        }

        // 综合计算渲染优先级（值越大越靠前）
        private float GetRenderPriority(Collider2D collider)
        {
            const float Y_SCALE = 0.01f; // Y轴权重系数
    
            // 基础值：sortingOrder * 1000（确保order是主要因素）
            float priority = GetSortingOrder(collider) * 1000f;
    
            // 叠加Y轴影响：Y越小，附加值越大（模拟2D透视）
            priority += (1f - collider.transform.position.y * Y_SCALE);
    
            return priority;
        }

        // 获取物体的sortingOrder
        private int GetSortingOrder(Collider2D collider)
        {
            var sr = collider.GetComponent<SpriteRenderer>();
            return sr != null ? sr.sortingOrder : 0;
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
                // 检查是否点击在UI上
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
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
