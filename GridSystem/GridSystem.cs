using UnityEngine;
using UnityGameFramework.Runtime;
using Dungeon.DungeonGameEntry;
using Dungeon.BlackBoardSystem;
using GameFramework;

namespace Dungeon.GridSystem
{
    [RequireComponent(typeof(Grid))]
    public partial class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        private void Start()
        {
            m_Grid = gameObject.GetOrAddComponent<Grid>();

            m_VisualGrid = gameObject.GetOrAddComponent<VisualGrid>();
            m_LogicalGrid = gameObject.GetOrAddComponent<LogicalGrid>();

            m_DungeonExitWorldPositionVector3Key = DungeonGameEntry.DungeonGameEntry.WorldBlackboard.GetBlackboard().GetOrRegisterKey(DungeonGameWorldBlackboardEnum.DungeonExitWorldPositionVector3);

            InitPosition(transform);

            InitGridSystem();
        }
        private void InitGridSystem()
        {
            if (!string.IsNullOrEmpty(m_GridDataPath)&&Load(m_GridDataPath))
            {
                return;
            }

            if (gridData != null)
            {
                Load(gridData);
                return;
            }

            GameFrameworkLog.Error("[GridSystem] GridData is null and path is null");
        }
        public void SetGrid(GridData data) => gridData = data;
        public void SetGrid(string path) => m_GridDataPath = path;
        public void UnLoad()
        {
            m_VisualGrid.Clear();
            m_LogicalGrid.Clear();
        }

        private void OnEnable()
        {
            DungeonGameEntry.DungeonGameEntry.WorldBlackboard.RegisterExpert(this);
            
            GameEntry.Event.GetComponent<EventComponent>().Subscribe(TryPlaceTrapEventArgs.EventId,HandleTrapPlacement);
            GameEntry.Event.GetComponent<EventComponent>().Subscribe(TryPlaceMonsterEventArgs.EventId,HandleMonsterPlacement);

            SubscribEvents();
        }
        private void OnDisable()
        {
            DungeonGameEntry.DungeonGameEntry.WorldBlackboard.DeregisterExpert(this);
            
            GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(TryPlaceTrapEventArgs.EventId,HandleTrapPlacement);
            GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(TryPlaceMonsterEventArgs.EventId,HandleMonsterPlacement);

            UnSubscribEvents();
        }

        public Material m_TileMapMaterial;
        [SerializeField] private GridData gridData;
        [SerializeField] private string m_GridDataPath;

        [SerializeField] private VisualGrid m_VisualGrid;
        [SerializeField] private LogicalGrid m_LogicalGrid;
        private Grid m_Grid;
        private BlackboardKey m_DungeonExitWorldPositionVector3Key;
        private GridProperties properties => gridData.properties;

    }
}