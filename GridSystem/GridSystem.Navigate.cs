using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeon.BlackBoardSystem;
using UnityEngine;

namespace Dungeon.GridSystem
{
    public partial class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        /// <summary>
        /// return path in world space
        /// </summary>
        /// <param name="fromPosInGridCoord"></param>
        /// <param name="toPosInGridCoord"></param>
        /// <returns></returns>
        public Stack<Vector3> FindPath(Vector2Int fromPosInGridCoord, Vector2Int toPosInGridCoord)
        {
            var path = m_LogicalGrid.FindPath_AStar(fromPosInGridCoord, toPosInGridCoord);

            var result = new Stack<Vector3>();
            var offset = m_LogicalGrid.GridDownLeftOriginPoint;
            var list = path.path.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var p = list[i];
                result.Push(new Vector3(p.x, p.y, 0) + offset);
            }

            return result;
        }
        public Stack<Vector3> FindPath(Vector3 fromPosInWorldCoord, Vector3 toPosInWorldCoord)
        {
            return FindPath(WorldToGridPosition(fromPosInWorldCoord), WorldToGridPosition(toPosInWorldCoord));
        }

        public Stack<Vector3> FindPath_IgnoreFromToDynamicObstacle(Vector2Int fromPosInGridCoord, Vector2Int toPosInGridCoord)
        {
            var path = m_LogicalGrid.FindPath_AStar_IgnoreFromToDynamicObstacle(fromPosInGridCoord, toPosInGridCoord);

            var result = new Stack<Vector3>();
            var offset = m_LogicalGrid.GridDownLeftOriginPoint;
            var list = path.path.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var p = list[i];
                result.Push(new Vector3(p.x, p.y, 0) + offset);
            }

            return result;
        }
        public Stack<Vector3> FindPath_IgnoreFromToDynamicObstacle(Vector3 fromPosInWorldCoord, Vector3 toPosInWorldCoord)
        {
            return FindPath_IgnoreFromToDynamicObstacle(WorldToGridPosition(fromPosInWorldCoord), WorldToGridPosition(toPosInWorldCoord));
        }

        public Vector3 FindNearestWallInDirection(Vector2Int fromPosInGridCoord, Vector2Int toPosInGridCoord)
        {
            Vector2 direction = (new Vector2(toPosInGridCoord.x, toPosInGridCoord.y) - new Vector2(fromPosInGridCoord.x, fromPosInGridCoord.y)).normalized;
            Vector2Int currentPos = fromPosInGridCoord;
            while (true)
            {
                currentPos += new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
                if (m_LogicalGrid.IsUnReachable(currentPos))
                {
                    return new Vector3(currentPos.x, currentPos.y, 0);
                }
            }
        }

        public Vector3 FindNearestWallInDirection(Vector3 fromPosInWorldCoord, Vector3 toPosInWorldCoord)
        {
            Vector2Int fromGridCoord = new Vector2Int(Mathf.RoundToInt(fromPosInWorldCoord.x), Mathf.RoundToInt(fromPosInWorldCoord.y));
            Vector2Int toGridCoord = new Vector2Int(Mathf.RoundToInt(toPosInWorldCoord.x), Mathf.RoundToInt(toPosInWorldCoord.y));
            return FindNearestWallInDirection(fromGridCoord, toGridCoord);
        }

        public Vector3 FindNearestWallInDirection(Vector2 fromPosInWorldCoord, Vector2 toPosInWorldCoord)
        {
            Vector2Int fromGridCoord = new Vector2Int(Mathf.RoundToInt(fromPosInWorldCoord.x), Mathf.RoundToInt(fromPosInWorldCoord.y));
            Vector2Int toGridCoord = new Vector2Int(Mathf.RoundToInt(toPosInWorldCoord.x), Mathf.RoundToInt(toPosInWorldCoord.y));
            return FindNearestWallInDirection(fromGridCoord, toGridCoord);
        }
        public Vector3 GetDungeonExitWorldPosition()
        {
            DungeonGameEntry.DungeonGameEntry.WorldBlackboard.TryGetValue<Vector3>(m_DungeonExitWorldPositionVector3Key, out var exit);

            return exit;
        }
    }
}
