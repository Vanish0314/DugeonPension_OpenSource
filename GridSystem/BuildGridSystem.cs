using System;
using System.Collections.Generic;
using Dungeon.Common.MonoPool;
using Dungeon.DungeonEntity.Monster;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityGameFramework.Runtime;

namespace Dungeon.GridSystem
{
    [RequireComponent(typeof(Grid))]
    public class BuildGridSystem : MonoBehaviour
    {
        private static BuildGridSystem _instance;
        
        [SerializeField] private GridProperties properties;
        [SerializeField] private BuildingGrid m_BuildingGrid;
        
        private Grid m_Grid;
        [SerializeField] private Tilemap m_CurrentTilemap;

        public static BuildGridSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<BuildGridSystem>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("BuildGridSystem");
                        _instance = obj.AddComponent<BuildGridSystem>();
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            
            m_Grid = GetComponent<Grid>();
            m_BuildingGrid = GetComponent<BuildingGrid>();
        }

        private void Start()
        {
            InitializeForCurrentScene();
        }

        private void OnEnable()
        {
            GameEntry.Event.Subscribe(TryPlaceBuildingEventArgs.EventId,HandleTryPlaceBuildingEventArgs);
        }

        private void OnDisable()
        {
           GameEntry.Event.Unsubscribe(TryPlaceBuildingEventArgs.EventId,HandleTryPlaceBuildingEventArgs);
        }
        
        private void InitializeForCurrentScene()
        {
            if (m_CurrentTilemap != null)
            {
                m_CurrentTilemap.gameObject.SetActive(true);
                InitializeGridSystem();
                return;
            }
        }

        private void InitializeGridSystem()
        {
            // 初始化网格属性
            properties = new GridProperties
            {
                width = m_CurrentTilemap.size.x,
                height = m_CurrentTilemap.size.y,
                originPoint = m_CurrentTilemap.origin
            };

            // 初始化建筑网格 
            m_BuildingGrid.Init(m_CurrentTilemap);
        }
        
        private void HandleTryPlaceBuildingEventArgs(object sender, GameEventArgs gameEventArgs)
        {
            TryPlaceBuildingEventArgs args = (TryPlaceBuildingEventArgs)gameEventArgs;
            
            Vector3 worldPos = args.WorldPosition;
            MonoPoolItem buildingItem = args.BuildingItem;
            BuildingData buildingData = args.BuildingData;
            
            Vector2Int gridPos = WorldToGridPosition(worldPos);
            Vector3 cellLocalPos = GetCellLocalPosition(worldPos, gridPos);

            MetropolisBuildingBase buildingInstantiated = buildingItem.GetComponent<MetropolisBuildingBase>();

            if (TryPlaceBuilding(gridPos, cellLocalPos, buildingData, buildingInstantiated))
            {
                GameEntry.Event.Fire(this, OnBuildingPlacedEventArgs.Create(buildingData));
            }
            else
            {
                buildingItem.ReturnToPool();
            }
        }

        /// <summary>
        /// 尝试在指定位置放置建筑
        /// </summary>
        public bool TryPlaceBuilding(Vector2Int gridPos, Vector3 cellLocalPos, BuildingData buildingData, MetropolisBuildingBase buildingInstantiated)
        {
            Vector2Int originGridPos = GetBuildingOrigin(gridPos, buildingData.size, cellLocalPos);
            
            if (!CanBuildAt(originGridPos, buildingData)) return false;
            
            Vector3 centerWorldPosition = CalculateWorldPosition(gridPos, cellLocalPos, buildingData.size);

            // 更新建筑网格
            if (m_BuildingGrid.TryPlaceBuilding(originGridPos.x, originGridPos.y, buildingData))
            {
                buildingInstantiated.transform.position = centerWorldPosition;
                return true;
            }
            return false;
        }
        
        #region PUBLIC
        public void SetTile(Vector2Int gridPos, TileDesc tileDesc)
        {
            //--------------------
        }
        
        public Vector3 GridToWorldPosition(Vector2Int gridPos) => m_BuildingGrid.GridToWorldPosition(gridPos.x, gridPos.y);
        public Vector2Int WorldToGridPosition(Vector3 worldPosition) => m_BuildingGrid.WorldToGridPosition(worldPosition);

