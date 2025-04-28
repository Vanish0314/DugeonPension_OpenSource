using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dungeon.BlackBoardSystem;
using Dungeon.Common.MonoPool;
using Dungeon.DungeonEntity.InteractiveObject;
using Dungeon.DungeonEntity.Monster;
using Dungeon.DungeonEntity.Trap;
using GameFramework;
using GameFramework.Event;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon.GridSystem
{
    public partial class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        public void Load(string GridDataPath)
        {
            m_GridDataPath = GridDataPath;
            var data = JsonUtility.FromJson<GridData>(File.ReadAllText(GridDataPath));
            Load(data);
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

            if (PlaceDungeonMonster(monster, gridPos, worldCenterPos))
            {
                GameEntry.Event.GetComponent<EventComponent>().Fire(this,OnMonsterPlacedEventArgs.Create(args.MonsterData));
            }
            else
            {
                monsterItem.ReturnToPool();
            }
        }
        public bool PlaceDungeonMonster(DungeonMonsterBase monsterInstiated, Vector2Int gridPos, Vector3 worldCenterPos)
        {
            if(!CouldPlaceMonster(gridPos))
                return false;
            
            monsterInstiated.transform.position = worldCenterPos;
            return true;
        }
        
        private void HandleTrapPlacement(object sender, GameEventArgs gameEventArgs)
        {
            TryPlaceTrapEventArgs args = (TryPlaceTrapEventArgs)gameEventArgs;
            
            Vector3 worldPos = args.WorldPosition;
            MonoPoolItem trapItem = args.TrapItem;

            Vector3 worldCenterPos = SnapToGridCenter(worldPos);
            Vector3 worldCornerPos = SnapToGridCorner(worldPos);
            Vector2Int gridPos = WorldToGridPosition(worldCornerPos);

            DungeonTrapBase trap = trapItem.GetComponent<DungeonTrapBase>();
            
            if (PlaceDungeonTrap(trap, gridPos, worldCenterPos))
            {
                GameEntry.Event.GetComponent<EventComponent>().Fire(this,OnTrapPlacedEventArgs.Create(args.TrapData));
            }
            else
            {
                trapItem.ReturnToPool();
            }
        }
        public bool PlaceDungeonTrap(DungeonTrapBase trapInstiated, Vector2Int gridPos, Vector3 worldCenterPos)
        {
            if(!CouldPlaceTrap(gridPos))
                return false;

            Debug.Log(gridPos);
            trapInstiated.transform.position = worldCenterPos;
            m_LogicalGrid.AddTrap(gridPos, trapInstiated);
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
        public bool CouldPlaceMonster(Vector2Int gridPos)
        {
            if(!IsValidGridPosition(gridPos))
                return false;

            if(!GetReachablilty(gridPos))
                return false;
            
            if(!GridIsSpare(gridPos))
                return false;

            if(HasMonsterOnCell(gridPos))
                return false;
            
            return true;
        }
        public bool CouldPlaceTrap(Vector2Int gridPos)
        {
            Debug.Log(IsValidGridPosition(gridPos));
            if(!IsValidGridPosition(gridPos))
                return false;
            
            Debug.Log(GridIsSpare(gridPos));
            if(!GridIsSpare(gridPos))
                return false;
            
            Debug.Log(!GetReachablilty(gridPos));
            if(!GetReachablilty(gridPos))
                return false;

            Debug.Log(HasMonsterOnCell(gridPos));
            if(HasMonsterOnCell(gridPos))
                return false;

            return true;
        }
        public bool CouldPlaceInteractiveObject(Vector2Int gridPos)
        {
            return CouldPlaceTrap(gridPos);
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

        public bool IsValidGridPosition(Vector2Int gridPos)
        {
            return m_LogicalGrid.IsValidGridPosition(gridPos);
        }
        /// <summary>
        /// 返回位置上是否没有DungeonEntity
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public bool GridIsSpare(Vector2Int gridPos)
        {
            return m_LogicalGrid.GetStaticEntity(gridPos) == null;
        }

        public bool GetReachablilty(Vector2Int gridPos)
        {
            return !m_LogicalGrid.IsUnReachable(gridPos);
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



