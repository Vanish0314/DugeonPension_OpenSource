using System.Collections;
using System.Collections.Generic;
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

            // TODO(vanish): This is so stupid
            var temp = new Stack<Vector3>();
            var offset = m_LogicalGrid.GridDownLeftOriginPoint;
            foreach (var wayPoint in path.path)
                temp.Push(new Vector3(wayPoint.x, wayPoint.y, 0) + offset);

            var result = new Stack<Vector3>();
            foreach (var wayPoint in temp)
                result.Push(wayPoint);

            return result;
        }
        public Stack<Vector3> FindPath(Vector3 fromPosInWorldCoord, Vector3 toPosInWorldCoord)
        {
            return FindPath(new Vector2(
                fromPosInWorldCoord.x,
                fromPosInWorldCoord.y
            ), new Vector2(
                toPosInWorldCoord.x,
                toPosInWorldCoord.y
            ));
        }
        public Stack<Vector3> FindPath(Vector2 fromPosInWorldCoord, Vector2 toPosInWorldCoord)
        {
            return FindPath(new Vector2Int(
                m_LogicalGrid.WorldToGridPosition(fromPosInWorldCoord).x,
                m_LogicalGrid.WorldToGridPosition(fromPosInWorldCoord).y
            ), new Vector2Int(
                m_LogicalGrid.WorldToGridPosition(toPosInWorldCoord).x,
                m_LogicalGrid.WorldToGridPosition(toPosInWorldCoord).y
            ));
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
