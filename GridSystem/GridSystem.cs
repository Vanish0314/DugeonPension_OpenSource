using UnityEngine;
using UnityEngine.Tilemaps;
using Dungeon.Common;
using UnityGameFramework.Runtime;


namespace Dungeon.GridSystem
{
    [RequireComponent(typeof(Grid))]
    public class GridSystem : MonoSingletonLasy<GridSystem>
    {
        private void Awake()
        {
            m_Grid = gameObject.GetOrAddComponent<Grid>();

            m_VisualGrid = gameObject.GetOrAddComponent<VisualGrid>();
            m_LogicalGrid = gameObject.GetOrAddComponent<LogicalGrid>();

        }
        private void Start()
        {
            Debug.Assert(CheckEssentials(), "网格系统必须填入Tile");

            m_VisualGrid.SetTileBases(wallTile, groundTile, debugTile);

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

            if(tileDesc.type == TileType.Debug) // TEMP
                return;

            m_LogicalGrid.SetTile(gridPos , tileDesc.type == TileType.Wall ? GridCellReachableType.UnReachable : GridCellReachableType.Reachabel);
        }
        public GridProperties GetGridProperties() => properties;
        #endregion
        [SerializeField] private DungeonRuleTile groundTile;
        [SerializeField] private DungeonRuleTile wallTile;
        [SerializeField] private DungeonRuleTile debugTile;

        [SerializeField] private GridProperties properties;
        [SerializeField] private VisualGrid m_VisualGrid;
        [SerializeField] private LogicalGrid m_LogicalGrid;
        private Grid m_Grid;

    }
}