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

            SubscribleEvents();

            m_DungeonSystem = gameObject.GetComponent<DungeonSystem>();
            m_MetropolisSystem = gameObject.GetComponent<MetropolisSystem>();
            m_UniversalSystem = gameObject.GetComponent<UniversalSystem>();
        }

        private void SubscribleEvents()
        {
            Event.Subscribe(OnSwitchToFactoryEvent.EventId, OnSwitchToFactory);
            Event.Subscribe(OnSwitchToDungeonEvent.EventId, OnSwitchToDungeon);
        }


        private void UnSubscribleEvents()
        {
            Event.Unsubscribe(OnSwitchToFactoryEvent.EventId, OnSwitchToFactory);
            Event.Unsubscribe(OnSwitchToDungeonEvent.EventId, OnSwitchToDungeon);
        }
        private void OnSwitchToFactory(object sender, GameEventArgs e)
        {
            m_DungeonSystem.enabled = false;
            m_MetropolisSystem.enabled = false;
        }

        private void OnSwitchToDungeon(object sender, GameEventArgs e)
        {
            m_DungeonSystem.enabled = true;
            m_MetropolisSystem.enabled = false;
        }

        private DungeonSystem m_DungeonSystem;
        private MetropolisSystem m_MetropolisSystem;
        private UniversalSystem m_UniversalSystem;
    }
}


#region Events
namespace Dungeon.Evnents
{
    public sealed class OnSwitchToFactoryEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnSwitchToFactoryEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnSwitchToFactoryEvent Create()
        {
            OnSwitchToFactoryEvent onStartFightButtonClickEventArgs = ReferencePool.Acquire<OnSwitchToFactoryEvent>();
            return onStartFightButtonClickEventArgs;
        }

        public override void Clear()
        {
        }
    }

    public sealed class OnSwitchToDungeonEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnSwitchToDungeonEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnSwitchToDungeonEvent Create()
        {
            OnSwitchToDungeonEvent onSwitchToDungeonEventArgs = ReferencePool.Acquire<OnSwitchToDungeonEvent>();
            return onSwitchToDungeonEventArgs;
        }

        public override void Clear()
        {
        }
    }
}

#endregion