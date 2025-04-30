using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    // TimeManager.cs
    public class TimeManager: MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }
    
        // 5分钟计时相关
        public event Action OnFiveMinutesElapsed;
        [SerializeField] private float fiveMinuteTimer;
        private const float FiveMinutes = 20f; // 300秒=5分钟
    
        // 游戏暂停控制
        public bool IsPaused { get; private set; }
        private bool _isFiveMinute;
        private int m_Flag = 0;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            if (m_Flag == 0)
            {
                m_Flag = 1;
            }
            else if (m_Flag == 1)
            {
                Subscribe();
            }
        }
        
        public void Subscribe()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnSceneLoadedEventArgs.EventId, OnSceneLoaded);
        }
        private void OnDisable()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnSceneLoadedEventArgs.EventId,OnSceneLoaded);
        }
        private void Update()
        {
            if (IsPaused) return;

            if (_isFiveMinute)
            {
                fiveMinuteTimer -= Time.deltaTime;
                if (TimelineModel.Instance != null)
                    TimelineModel.Instance.Timeline = fiveMinuteTimer / FiveMinutes;
        
                if (fiveMinuteTimer <= 0f)
                {
                    OnFiveMinutesElapsed?.Invoke();
                    _isFiveMinute = false;
                }
            }
        }
    
        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
        }
        
        private void OnSceneLoaded(object sender, GameEventArgs e)
        {
            OnSceneLoadedEventArgs sceneLoadedEventArgs = e as OnSceneLoadedEventArgs;
            if (sceneLoadedEventArgs.SceneID == 2)
            {
                fiveMinuteTimer = FiveMinutes;
                _isFiveMinute = true;
            }
        }
        
        // 用于UI显示
        public float GetRemainingFiveMinuteTime()
        {
            return fiveMinuteTimer;
        }
    }
}
