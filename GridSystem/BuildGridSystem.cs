using System.Collections.Generic;
using Dungeon.DungeonGameEntry;
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

        private void Start()
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
            
            // 初始禁用所有Tilemap子物体
            SetAllTilemapsActive(false);
            
            DungeonGameEntry.DungeonGameEntry.Event.GetComponent<EventComponent>().Subscribe(OnSceneLoadedEventArgs.EventId,OnSceneLoaded);

            InitializeForCurrentScene(2);

        }
        private void OnDisable()
        {
            GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnSceneLoadedEventArgs.EventId,OnSceneLoaded);
        }
        
        private void OnSceneLoaded(object sender, GameEventArgs gameEventArgs)
        {
            if (gameEventArgs is OnSceneLoadedEventArgs sceneLoadedEventArgs)
                InitializeForCurrentScene(sceneLoadedEventArgs.SceneID);
        }

        private void InitializeForCurrentScene(int sceneId)
        {
            if (sceneId == 1)
                return;
            string targetTilemapName = $"Tilemap{sceneId}";

            // 查找对应的Tilemap子物体
            Transform tilemapChild = transform.Find(targetTilemapName);
            if (tilemapChild != null)
            {
                m_CurrentTilemap = tilemapChild.GetComponent<Tilemap>();
                if (m_CurrentTilemap != null)
                {
                    SetAllTilemapsActive(false);
                    m_CurrentTilemap.gameObject.SetActive(true);
                    InitializeGridSystem();
                    return;
                }
            }

            Debug.LogError($"找不到对应场景的Tilemap: {targetTilemapName}");
        }

        private void SetAllTilemapsActive(bool active)
        {
            foreach (Transform child in transform)
            {
                Tilemap tm = child.GetComponent<Tilemap>();
                if (tm != null)
                {
                    tm.gameObject.SetActive(active);
                }
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
        
        public bool CanBuildAt(Vector3 worldPos, BuildingData buildingData)
        {
            var gridPos = WorldToGridPosition(worldPos);
            return CanBuildAt(gridPos, buildingData);
        }

        /// <summary>
        /// 尝试在指定位置放置建筑
        /// </summary>
        public bool TryPlaceBuilding(Vector2Int gridPos, BuildingData buildingData)
        {
            if (!CanBuildAt(gridPos, buildingData)) return false;

            // 更新建筑网格
            if (m_BuildingGrid.TryPlaceBuilding(gridPos.x, gridPos.y, buildingData))
            {
                return true;
            }
            return false;
        }

        //这个不兑，暂时也用不到
        public bool TryPlaceBuilding(Vector3 worldPos, BuildingData buildingData)
        {
            var gridPos = WorldToGridPosition(worldPos);
            return TryPlaceBuilding(gridPos, buildingData);
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

        public bool TryRemoveBuilding(Vector3 worldPos)
        {
            var gridPos = WorldToGridPosition(worldPos);
            return TryRemoveBuilding(gridPos);
        }

        /// <summary>
        /// 获取指定位置的建筑数据
        /// </summary>
        public BuildingData GetBuildingData(Vector2Int gridPos)
        {
            return m_BuildingGrid.GetBuildingData(gridPos.x, gridPos.y);
        }

        public BuildingData GetBuildingData(Vector3 worldPos)
        {
            var gridPos = WorldToGridPosition(worldPos);
            return GetBuildingData(gridPos);
        }
        #endregion
       
        public GridProperties GetGridProperties() => properties;
        
        #endregion
    }
}