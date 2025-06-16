using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Evnents;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    // TimeManager.cs
    public class TimeManager: MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [Header("波次计时器")] 
        [SerializeField] private float firstReachCutdown = 3f;
        [SerializeField] private float defaultReachCutdown = 100f;
        [SerializeField] private float cutdownTimer;
        
        private float cutdown = 3f;
    
        // 游戏暂停控制
        public bool IsPaused { get; private set; } = false;
        private bool _heroIsReaching;
        private int m_Flag = 0;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            Subscribe();
        }

        private void OnEnable()
        {
            if (m_Flag == 0)
            {
                cutdown = firstReachCutdown;
                m_Flag = 1;
            }
            else if (m_Flag == 1)
            {
                cutdown = defaultReachCutdown;
                Subscribe();
            }
        }
        
        public void Subscribe()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnSwitchedToMetroplisProcedureEvent.EventId, OnSwitchedToMetroplis);
        }
        private void OnDisable()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnSwitchedToMetroplisProcedureEvent.EventId,OnSwitchedToMetroplis);
        }
        private void Update()
        {
            if (IsPaused) return;

            if (_heroIsReaching)
            {
                cutdownTimer -= Time.deltaTime;
                if (TimelineModel.Instance != null)
                    TimelineModel.Instance.Timeline = cutdownTimer / cutdown;
        
                if (cutdownTimer <= 0f)
                {
                    DungeonGameEntry.DungeonGameEntry.Event.Fire(this,OnHeroReachEventArgs.Create());
                    _heroIsReaching = false;
                }
            }
        }
    
        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
        }
        
        private void OnSwitchedToMetroplis(object sender, GameEventArgs e)
        {
            cutdownTimer = cutdown;
            _heroIsReaching = true;
        }
    }

    public class OnHeroReachEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnHeroReachEventArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnHeroReachEventArgs Create()
        {
            OnHeroReachEventArgs onHeroReachEventArgs = ReferencePool.Acquire<OnHeroReachEventArgs>();
            return onHeroReachEventArgs;
        }

        public override void Clear()
        {
            
        }
    }
}
