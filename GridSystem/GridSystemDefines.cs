using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.GridSystem
{
    [Serializable]
    public struct GridProperties
    {
        public int width;      // 网格宽度
        public int height;     // 网格高度
        public const float cellSize = 1;     // 每个单元格的大小
        public Vector3 originPoint; // 网格原点(即左下角)
    }
    public struct LogicalCell
    {
        public LogicalCell(TilePathBlockType type) {  this.type = type; }
        
        public TilePathBlockType type;
    }
    public struct BuildingCell
    {
        public bool IsBuildable; // 该单元格是否允许建造
        public bool HasBuilding; // 该单元格是否有建筑
        public BuildingData BuildingData; // 建筑数据（暂时）
        public Vector2Int BuildingOrigin; // 记录建筑原点坐标
    
        public BuildingCell(bool isBuildable = true)
        {
            this.IsBuildable = isBuildable;
            this.HasBuilding = false;
            this.BuildingData = null;
            this.BuildingOrigin = Vector2Int.zero;
        }
    }
    public enum TilePathBlockType
    {
        Ground,
        Wall
    }
    public enum TileFunctionType
    {
        Default,     
        Trap,        
        Treasure,    
        Door,
    }
    public struct TileDesc
    {
        public TilePathBlockType type;
    }

    public struct GridPath
    {
        public Stack<Vector2Int> path;
    }

    public enum GridCellReachableType
    {
        Reachabel,
        UnReachable
    }
}
