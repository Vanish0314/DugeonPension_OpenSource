using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class TimelineModel : MonoBehaviour
    {
        public static TimelineModel Instance { get; private set; }
        
        public event Action OnTimelineChanged;

        private float timeline = 0;
        public float Timeline
        {
            get => timeline;
            set
            {
                timeline = value;
                OnTimelineChanged?.Invoke();
            }
        }

        private void Awake() {
            if (Instance == null) Instance = this;
        }
    }
}
