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
            DontDestroyOnLoad(gameObject);
            
            m_Grid = GetComponent<Grid>();
            m_BuildingGrid = GetComponent<BuildingGrid>();
        }

        private void Start()
        {
            InitializeForCurrentScene();
        }

        private void OnEnable()
        {
            GameEntry.Event.GetComponent<EventComponent>().Subscribe(TryPlaceBuildingEventArgs.EventId,HandleTryPlaceBuildingEventArgs);
        }

        private void OnDisable()
        {
           GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(TryPlaceBuildingEventArgs.EventId,HandleTryPlaceBuildingEventArgs);
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

            // 初始化建筑网格 -----------------------------------------仅经营部分会用到
            m_BuildingGrid.Init(m_CurrentTilemap);
        }
        
        private void HandleTryPlaceBuildingEventArgs(object sender, GameEventArgs gameEventArgs)
        {
            TryPlaceBuildingEventArgs args = (TryPlaceBuildingEventArgs)gameEventArgs;
            
            Vector3 worldPos = args.WorldPosition;
            MonoPoolItem buildingItem = args.BuildingItem;
            
            Vector2Int gridPos = WorldToGridPosition(worldPos);
    
            GameObject buildingGo = buildingItem.gameObject;

            if (TryPlaceBuilding(gridPos,args.BuildingData,buildingGo))
            {
                GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnBuildingPlacedEventArgs.Create(args.BuildingData));
            }
            else
            {
                buildingItem.ReturnToPool();
            }
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
        /// 尝试在指定位置放置建筑
        /// </summary>
        public bool TryPlaceBuilding(Vector2Int gridPos, BuildingData buildingData,GameObject buildingGameObject)
        {
            if (!CanBuildAt(gridPos, buildingData)) return false;

            // 更新建筑网格
            if (m_BuildingGrid.TryPlaceBuilding(gridPos.x, gridPos.y, buildingData))
            {
                buildingGameObject.transform.position = GridToWorldPosition(gridPos);
                return true;
            }
            return false;
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

        /// <summary>
        /// 获取指定位置的建筑数据
        /// </summary>
        public BuildingData GetBuildingData(Vector2Int gridPos)
        {
            return m_BuildingGrid.GetBuildingData(gridPos.x, gridPos.y);
        }
        
        #endregion
       
        public GridProperties GetGridProperties() => properties;
        
        #endregion
    }
}