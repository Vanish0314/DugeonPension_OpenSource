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
        public float cellSize;     // 每个单元格的大小
        public Vector3 originPoint; // 网格原点

        public GridProperties(int width = 128, int height = 128, float size = 1.0f, Vector3 origin = default)
        {
            this.width = width;
            this.height = height;
            cellSize = size;
            originPoint = origin;
        }
    }
    public struct LogicalCell
    {
        public LogicalCell(GridCellReachableType type) {  this.type = type; }
        
        public GridCellReachableType type;
    }
    public enum TileType
    {
        Ground,
        Wall,
        Building,
        Debug
    }
    public struct TileDesc
    {
        public TileType type;
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
