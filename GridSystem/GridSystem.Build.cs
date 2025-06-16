using System.IO;
using Dungeon.BlackBoardSystem;
using Dungeon.Common;
using Dungeon.DungeonEntity;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon.GridSystem
{
    public partial class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        public bool Load(string GridDataPath)
        {
            m_GridDataPath = GridDataPath;
            try
            {
                var data = JsonUtility.FromJson<GridData>(File.ReadAllText(GridDataPath));
                Load(data);

            }
            catch (System.Exception e)
            {
                return false;
            }

            return true;
        }
        public void Load(GridData data)
        {
            gridData = data;
            InitGrid();
        }
        public void PlaceDungeonEntity(GameObject goInstiated, Vector2Int gridPos)
        {
            var entity = goInstiated.GetComponent<DungeonMonsterBase>();
            if( entity != null)
            {
                PlaceDungeonEntity(entity, gridPos);
            }
        }
        public void PlaceDungeonEntity(DungeonEntity.DungeonEntity entityInstiated, Vector2Int gridPos)
        {
            if (entityInstiated is DungeonMonsterBase monster)
            {
                //PlaceDungeonMonster(monster,gridPos);
            }
            else if (entityInstiated is DungeonTrapBase trap)
            {
                //PlaceDungeonTrap(trap,gridPos);
            }
            else if (entityInstiated is DungeonInteractiveObjectBase interactiveObject)
            {
                PlaceDungeonInteractiveObject(interactiveObject,gridPos);
            }
        }
        
        private void HandleMonsterPlacement(object sender, GameEventArgs gameEventArgs)
        {
            TryPlaceMonsterEventArgs args = (TryPlaceMonsterEventArgs)gameEventArgs;
            
            Vector3 worldPos = args.WorldPosition;
            MonoPoolItem monsterItem = args.MonsterItem;
            
            Vector3 worldCenterPos = SnapToGridCenter(worldPos);
            Vector3 worldCornerPos = SnapToGridCorner(worldPos);
            Vector2Int gridPos = WorldToGridPosition(worldCornerPos);

            DungeonMonsterBase monster = monsterItem.GetComponent<DungeonMonsterBase>();

            if (PlaceDungeonMonster(monster, gridPos, worldCenterPos,args.MonsterData.cost))
            {
                GameEntry.Event.GetComponent<EventComponent>().Fire(this,OnMonsterPlacedEventArgs.Create(args.MonsterData));
            }
            else
            {
                monsterItem.ReturnToPool();
            }
        }
        
        public bool PlaceDungeonMonster(DungeonMonsterBase monsterInstiated, Vector2Int gridPos, Vector3 worldCenterPos, Cost cost)
        {
            if (!CouldPlaceMonster(gridPos,cost))
                return false;

            Vector3 yOffset = new Vector3(0, -0.5f, 0);
            
            monsterInstiated.transform.position = worldCenterPos + yOffset;
            m_LogicalGrid.AddMonster(gridPos, monsterInstiated);

            GetMonoRoomAt(gridPos, out var room);
            room.AddEntity(monsterInstiated, cost);
            
            Audio.Instance.PlayAudio("放置魔物");

            return true;
        }
        
        private void HandleTrapPlacement(object sender, GameEventArgs gameEventArgs)
        {
            TryPlaceTrapEventArgs args = (TryPlaceTrapEventArgs)gameEventArgs;
            
            Vector3 worldPos = args.WorldPosition;
            MonoPoolItem trapItem = args.TrapItem;

            Vector3 worldCenterPos = SnapToGridCenter(worldPos);
            Vector3 worldCornerPos = SnapToGridCorner(worldPos);
            Vector2Int gridPos = WorldToGridPosition(worldCenterPos);

            DungeonTrapBase trap = trapItem.GetComponent<DungeonTrapBase>();
            
            if (PlaceDungeonTrap(trap, gridPos, worldCenterPos,args.TrapData.cost))
            {
                GameEntry.Event.GetComponent<EventComponent>().Fire(this,OnTrapPlacedEventArgs.Create(args.TrapData));
            }
            else
            {
                trapItem.ReturnToPool();
            }
        }
        public bool PlaceDungeonTrap(DungeonTrapBase trapInstiated, Vector2Int gridPos, Vector3 worldCenterPos, Cost cost)
        {
            if(!CouldPlaceTrap(gridPos,trapInstiated,cost))
                return false;

            trapInstiated.transform.position = worldCenterPos;
            m_LogicalGrid.AddTrap(gridPos, trapInstiated);

            GetMonoRoomAt(gridPos, out var room);
            room.AddEntity(trapInstiated, cost);

            Audio.Instance.PlayAudio("陷阱布署");
            
            return true;
        }
        public void PlaceDungeonInteractiveObject(DungeonInteractiveObjectBase interactiveObjectInstiated, Vector2Int gridPos)
        {
            if(!CouldPlaceInteractiveObject(gridPos))
                return;

            interactiveObjectInstiated.transform.position = m_LogicalGrid.GridToWorldPosition(gridPos.x, gridPos.y);
            m_LogicalGrid.AddInteractiveObject(gridPos, interactiveObjectInstiated);
        }
        public void SetTile(Vector2Int gridPos, DungeonRuleTile tile)
        {
            if(!CouldPlaceTile(gridPos, tile))
                return;

            m_VisualGrid.SetTile(gridPos, tile);
            m_LogicalGrid.SetTile(gridPos, tile);
        }
        public void SetTile(Vector2Int gridPos, DungeonTile tile)
        {
            if(!CouldPlaceTile(gridPos, tile))
                return;

            m_VisualGrid.SetTile(gridPos, tile);
            m_LogicalGrid.SetTile(gridPos, tile);
        }
        public bool CouldPlaceMonster(Vector2Int gridPos,Cost cost)
        {
            if(!IsValidGridPosition(gridPos))
                return false;

            if(!GetReachablilty(gridPos))
                return false;
            
            if(!GridIsSpare(gridPos))
                return false;

            if(HasMonsterOnCell(gridPos))
                return false;

            if(!HasReachedRoomCapacity(gridPos,cost))
                return false;
            
            return true;
        }
        public bool CouldPlaceTrap(Vector2Int gridPos,DungeonTrapBase trap, Cost cost)
        {
            if(!IsValidGridPosition(gridPos))
                return false;
            
            if (!GridIsSpare(gridPos,trap))
                return false;

            if(!GetReachablilty(gridPos))
                return false;

            if(HasMonsterOnCell(gridPos))
                return false;

            if(!HasReachedRoomCapacity(gridPos,cost))
                return false;

            return true;
        }
        public bool CouldPlaceInteractiveObject(Vector2Int gridPos)
        {
            GameFrameworkLog.Warning($"[GridSystem] 不能放置互动物品");
            return false;
            // return CouldPlaceTrap(gridPos);
        }
        public bool CouldPlaceTile(Vector2Int gridPos, DungeonRuleTile tile)
        {
            if(! IsValidGridPosition(gridPos))
                return false;

            if(! GridIsSpare(gridPos))
                return false;

            return true;
        }
        public bool CouldPlaceTile(Vector2Int gridPos, DungeonTile tile)
        {
            if(! IsValidGridPosition(gridPos))
                return false;
            
            if(! GridIsSpare(gridPos))
                return false;

            if(! GetReachablilty(gridPos))
                return false;

            return true;
        }

        public bool HasReachedRoomCapacity(Vector2Int gridPos, Cost cost)
        {
            GetMonoRoomAt(gridPos, out var room);
            if (room == null)
            {
                GameFrameworkLog.Info($"[GridSystem] 无法放置,因为没有房间");
                return false;
            }

            var res = room.CouldHold(cost);

            if (!res)
            {
                GameFrameworkLog.Info($"[GridSystem] 房间{room.name}容量不足,无法放置");
            }

            return res;
        }

        public bool IsValidGridPosition(Vector2Int gridPos)
        {
            var res = m_LogicalGrid.IsValidGridPosition(gridPos);
            if (!res)
            {
                GameFrameworkLog.Info($"[GridSystem] 位置{gridPos}不在逻辑网格内,无法放置");
            }

            return res;
        }
        /// <summary>
        /// 返回位置上是否没有DungeonEntity
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public bool GridIsSpare(Vector2Int gridPos)
        {
            var res = m_LogicalGrid.GetStaticEntity(gridPos) == null;
            if (!res)
            {
                GameFrameworkLog.Info($"[GridSystem] 位置{gridPos}上已经有实体,无法放置");
            }

            return res;
        }

        /// <summary>
        /// 返回位置上是否没有DungeonEntity
        /// </summary>
        /// <param name="gridPos"></param>
        /// <param name="trap"></param>
        /// <returns></returns>
        public bool GridIsSpare(Vector2Int gridPos, DungeonTrapBase trap)
        {
            bool res = true;

            for (int x = 0; x < trap.size.x; x++)
            {
                for (int y = 0; y < trap.size.y; y++)
                {
                    if (!GridIsSpare(new Vector2Int(gridPos.x + x, gridPos.y + y)))
                        res = false;
                }
            }

            if (!res)
            {
                GameFrameworkLog.Info($"[GridSystem] 位置{gridPos}上已经有陷阱,无法放置");
            }

            return res;
        }

        public bool GetReachablilty(Vector2Int gridPos)
        {
            var res = !m_LogicalGrid.IsUnReachable(gridPos);

            if (!res)
            {
                GameFrameworkLog.Info($"[GridSystem] 位置{gridPos}不可达,无法放置");
            }

            GetMonoRoomAt(gridPos, out var room);
            if (room != null && !room.IsExit())
            {
                res &= true;
            }
            else
            {
                GameFrameworkLog.Info($"[GridSystem] 位置{gridPos}不在可放置房间内,无法放置");
            }

            return res;
        }
        public bool HasMonsterOnCell(Vector2Int gridPos)
        {
            Collider2D[] results = new Collider2D[2];
            Physics2D.OverlapBoxNonAlloc(GridToWorldPosition(gridPos),Vector2.one*0.5f,0,results,LayerMask.GetMask("Monster"));
         
            #if UNITY_EDITOR
            foreach (Collider2D collider in results)
            {
                if(collider == null)
                    break;
                if(collider.gameObject.GetComponent<DungeonMonsterBase>()== null)
                {
                    GameFrameworkLog.Error("[GridSystem] Unexpected Collider2D found on cell, please check the layer of the collider:" + collider.gameObject.name);
                }
            }
            #endif

            return results[0] != null;
        }
        private void InitGrid()
        {
            m_VisualGrid.Load(gridData);

            m_LogicalGrid.Init(gridData);
            InitRooms();
        }
        private void InitPosition(Transform go)
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;

            foreach (Transform child in go)
                InitPosition(child);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            m_Grid = gameObject.GetOrAddComponent<Grid>();

            m_VisualGrid = gameObject.GetOrAddComponent<VisualGrid>();
            m_LogicalGrid = gameObject.GetOrAddComponent<LogicalGrid>();
        }
#endif
    }
}



