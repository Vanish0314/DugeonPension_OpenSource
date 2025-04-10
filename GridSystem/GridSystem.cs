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
using System.ComponentModel;
using CrashKonijn.Goap.Runtime;


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
            if (gridData != null)
            {
                Load(gridData);
            }

            if (!string.IsNullOrEmpty(m_GridDataPath))
            {
                Load(m_GridDataPath);
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
        }
        private void OnDisable()
        {
            DungeonGameEntry.DungeonGameEntry.WorldBlackboard.DeregisterExpert(this);
        }

        [SerializeField] private GridData gridData;
        [SerializeField] private string m_GridDataPath;


        [SerializeField] private VisualGrid m_VisualGrid;
        [SerializeField] private LogicalGrid m_LogicalGrid;
        private Grid m_Grid;
        private BlackboardKey m_DungeonExitWorldPositionVector3Key;
        private GridProperties properties => gridData.properties;

    }
}