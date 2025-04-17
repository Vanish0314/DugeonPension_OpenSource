using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using Dungeon.DungeonEntity.InteractiveObject;
using Dungeon.DungeonEntity.Monster;
using Dungeon.DungeonEntity.Trap;
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
        public Vector3? GetUnvisitedRoomCenterPos()
        {
            var room = GetUnvisitedRoom();
            
            if(room != null)
                return DungeonGameEntry.DungeonGameEntry.GridSystem.GridToWorldPosition(room.centerPos);

            GameFrameworkLog.Warning("[AgentLowLevelSystem] GetUnvisitedRoomCenterPos: every room has been visited");
            return null;
        }
        public DungeonMonsterBase GetNearestTorchInVision()
        {
            if(m_BrainMemory.monstersInVision.Count == 0)
            {
                return null;
            }

            var result = m_BrainMemory.monstersInVision[0];
            foreach(var monster in m_BrainMemory.monstersInVision)
            {
                if(Vector3.Distance(transform.position, monster.transform.position) < Vector3.Distance(transform.position, result.transform.position))
                {
                    result = monster;
                }
            }

            return result;
        }

        public DungeonTrapBase GetNearestTrapInVision()
        {
            if(m_BrainMemory.trapInVision.Count == 0)
            {
                return null;
            }

            var result = m_BrainMemory.trapInVision[0];
            foreach(var trap in m_BrainMemory.trapInVision)
            {
                if(Vector3.Distance(transform.position, trap.transform.position) < Vector3.Distance(transform.position, result.transform.position))
                {
                    result = trap;
                }
            }

            return result;
        }

        public StandardDungeonTreasureChest GetNearestTreasureChest()
        {
            if(m_BrainMemory.treasureChestInVision.Count == 0)
            {
                return null;
            }

            var result = m_BrainMemory.treasureChestInVision[0];
            foreach(var treasureChest in m_BrainMemory.treasureChestInVision)
            {
                if(Vector3.Distance(transform.position, treasureChest.transform.position) < Vector3.Distance(transform.position, result.transform.position))
                {
                    result = treasureChest;
                }
            }

            return result;
        }
        

        public void OnSee(GameObject entity)
        {
            if(entity.GetComponent<DungeonMonsterBase>()!= null)
            {
                m_BrainMemory.monstersInVision.Add(entity.GetComponent<DungeonMonsterBase>());
            }
            else if(entity.GetComponent<StandardDungeonTreasureChest>()!= null)
            {
                m_BrainMemory.treasureChestInVision.Add(entity.GetComponent<StandardDungeonTreasureChest>());
            }
            else if(entity.GetComponent<DungeonTrapBase>()!= null)
            {
                m_BrainMemory.trapInVision.Add(entity.GetComponent<DungeonTrapBase>());
            }
            else
            {
                GameFrameworkLog.Error("[AgentLowLevelSystem] OnSee: unknown entity:" + entity.name);
            }
        }
        public void OnUnSee(GameObject entity) //TODO(vanish): 当go死亡时不调用导致没能正确销毁
        {
            if(entity.GetComponent<DungeonMonsterBase>()!= null)
            {
                m_BrainMemory.monstersInVision.Remove(entity.GetComponent<DungeonMonsterBase>());
            }
            else if(entity.GetComponent<StandardDungeonTreasureChest>()!= null)
            {
                m_BrainMemory.treasureChestInVision.Remove(entity.GetComponent<StandardDungeonTreasureChest>());
            }
            else if(entity.GetComponent<DungeonTrapBase>()!= null)
            {
                m_BrainMemory.trapInVision.Remove(entity.GetComponent<DungeonTrapBase>());
            }
            else
            {
                GameFrameworkLog.Error("[AgentLowLevelSystem] OnUnSee: unknown entity:" + entity.name);
            }
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
            public List<DungeonMonsterBase> monstersInVision = new ();
            public List<StandardDungeonTreasureChest> treasureChestInVision = new ();
            public List<DungeonTrapBase> trapInVision = new ();
        }
    }
}