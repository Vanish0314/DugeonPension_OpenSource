using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using GameFramework;
using UnityEngine;
using static Dungeon.GridSystem.GridSystem;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour
    {
        public void OnVisitedRoom(Room room)
        {
            AddVisitedRoom(room);
        }
        public bool HaveVisitedRoom(Room room)
        {
            return m_BrainMemory.roomsVisited.Contains(room);
        }

        public Vector3 GetUnvisitedRoomCenterPos()
        {
            var room = GetUnvisitedRoom();
            return DungeonGameEntry.DungeonGameEntry.GridSystem.GridToWorldPosition(room.centerPos);
        }

        /// <summary>
        /// 让勇者尝试寻找前往出口
        /// </summary>
        private void TryFinishDungeon()
        {
            var nextGoal = GetUnvisitedRoom();
            if(nextGoal == null)
            {
                GameFrameworkLog.Error("[AgentLowLevelSystem] every room has been visited, can not find the exit");
                return;
            }

            MoveTo(DungeonGameEntry.DungeonGameEntry.GridSystem.GridToWorldPosition(nextGoal.centerPos));
        }
        
        // 清除记忆
        private void ClearBrainMemory()
        {
            m_BrainMemory = new();
        }

        // 加载记忆
        private void LoadBrainMemory(BrainMemory memory)
        {
            m_BrainMemory = memory;
        }

        private Room GetUnvisitedRoom()
        {
            List<Room> everyRoom = new ();

            foreach (var room in m_BrainMemory.roomsVisited)
            {
                foreach (var neighbour in room.neighbours)
                {
                    everyRoom.Add(neighbour);
                }
            }

            foreach (var room in everyRoom)
            {
                if(!m_BrainMemory.roomsVisited.Contains(room))
                {
                    return room;
                }
            }

            return null;
        }

        private void AddVisitedRoom(Room room)
        {
            if(!m_BrainMemory.roomsVisited.Contains(room))
            {
                m_BrainMemory.roomsVisited.Add(room);
            }
        }

        private BrainMemory m_BrainMemory = new();

        private class BrainMemory
        {
            public List<Room> roomsVisited = new ();
        }
    }
}