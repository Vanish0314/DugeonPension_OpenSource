using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dungeon
{
    public class Quarry : MetropolisBuildingBase
    {
        [Header("UI Settings")] [SerializeField]
        private GameObject resourceUIPrefab; // UI预制体

        [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0); // UI显示偏移
        private GameObject currentUIInstance;

        // 点击检测（需要建筑有Collider）
        private void OnMouseDown()
        {
            if (!EventSystem.current.IsPointerOverGameObject()) // 排除点击UI的情况
            {
                ShowResourceUI();
            }
        }

        // 显示资源UI
        public void ShowResourceUI()
        {
            // 如果已有UI则关闭
            if (currentUIInstance != null)
            {
                Destroy(currentUIInstance);
                return;
            }

            // 创建UI实例
            currentUIInstance = Instantiate(resourceUIPrefab, transform.position + uiOffset, Quaternion.identity);
            currentUIInstance.transform.SetParent(transform); // 设为建筑子物体

            // 获取UI组件
            ResourceUI uiComponent = currentUIInstance.GetComponent<ResourceUI>();
            if (uiComponent != null)
            {
                uiComponent.Setup(this);
            }

            // 点击其他地方关闭UI
            StartCoroutine(CloseUIOnClickOutside());
        }

        private IEnumerator CloseUIOnClickOutside()
        {
            while (currentUIInstance != null)
            {
                if (Input.GetMouseButtonDown(0) &&
                    !RectTransformUtility.RectangleContainsScreenPoint(
                        currentUIInstance.GetComponent<RectTransform>(),
                        Input.mousePosition,
                        Camera.main))
                {
                    Destroy(currentUIInstance);
                }

                yield return null;
            }
        }
        
    }
}
