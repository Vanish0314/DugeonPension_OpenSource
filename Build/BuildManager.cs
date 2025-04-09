using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon;
using Dungeon.GridSystem;
using GameFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dungeon
{
    public class BuildManager : MonoBehaviour
    {
        [SerializeField] public InputReader inputReader;
        private Vector3 m_CurrentMouseWorldPos;

        [SerializeField] private GridSystem.GridSystemTemp gridSystem;
        
        // 存储所有建筑的BuildingData
        public BuildingData[] buildingDatas;
        
        private BuildingData m_SelectedBuildingData;
        private GameObject m_PreviewInstance;

        // 建造完成事件
        public event Action<BuildingData> OnBuildCompleted;
        
        // 建造资源缺乏事件 --------------------------------加不加参数再说
        public event Action CannotAfford;
        private void OnEnable()
        {
            // 通过Resources.Load加载 BuildingData （暂时）
            buildingDatas = Resources.LoadAll<BuildingData>("Prefabs");
            GameFrameworkLog.Info(buildingDatas);
        }
        
        private void Update()
        {
                // 更新预览位置
                UpdatePreview();
        }

        public void DisableGridSystemTemp()
        {
            gridSystem.gameObject.SetActive(false);
        }
        private void OnMouseMoved(Vector2 mouseScreenPos)
        {
            // 将屏幕坐标转换为世界坐标
            m_CurrentMouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            m_CurrentMouseWorldPos.z = 0; // 确保z坐标为0
        }
        
        // 通过ID获取BuildingData
        public BuildingData GetBuildingData(string buildingId)
        {
            foreach (var data in buildingDatas)
            {
                if (data.buildingId == buildingId)
                {
                    OnBuildingSelected(data);
                    return data;
                }
            }
            Debug.LogError($"BuildingData not found: {buildingId}");
            return null;
        }

        private void OnBuildingSelected(BuildingData data)
        {
            // 取消当前选择
            if (m_SelectedBuildingData != null)
                Destroy(m_PreviewInstance);

            m_SelectedBuildingData = data;

            // 订阅点击事件
            inputReader.OnMouseMoveEvent += OnMouseMoved;
            inputReader.OnLeftPressBeginEvent += TryPlaceBuilding;
            inputReader.OnRightPressBeginEvent += CancelBuilding;
            
            // 创建半透明预览
            m_PreviewInstance = Instantiate(data.buildingPrefab);
            m_PreviewInstance.GetComponent<ProducingBuildingBase>().enabled = false;
            var renderer = m_PreviewInstance.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.color *= new Color(1, 1, 1, 0.3f); // 半透明效果
        }

        // 计算建筑中心偏移量（单位：格子数）
        private Vector2 GetBuildingCenterOffset(Vector2Int buildingSize)
        {
            // 奇数列：不需要水平偏移 (1,3,5...)
            // 偶数列：需要偏移0.5格 (2,4,6...)
            float offsetX = (buildingSize.x % 2 == 0) ? 0.5f : 0f;
            float offsetY = (buildingSize.y % 2 == 0) ? 0.5f : 0f;
    
            return new Vector2(offsetX, offsetY);
        }
        
        // 计算建筑左下角原点（网格坐标）
        private Vector2Int GetBuildingOrigin(Vector2Int centerGridPos, Vector2Int buildingSize)
        {
            int offsetX = (buildingSize.x - 1) / 2;
            int offsetY = (buildingSize.y - 1) / 2;
    
            return new Vector2Int(
                centerGridPos.x - offsetX,
                centerGridPos.y - offsetY
            );
        }

        // 检查资源是否足够
        private bool CanAffordBuilding(BuildingData buildingData)
        {
            return ResourceModel.Instance.Gold >= buildingData.cost.gold &&
                   ResourceModel.Instance.Stone >= buildingData.cost.stone &&
                   ResourceModel.Instance.MagicPower >= buildingData.cost.magicPower &&
                   ResourceModel.Instance.Material >= buildingData.cost.material;
        }
        private void TryPlaceBuilding()
        {
            if (m_SelectedBuildingData == null) return;
            
            if (!CanAffordBuilding(m_SelectedBuildingData))
            {
                CannotAfford?.Invoke();
                Debug.Log("资源不足，无法建造");
                return;
            }
            
            Vector2Int gridPos = gridSystem.WorldToGridPosition(m_CurrentMouseWorldPos);
            Vector2Int gridOriginPos = GetBuildingOrigin(gridPos, m_SelectedBuildingData.size);
            
            if (gridSystem.CanBuildAt(gridOriginPos, m_SelectedBuildingData))
            {
                // 写入 buildGrid 逻辑
                gridSystem.TryPlaceBuilding(gridOriginPos, m_SelectedBuildingData);
                
                // 计算精确的居中位置
                Vector2 centerOffset = GetBuildingCenterOffset(m_SelectedBuildingData.size);
            
                Vector3 worldCenterPos = gridSystem.GridToWorldPosition(gridPos) + 
                                         new Vector3(
                                             centerOffset.x * GridProperties.cellSize,
                                             centerOffset.y * GridProperties.cellSize,
                                             0);
                
                // 实例化建筑（精确居中）
                GameObject building = Instantiate(
                    m_SelectedBuildingData.buildingPrefab,
                    worldCenterPos,
                    Quaternion.identity
                );
                
                // 触发事件（传递BuildingData而非名字）
                OnBuildCompleted?.Invoke(m_SelectedBuildingData);

                // 重置状态
                Destroy(m_PreviewInstance);
                m_SelectedBuildingData = null;
            }
        }

        private void UpdatePreview()
        {
            if (m_SelectedBuildingData == null || m_PreviewInstance == null) return;
            
            Vector2Int gridPos = gridSystem.WorldToGridPosition(m_CurrentMouseWorldPos);

            // 计算精确的居中位置
            Vector2 centerOffset = GetBuildingCenterOffset(m_SelectedBuildingData.size);
            Vector3 worldCenterPos = gridSystem.GridToWorldPosition(gridPos) + 
                                     new Vector3(
                                         centerOffset.x * GridProperties.cellSize,
                                         centerOffset.y * GridProperties.cellSize,
                                         0);

            m_PreviewInstance.transform.position = worldCenterPos;
        }

        private void CancelBuilding()
        {
            inputReader.OnMouseMoveEvent -= OnMouseMoved;
            inputReader.OnLeftPressBeginEvent -= TryPlaceBuilding;
            inputReader.OnRightPressBeginEvent -= CancelBuilding;
            Destroy(m_PreviewInstance);
            m_SelectedBuildingData = null;
        }
    }
}

