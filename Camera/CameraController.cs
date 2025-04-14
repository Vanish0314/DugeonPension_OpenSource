using System;
using UnityEngine;

namespace Dungeon
{
    public class CameraController : MonoBehaviour
    {
        [Header("Base Settings")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private float moveSpeed = 15f;
        [SerializeField] private float zoomSpeed = 0.25f;
        
        [Header("Zoom Bounds")]
        [SerializeField] private float minZoom = 1f;
        [SerializeField] private float maxZoom = 5f;

        [Header("Move Bounds")] 
        [SerializeField] private Vector2 minBounds = new Vector2(-10, -10);
        [SerializeField] private Vector2 maxBounds = new Vector2(10, 10);

        private Camera m_MainCamera;
        private bool m_IsMoving = false;
        private Vector3 m_Dir;
        
        private void Awake()
        {
            m_MainCamera = this.GetComponent<Camera>();
            inputReader = Resources.Load<InputReader>("InputReader");
        }

        private void Update()
        {
            if (m_IsMoving)
            {
                transform.position += m_Dir * (moveSpeed * Time.deltaTime);
                // 添加边界限制
                ClampCameraPosition();
            }
        }

        private void OnEnable()
        {
            inputReader.OnCameraMoveEvent += HandleCameraMove;
            inputReader.OnCameraMoveEndEvent += HandleCameraMoveEnd;
            inputReader.OnCameraZoomEvent += HandleCameraZoom;
        }
        

        private void OnDisable()
        {
            inputReader.OnCameraMoveEvent -= HandleCameraMove;
            inputReader.OnCameraMoveEndEvent -= HandleCameraMoveEnd;
            inputReader.OnCameraZoomEvent -= HandleCameraZoom;
        }

        private void HandleCameraMove(Vector2 moveDirection)
        {
            m_IsMoving = true;
            m_Dir = new Vector3(moveDirection.x, moveDirection.y, 0);
        }
        
        private void HandleCameraMoveEnd()
        {
            m_IsMoving = false;
            m_Dir = Vector3.zero;
        }

        private void HandleCameraZoom(float zoomInput)
        {
            float zoomDelta = -zoomInput; // 反转滚动方向
            m_MainCamera.orthographicSize = Mathf.Clamp(
                m_MainCamera.orthographicSize + zoomDelta * zoomSpeed * Time.deltaTime,
                minZoom,
                maxZoom
            );
            ClampCameraPosition();
        }
        
        // 核心边界限制方法
        private void ClampCameraPosition()
        {
            // 计算摄像机视口尺寸
            float verticalSize = m_MainCamera.orthographicSize;
            float horizontalSize = verticalSize * m_MainCamera.aspect;

            // 计算有效边界
            float minX = minBounds.x + horizontalSize;
            float maxX = maxBounds.x - horizontalSize;
            float minY = minBounds.y + verticalSize;
            float maxY = maxBounds.y - verticalSize;

            // 确保边界有效性
            if (minX > maxX) (minX, maxX) = (maxX, minX);
            if (minY > maxY) (minY, maxY) = (maxY, minY);

            // 限制位置
            Vector3 clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
            clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);
            transform.position = clampedPos;
        }
        
#if UNITY_EDITOR
        // 简单边界可视化
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(
                (Vector3)(minBounds + maxBounds) / 2,
                new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 1)
            );
        }
#endif
    }
}
