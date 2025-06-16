using System;
using System.Collections.Generic;
using Dungeon.BlackBoardSystem;
using Dungeon.DungeonEntity;
using Dungeon.Evnents;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dungeon.GridSystem
{
    public partial class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        private void SubscribEvents()
        {
            GameEntry.Event.Subscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
            GameEntry.Event.Subscribe(OnDungeonCalculationFinishedEvent.EventId, OnDungeonCalculationFinishedEventHandler);
        }

        private void OnHeroArrivedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            // DuplicateDungeonEntity();            
        }
        private void OnDungeonCalculationFinishedEventHandler(object sender, GameEventArgs e)
        {
            // ClearDuplicatedEntity();

            ClearDungeonEntity();
        }
        private void UnSubscribEvents()
        {
            GameEntry.Event.Unsubscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
            GameEntry.Event.Unsubscribe(OnDungeonCalculationFinishedEvent.EventId, OnDungeonCalculationFinishedEventHandler);
        }

        private void DuplicateDungeonEntity()
        {
            foreach (var (pos, trap) in m_LogicalGrid.trapMap)
            {
                var newTrap = (DungeonTrapBase)trap.Duplicate();
                newTrap.transform.position = trap.transform.position;
                newTrap.transform.parent = trap.transform.parent;

                trap.gameObject.SetActive(false);
                newTrap.gameObject.SetActive(true);
                duplicatedEntities.Add(newTrap);

                SceneManager.MoveGameObjectToScene(newTrap.gameObject, SceneManager.GetSceneByName("DungeonGameScene"));
            }

            foreach (var (pos, interactiveObject) in m_LogicalGrid.interactMap)
            {
                var newInteractiveObject = (DungeonInteractiveObjectBase)interactiveObject.Duplicate();
                newInteractiveObject.transform.position = interactiveObject.transform.position;
                newInteractiveObject.transform.parent = interactiveObject.transform.parent;

                interactiveObject.gameObject.SetActive(false);
                newInteractiveObject.gameObject.SetActive(true);
                duplicatedEntities.Add(newInteractiveObject);

                SceneManager.MoveGameObjectToScene(newInteractiveObject.gameObject, SceneManager.GetSceneByName("DungeonGameScene"));
            }

            foreach (var (pos, monster) in m_LogicalGrid.monsterMap)
            {
                var newMonster = (DungeonMonsterBase)monster.Duplicate();
                newMonster.transform.position = monster.transform.position;
                newMonster.transform.parent = monster.transform.parent;

                monster.gameObject.SetActive(false);
                newMonster.gameObject.SetActive(true);
                duplicatedEntities.Add(newMonster);

                SceneManager.MoveGameObjectToScene(newMonster.gameObject, SceneManager.GetSceneByName("DungeonGameScene"));
            }
        }
        private void ClearDuplicatedEntity()
        {
            foreach (var entity in duplicatedEntities)
            {
                entity.gameObject.SetActive(false);
                entity.ReturnToPool();
            }

            foreach (var (pos, trap) in m_LogicalGrid.trapMap)
            {
                trap.gameObject.SetActive(true);
            }

            foreach (var (pos, interactiveObject) in m_LogicalGrid.interactMap)
            {
                interactiveObject.gameObject.SetActive(true);
            }

            foreach (var (pos, monster) in m_LogicalGrid.monsterMap)
            {
                monster.gameObject.SetActive(true);
            }
        }

        private void ClearDungeonEntity()
        {
            foreach (var entity in duplicatedEntities)
            {
                entity.gameObject.SetActive(false);
                entity.ReturnToPool();
            }

            foreach (var (pos, trap) in m_LogicalGrid.trapMap)
            {
                if (!trap.isInPool)
                    trap.ReturnToPool();
            }

            foreach (var (pos, interactiveObject) in m_LogicalGrid.interactMap)
            {
                if (!interactiveObject.isInPool)
                    interactiveObject.ReturnToPool();
            }

            foreach (var (pos, monster) in m_LogicalGrid.monsterMap)
            {
                if (!monster.isInPool)
                    monster.ReturnToPool();
            }

            foreach (var (room, monoRoom) in m_DungeonRoomDict)
            {
                monoRoom.capacity.currentMagic = 0;
                monoRoom.capacity.currentMaterial = 0;
            }
        }

        private List<DungeonEntity.DungeonEntity> duplicatedEntities = new();
    }
}