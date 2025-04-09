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

        [SerializeField] private GridData gridData;

        [SerializeField] private VisualGrid m_VisualGrid;
        [SerializeField] private LogicalGrid m_LogicalGrid;
        private Grid m_Grid;
        private BlackboardKey m_DungeonExitWorldPositionVector3Key;
        private GridProperties properties => gridData.properties;

    }
}