        #region 建造系统核心功能
        /// <summary>
        /// 检查指定位置是否可以建造
        /// </summary>
        public bool CanBuildAt(Vector2Int gridPos, BuildingData buildingData)
        {
            return m_BuildingGrid.CanBuildAt(gridPos.x, gridPos.y, buildingData);
        }
        
        
        /// <summary>
        /// 尝试拆除指定位置的建筑
        /// </summary>
        public bool TryRemoveBuilding(Vector2Int gridPos)
        {
            if (m_BuildingGrid.GetBuildingData(gridPos.x, gridPos.y) == null)
                return false;

            // 更新建筑网格
            if (m_BuildingGrid.TryRemoveBuilding(gridPos.x, gridPos.y))
            {
                return true;
            }
            return false;
        }
        
        // 预览位置
        public (Vector3 worldPos, bool isValid) CalculatePreviewPosition(
            Vector3 mouseWorldPos, 
            BuildingData buildingData)
        {
            // 转换为网格坐标
            Vector2Int gridPos = WorldToGridPosition(mouseWorldPos);
            Vector2 cellLocalPos = GetCellLocalPosition(mouseWorldPos, gridPos);
    
            // 计算建筑原点
            Vector2Int originGridPos = GetBuildingOrigin(gridPos, buildingData.size, cellLocalPos);
    
            // 检查是否可建造
            bool canBuild = CanBuildAt(originGridPos, buildingData);
    
            // 计算中心坐标
            Vector3 centerWorldPos = CalculateWorldPosition(gridPos, cellLocalPos, buildingData.size);
    
            return (centerWorldPos, canBuild);
        }
        
        private Vector2Int GetBuildingOrigin(Vector2Int clickGridPos, Vector2Int buildingSize, Vector2 cellLocalPos)
        {
            int offsetX = 0;
            int offsetY = 0;

            // 水平方向偏移判断
            if (buildingSize.x % 2 == 0) // 偶数列
            {
                offsetX = (cellLocalPos.x >= 0.5f) ? -(buildingSize.x - 1) / 2 : -buildingSize.x / 2;
            }
            else // 奇数列
            {
                offsetX = -(buildingSize.x - 1) / 2; // 保持中心对齐
            }

            // 垂直方向偏移判断
            if (buildingSize.y % 2 == 0) // 偶数行
            {
                offsetY = (cellLocalPos.y >= 0.5f) ? -(buildingSize.y - 1) / 2 : -buildingSize.y / 2;
            }
            else // 奇数行
            {
                offsetY = -(buildingSize.y - 1) / 2; // 保持中心对齐
            }

            return new Vector2Int(
                clickGridPos.x + offsetX,
                clickGridPos.y + offsetY
            );
        }
        
        // 在 BuildGridSystem 类中添加以下方法
        private Vector2 GetCellLocalPosition(Vector3 worldPosition,Vector2Int gridPos)
        {
            Vector3 cellWorldPos = GridToWorldPosition(gridPos);
            Vector3 relativePos = worldPosition - cellWorldPos;

            // 归一化到 [0,1) 范围
            float localX = (relativePos.x / m_Grid.cellSize.x) + 0.5f;
            float localY = (relativePos.y / m_Grid.cellSize.y) + 0.5f;

            return new Vector2(localX, localY);
        }
        
        
        // 动态偏移计算
        private Vector2 GetBuildingCenterOffset(Vector2Int buildingSize, Vector2 cellLocalPos)
        {
            float offsetX = 0f;
            float offsetY = 0f;

            // 处理偶数尺寸的建筑
            if (buildingSize.x % 2 == 0)
            {
                // 根据点击位置在单元格内的局部坐标决定水平偏移方向
                offsetX = (cellLocalPos.x >= 0.5f) ? 0.5f : -0.5f;
            }

            if (buildingSize.y % 2 == 0)
            {
                // 根据点击位置在单元格内的局部坐标决定垂直偏移方向
                offsetY = (cellLocalPos.y >= 0.5f) ? 0.5f : -0.5f;
            }

            return new Vector2(offsetX, offsetY);
        }
        
        // 计算出建筑放置的世界坐标
        private Vector3 CalculateWorldPosition(Vector2Int gridPos, Vector3 cellLocalPos, Vector2Int size)
        {
            Vector2 centerOffset = GetBuildingCenterOffset(size, cellLocalPos);
        
            return GridToWorldPosition(gridPos) + 
                   new Vector3(
                       centerOffset.x,
                       centerOffset.y,
                       0
                   );
        }
        
        #endregion
       
        public GridProperties GetGridProperties() => properties;
        
        #endregion
    }
}