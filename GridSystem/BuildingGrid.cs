using System.Collections;
using System.Collections.Generic;
using Dungeon.GridSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon
{
    public class BuildingGrid : MonoBehaviour
    {
        private GridProperties gridProperties;
        private Map2D<BuildingCell> grid;
        
        private Vector3 originPoint => gridProperties.originPoint;
        public Vector3 GridDownLeftOriginPoint => GridToWorldPosition(0, 0);
        private Vector3 offset => new Vector3(GridProperties.cellSize / 2, GridProperties.cellSize / 2, 0);


        private void InitializeGrid()
        {
            grid = new Map2D<BuildingCell>(gridProperties.width, gridProperties.height);
            grid.FillAll(new BuildingCell());
        }

        public void OnResize(GridProperties properties)
        {
            gridProperties = properties;

            InitializeGrid();
        }
        public void Init(GridProperties properties)
        {
            gridProperties = properties;
            
            OnResize(properties);
        }
        
        public void Init(Tilemap tilemap)
        {
            var gridDesc = new GridProperties
            {
                width = tilemap.size.x,
                height = tilemap.size.y,
                originPoint = tilemap.origin
            };
            Init(gridDesc);
            
            var bounds = tilemap.cellBounds;
            
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int tilePosition = new (x, y, 0);
                    TileBase tile = tilemap.GetTile(tilePosition);

                    // Trans world position to grid position
                    var gridX = x - bounds.xMin;
                    var gridY = y - bounds.yMin;

                    if (tile is IMetropolisTile metropolisTile)
                    {
                        if(metropolisTile.PlacementType == TilePlacementType.Foundation)
                            grid.Set(gridX,gridY,new BuildingCell(true));
                        else
                            grid.Set(gridX,gridY,new BuildingCell(false));
                    }

                }
            }
        }

        #region PUBLIC METHODS

        public void SetTile(Vector2Int gridPos, bool isBuildable)
        {
            grid.Set(gridPos.x, gridPos.y, new BuildingCell { IsBuildable = isBuildable});
        }
        
        public bool CanBuildAt(int originX, int originY, BuildingData buildingData)
        {
            // 检查建筑是否超出网格边界
            if (originX < 0 || originY < 0 ||
                originX + buildingData.size.x > gridProperties.width ||
                originY + buildingData.size.y > gridProperties.height)
                return false;

            // 检查区域内所有格子
            for (int x = originX; x < originX + buildingData.size.x; x++)
            {
                for (int y = originY; y < originY + buildingData.size.y; y++)
                {
                    if (!IsValidCoordinate(x, y)) return false;
                    var cell = grid.Get(x, y);
                    if (!cell.IsBuildable || cell.HasBuilding)
                        return false;
                }
            }

            return true;
        }
        
        // 陷阱放置判断------------------------------------------------------------暂时
        public bool CanBuildAt(int originX, int originY, TrapData trapData)
        {
            // 检查建筑是否超出网格边界
            if (originX < 0 || originY < 0 ||
                originX + trapData.size.x > gridProperties.width ||
                originY + trapData.size.y > gridProperties.height)
                return false;

            // 检查区域内所有格子
            for (int x = originX; x < originX + trapData.size.x; x++)
            {
                for (int y = originY; y < originY + trapData.size.y; y++)
                {
                    if (!IsValidCoordinate(x, y)) return false;
                    var cell = grid.Get(x, y);
                    if (!cell.IsBuildable || cell.HasBuilding)
                        return false;
                }
            }

            return true;
        }
        
        // 怪物放置判断--------------------------------------------------------------暂时
        public bool CanBuildAt(int originX, int originY, MonsterData monsterData)
        {
            // 检查建筑是否超出网格边界
            if (originX < 0 || originY < 0 ||
                originX + monsterData.size.x > gridProperties.width ||
                originY + monsterData.size.y > gridProperties.height)
                return false;

            // 检查区域内所有格子
            for (int x = originX; x < originX + monsterData.size.x; x++)
            {
                for (int y = originY; y < originY + monsterData.size.y; y++)
                {
                    if (!IsValidCoordinate(x, y)) return false;
                    var cell = grid.Get(x, y);
                    if (!cell.IsBuildable || cell.HasBuilding)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 尝试放置建筑
        /// </summary>
        public bool TryPlaceBuilding(int originX, int originY, BuildingData buildingData)
        {
            if (!CanBuildAt(originX, originY, buildingData))
                return false;

            // 标记区域内所有格子
            for (int x = originX; x < originX + buildingData.size.x; x++)
            {
                for (int y = originY; y < originY + buildingData.size.y; y++)
                {
                    grid.Set(x, y, new BuildingCell
                    {
                        IsBuildable = false, 
                        HasBuilding = true,
                        BuildingData = buildingData,
                        BuildingOrigin = new Vector2Int(originX, originY)
                    });
                }
            }

            return true;
        }

        /// <summary>
        /// 尝试移除建筑
        /// </summary>
        public bool TryRemoveBuilding(int x, int y)
        {
            if (!IsValidCoordinate(x, y)) return false;

            var cell = grid.Get(x, y);
            if (!cell.HasBuilding) return false;

            // 获取建筑原点
            Vector2Int origin = cell.BuildingOrigin;
            BuildingData data = cell.BuildingData;

            // 清除整个建筑区域
            for (int bx = origin.x; bx < origin.x + data.size.x; bx++)
            {
                for (int by = origin.y; by < origin.y + data.size.y; by++)
                {
                    grid.Set(bx, by, new BuildingCell
                    {
                        IsBuildable = true,
                        HasBuilding = false,
                        BuildingData = null
                    });
                }
            }
            return true;
        }


        /// <summary>
        /// 获取指定坐标的建筑数据
        /// </summary>
        public BuildingData GetBuildingData(int x, int y)
        {
            if (!IsValidCoordinate(x, y)) return null;
            return grid.Get(x, y).BuildingData;
        }

        /// <summary>
        /// 世界坐标转网格坐标
        /// </summary>
        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        { 
            worldPosition -= offset;
            int x = Mathf.FloorToInt((worldPosition.x - gridProperties.originPoint.x) / GridProperties.cellSize);
            int y = Mathf.FloorToInt((worldPosition.y - gridProperties.originPoint.y) / GridProperties.cellSize);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// 网格坐标转世界坐标
        /// </summary>
        public Vector3 GridToWorldPosition(int x, int y)
        {
            return new Vector3(
                x * GridProperties.cellSize + gridProperties.originPoint.x,
                y * GridProperties.cellSize + gridProperties.originPoint.y,
                0) + offset;
        }

        #endregion

        #region PRIVATE METHODS

        private bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 &&
                   x < gridProperties.width &&
                   y >= 0 &&
                   y < gridProperties.height;
        }

        #endregion
        
        #region EDITOR GIZMOS

#if UNITY_EDITOR
        [Header("Gizmos")]
        public bool enableGizmos = true;
        public Color buildableColor = Color.green;
        public Color occupiedColor = Color.red;
        public Color invalidColor = Color.gray;

        private void OnDrawGizmos()
        {
            if (!enableGizmos || grid == null) return;

            for (int x = 0; x < gridProperties.width; x++)
            {
                for (int y = 0; y < gridProperties.height; y++)
                {
                    Vector3 cellWorldPos = GridToWorldPosition(x, y);
                    var cell = grid.Get(x, y);

                    if (!cell.IsBuildable)
                    {
                        Gizmos.color = invalidColor;
                    }
                    else if (cell.HasBuilding)
                    {
                        Gizmos.color = occupiedColor;
                    }
                    else
                    {
                        Gizmos.color = buildableColor;
                    }

                    Gizmos.DrawWireCube(cellWorldPos, new Vector3(GridProperties.cellSize, GridProperties.cellSize, 0));
                }
            }
        }
#endif

        #endregion
    }
}

