using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Evnents;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon.DungeonGameEntry
{
    public partial class DungeonGameEntry : MonoBehaviour
    {
        private static DungeonGameEntry instance;
        private static readonly object lockObject = new();

        public static DungeonGameEntry Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType<DungeonGameEntry>();

                        if (instance == null)
                        {
                            GameObject singletonObject = new(typeof(DungeonGameEntry).Name);
                            instance = singletonObject.AddComponent<DungeonGameEntry>();
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                    return instance;
                }
            }
        }

        private void InitInstance()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void SubscribleEvents()
        {
            m_DungeonSystem = gameObject.GetComponentInChildren<DungeonSystem>();
            m_MetropolisSystem = gameObject.GetComponentInChildren<MetropolisSystem>();
            m_UniversalSystem = gameObject.GetComponentInChildren<UniversalSystem>();

            m_DungeonSystem.gameObject.SetActive(false);
            m_MetropolisSystem.gameObject.SetActive(false);

            Event.Subscribe(OnPlayerSwitchToMetroplisEvent.EventId, OnSwitchToFactory);
            Event.Subscribe(OnPlayerSwitchToDungeonEvent.EventId, OnSwitchToDungeon);
            Event.Subscribe(OnProcedureInitGameMainLeaveEvent.EventId, OnProcedureInitGameMainLeaveEventHandler);
        }

        private void UnSubscribleEvents()
        {
            Event.Unsubscribe(OnPlayerSwitchToMetroplisEvent.EventId, OnSwitchToFactory);
            Event.Unsubscribe(OnPlayerSwitchToDungeonEvent.EventId, OnSwitchToDungeon);
            Event.Unsubscribe(OnProcedureInitGameMainLeaveEvent.EventId, OnProcedureInitGameMainLeaveEventHandler);
        }
        private void OnSwitchToFactory(object sender, GameEventArgs e)
        {
            m_DungeonSystem.gameObject.SetActive(false);
            m_MetropolisSystem.gameObject.SetActive(true);
        }

        private void OnSwitchToDungeon(object sender, GameEventArgs e)
        {
            m_DungeonSystem.gameObject.SetActive(true);
            m_MetropolisSystem.gameObject.SetActive(false);
        }

        private DungeonSystem m_DungeonSystem;
        private MetropolisSystem m_MetropolisSystem;
        private UniversalSystem m_UniversalSystem;
    }
}

