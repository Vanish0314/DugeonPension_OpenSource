using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    // TimeManager.cs
    public class TimeManager: MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }
    
        // 5分钟计时相关
        public event Action OnFiveMinutesElapsed;
        private float fiveMinuteTimer;
        private const float FiveMinutes = 5f; // 300秒=5分钟
    
        // 游戏暂停控制
        public bool IsPaused { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            fiveMinuteTimer = FiveMinutes;
        }
    
        private void Update()
        {
            if (IsPaused) return;
        
            fiveMinuteTimer -= Time.deltaTime;
            if (TimelineModel.Instance != null)
                TimelineModel.Instance.Timeline = fiveMinuteTimer / FiveMinutes;
        
            if (fiveMinuteTimer <= 0f)
            {
                fiveMinuteTimer = FiveMinutes;
                OnFiveMinutesElapsed?.Invoke();
            }
        }
    
        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
        }
        
        // 用于UI显示
        public float GetRemainingFiveMinuteTime()
        {
            return fiveMinuteTimer;
        }
    }
}
