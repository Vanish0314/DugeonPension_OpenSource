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
            m_GalPrefab.SetActive(false);

            SubscribeEvents();
        }
        void OnDestroy()
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
        }

        private void OnHeroArrivedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            m_GalPrefab.SetActive(true);
        }

        private void UnSubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
        }

        private GameObject m_GalPrefab;
        
    }
}
