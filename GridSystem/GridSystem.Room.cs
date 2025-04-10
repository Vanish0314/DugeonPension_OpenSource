using System.Collections;
using System.Collections.Generic;
using Dungeon.BlackBoardSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.GridSystem
{
    public partial class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        public class Room
        {
            public string name;
            public int hashID;
            public Vector2Int centerPos;
            public Room[] neighbours;

            public Room(Vector2Int centerPos, string name,Room[] neighbours)
            {
                this.centerPos = centerPos;
                this.name = name;
                hashID = name.ComputeFNV1aHash();
                this.neighbours = neighbours;
            }
        }

        private void CalculateRoom()
        {
            m_DungeonRooms = m_LogicalGrid.CalculateRoom();
        }

        public void GetRoomAt(Vector2Int gridPos, out Room room)
        {
            m_LogicalGrid.GetRoomAt(gridPos, out room);
        }
        private List<Room> m_DungeonRooms;

    }
}
