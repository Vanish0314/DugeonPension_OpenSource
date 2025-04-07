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


namespace Dungeon.GridSystem
{
    [RequireComponent(typeof(Grid))]
    public class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        private void Awake()
        {
        }
        private void Start()
        {
            m_Grid = gameObject.GetOrAddComponent<Grid>();

            m_VisualGrid = gameObject.GetOrAddComponent<VisualGrid>();
            m_LogicalGrid = gameObject.GetOrAddComponent<LogicalGrid>();

            m_DungeonExitWorldPositionVector3Key = DungeonGameEntry.DungeonGameEntry.WorldBlackboard.GetBlackboard().GetOrRegisterKey(DungeonGameWorldBlackboardEnum.DungeonExitWorldPositionVector3);

            Debug.Assert(CheckEssentials(), "网格系统必须填入Tile");

            m_VisualGrid.SetTileBases(wallTile, groundTile, debugTile);

            InitPosition(transform);

            InitGrid();
        }
        private void OnEnable()
        {
            DungeonGameEntry.DungeonGameEntry.WorldBlackboard.RegisterExpert(this);
        }
        private void OnDisable()
        {
            DungeonGameEntry.DungeonGameEntry.WorldBlackboard.DeregisterExpert(this);
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
            m_LogicalGrid.OnResize(properties);
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

            m_LogicalGrid.Init(m_VisualGrid);

            properties = p.Value;
        }

        #region PUBLIC
        public void SetTile(Vector2Int gridPos, TileDesc tileDesc)
        {
            m_VisualGrid.SetTile(gridPos, tileDesc);

            if (tileDesc.type == TileType.Debug) // TEMP
                return;

            m_LogicalGrid.SetTile(gridPos, tileDesc.type == TileType.Wall ? GridCellReachableType.UnReachable : GridCellReachableType.Reachabel);
        }

        public Vector3 GetDungeonExitWorldPosition()
        {
            DungeonGameEntry.DungeonGameEntry.WorldBlackboard.TryGetValue<Vector3>(m_DungeonExitWorldPositionVector3Key, out var exit);

            return exit;
        }
        public Vector3 GrideToWorldPosition(Vector2Int gridPos) => m_LogicalGrid.GridToWorldPosition(gridPos.x, gridPos.y);
        public Vector2Int WorldToGridPosition(Vector3 worldPosition) => m_LogicalGrid.WorldToGridPosition(worldPosition);

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
        public GridProperties GetGridProperties() => properties;

        public int GetInsistence(Blackboard blackboard)
        {
            return (int)InsistenceLevel.Dictator;
        }

        public void Execute(Blackboard blackboard)
        {
        }
        #endregion
        [SerializeField] private DungeonRuleTile groundTile;
        [SerializeField] private DungeonRuleTile wallTile;
        [SerializeField] private DungeonRuleTile debugTile;

        [SerializeField] private GridProperties properties;
        [SerializeField] private VisualGrid m_VisualGrid;
        [SerializeField] private LogicalGrid m_LogicalGrid;
        private Grid m_Grid;

        private BlackboardKey m_DungeonExitWorldPositionVector3Key;

    }
}