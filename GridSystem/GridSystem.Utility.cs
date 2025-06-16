using System.Collections;
using System.Collections.Generic;
using Dungeon.BlackBoardSystem;
using UnityEngine;

namespace Dungeon.GridSystem
{
    public partial class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        public Vector3 GridToWorldPosition(Vector2Int gridPos) => m_LogicalGrid.GridToWorldPosition(gridPos.x, gridPos.y);
        public Vector2Int WorldToGridPosition(Vector3 worldPosition) => m_LogicalGrid.WorldToGridPosition(worldPosition);
        public GridProperties GetGridProperties() => properties;
        public Vector3 SnapToGridCorner(Vector3 worldPosition) => m_LogicalGrid.SnapToGridCorner(worldPosition);
        public Vector3 SnapToGridCenter(Vector3 worldPosition) => m_LogicalGrid.SnapToGridCenter(worldPosition);
        public int GetGridDistance(Vector3 worldPosition1, Vector3 worldPosition2) => m_LogicalGrid.GetGridDistance(worldPosition1, worldPosition2);
        public int GetGridDistance(Vector2Int gridPosition1, Vector2Int gridPosition2) => m_LogicalGrid.GetGridDistance(gridPosition1, gridPosition2);
    }
}
