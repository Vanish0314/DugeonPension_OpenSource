using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class BuildModel : MonoBehaviour
    {
        public static BuildModel Instance { get; private set; }
        
        // 为每个建筑定义单独的事件
        public event Action OnCastleCountChanged;
        public event Action OnQuarryCountChanged;
        public event Action OnFactoryCountChanged;
        public event Action OnMonsterLairCountChanged;
        public event Action OnMonitorCountChanged;
        public event Action OnDormitoryCountChanged;
        public event Action OnTrapCountChanged;

        private int castleCount;
        public int CastleCount
        {
            get => castleCount;
            set
            {
                castleCount = value;
                OnCastleCountChanged?.Invoke();
            }
        }

        private int quarryCount;
        public int QuarryCount
        {
            get => quarryCount;
            set
            {
                quarryCount = value;
                OnQuarryCountChanged?.Invoke();
            }
        }

        private int factoryCount;
        public int FactoryCount
        {
            get => factoryCount;
            set
            {
                factoryCount = value;
                OnFactoryCountChanged?.Invoke();
            }
        }

        private int monsterLairCount;
        public int MonsterLairCount
        {
            get => monsterLairCount;
            set
            {
                monsterLairCount = value;
                OnMonsterLairCountChanged?.Invoke();
            }
        }
        
        private int monitorCount;
        public int MonitorCount
        {
            get => monitorCount;
            set
            {
                monitorCount = value;
                OnMonitorCountChanged?.Invoke();
            }
        }
        
        private int dormitoryCount;
        public int DormitoryCount
        {
            get => dormitoryCount;
            set
            {
                dormitoryCount = value;
                OnDormitoryCountChanged?.Invoke();
            }
        }
        
        private int trapCount;
        public int TrapCount
        {
            get => dormitoryCount;
            set
            {
                dormitoryCount = value;
                OnTrapCountChanged?.Invoke();
            }
        }
        private void Awake() {
            if (Instance == null) Instance = this;
        }
    }
}
