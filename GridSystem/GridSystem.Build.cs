using System.Collections;
using System.Collections.Generic;
using Dungeon.BlackBoardSystem;
using UnityEngine;

namespace Dungeon.GridSystem
{
    public partial class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        public void Load(GridData data)
        {
            gridData = data;
            InitGrid();
        }
        public void SetTile(Vector2Int gridPos, DungeonRuleTile tile)
        {
            m_VisualGrid.SetTile(gridPos, tile);
            m_LogicalGrid.SetTile(gridPos, tile);
        }
        private void InitGrid()
        {
            m_VisualGrid.Load(gridData);
            m_VisualGrid.Init();

            m_LogicalGrid.Init(gridData);
        }
        private void InitPosition(Transform go)
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;

            foreach (Transform child in go)
                InitPosition(child);
        }
    }
}
