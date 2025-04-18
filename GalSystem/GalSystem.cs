using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public class GalSystem : MonoBehaviour
    {
        public void HeroEndedDialogureBeforeDungeonExplore()
        {
            GameEntry.Event.Fire(this, OnHeroStartExploreDungeonEvent.Create());
        }

        private void Start()
        {
            m_GalPrefab = transform.GetChild(0).gameObject;
            m_Dto = transform.GetComponentInChildren<DtoManager>().gameObject;
            m_Dao = transform.GetComponentInChildren<DaoManager>().gameObject;

            m_GalPrefab.SetActive(false);
            m_Dto.SetActive(false);
            m_Dao.SetActive(false);
            
            SubscribeEvents();
        }
        void OnDestroy()
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
            GameEntry.Event.Subscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);
        }

        private void OnHeroArrivedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            m_GalPrefab.SetActive(true);
            m_Dto.SetActive(true);
            m_Dao.SetActive(true);
        }

        private void OnHeroStartExploreDungeonEventHandler(object sender, GameEventArgs e)
        {
            m_GalPrefab.SetActive(false);
            m_Dto.SetActive(false);
            m_Dao.SetActive(false);
        }

        private void UnSubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
            GameEntry.Event.Unsubscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);
        }

        private GameObject m_GalPrefab;
        private GameObject m_Dto;
        private GameObject m_Dao;
        
    }
}
