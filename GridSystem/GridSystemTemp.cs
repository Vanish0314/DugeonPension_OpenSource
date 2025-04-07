using UnityEngine;
using UnityEngine.Tilemaps;
using Dungeon.Common;
using UnityGameFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeon.DungeonGameEntry;
using Dungeon.BlackBoardSystem;
using GameFramework;
using Dungeon;


namespace Dungeon.GridSystem
{
    [RequireComponent(typeof(Grid))]
    public class GridSystemTemp : MonoSingletonLasy<GridSystem>
    {
        private void Awake()
        { 
            m_Grid = gameObject.GetOrAddComponent<Grid>();

            m_VisualGrid = gameObject.GetOrAddComponent<VisualGrid>();
            m_BuildingGrid = gameObject.GetOrAddComponent<BuildingGrid>();
        }
        private void Start()
        {
            Debug.Assert(CheckEssentials(), "网格系统必须填入Tile");

            m_VisualGrid.SetTileBases(wallTile, groundTile, foundationTile);//------------

            InitPosition(transform);

            InitGrid();
        }
        private bool CheckEssentials() => groundTile && wallTile;

        /// <summary>
        /// Resize the grid with new properties
        /// </summary>
        /// <param name="props"></param>
        private void Resize(GridProperties props)
        {
            properties = props;

            m_VisualGrid.OnResize(properties);
        }
        private void InitPosition(Transform go)
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;

            foreach (Transform child in go)
                InitPosition(child);
        }

        /// <summary>
        /// Init Grid with default values
        /// </summary>
        private void InitGrid()
        {
            var p = m_VisualGrid.GetGridProperties();
            if (p == null)
            {
                p = new GridProperties(64, 64, 1, default);
                m_VisualGrid.OnResize(p.Value);
            }

            //m_LogicalGrid.Init(m_VisualGrid);
            m_BuildingGrid.Init(m_VisualGrid);//------------------

            properties = p.Value;
        }

        #region PUBLIC
        public void SetTile(Vector2Int gridPos, TileDesc tileDesc)
        {
            m_VisualGrid.SetTile(gridPos, tileDesc);

            if (tileDesc.type == TileType.Debug) // TEMP
                return;
            
            m_BuildingGrid.SetTile(gridPos, tileDesc.type == TileType.Foundation);
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
        
            // 更新逻辑网格（整个建筑区域设为不可通行）
            for (int x = gridPos.x; x < gridPos.x + buildingData.size.x; x++)
            {
                for (int y = gridPos.y; y < gridPos.y + buildingData.size.y; y++)
                {
                    //m_LogicalGrid.SetTile(new Vector2Int(x, y), GridCellReachableType.UnReachable);
                }
            }
        
            // 更新建筑网格
            if (m_BuildingGrid.TryPlaceBuilding(gridPos.x, gridPos.y, buildingData))
            {
                // 更新可视化网格（传入建筑尺寸）--------------------先不急
                // m_VisualGrid.SetBuildingTile(gridPos, new TileDesc { type = TileType.Building }, buildingData.size);
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
                // 更新逻辑网格（设为可通行）
                // m_LogicalGrid.SetTile(gridPos, GridCellReachableType.Reachabel);
                
                // 更新可视化网格
                m_VisualGrid.SetTile(gridPos, new TileDesc { type = TileType.Ground });
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
        [SerializeField] private DungeonRuleTile groundTile;
        [SerializeField] private DungeonRuleTile wallTile;
        [SerializeField] private DungeonRuleTile foundationTile;//----------------
        [SerializeField] private DungeonRuleTile debugTile;

        [SerializeField] private GridProperties properties;
        [SerializeField] private VisualGrid m_VisualGrid;
        [SerializeField] private BuildingGrid m_BuildingGrid;//-----------------
        private Grid m_Grid;
    }
}