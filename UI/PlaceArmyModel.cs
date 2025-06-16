using System;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    [Serializable]
    public class TrapCountSetting
    {
        public TrapType trapType;
        public int count;
    }
    
    [Serializable]
    public class MonsterCountSetting
    {
        public MonsterType monsterType;
        public int count;
    }

    public class PlaceArmyModel : MonoBehaviour
    {
        public static PlaceArmyModel Instance { get; private set; }
        
        [SerializeField] private TrapCountSetting[] trapCountSettings;
        [SerializeField] private MonsterCountSetting[] monsterCountSettings;

        public event Action<TrapType, int> OnTrapCountChanged;
        public event Action<MonsterType, int> OnMonsterCountChanged;

        private readonly Dictionary<TrapType, int> m_TrapCounts = new Dictionary<TrapType, int>();
        private readonly Dictionary<MonsterType, int> m_MonsterCounts = new Dictionary<MonsterType, int>();

        private void Awake() 
        {
            if (Instance == null) Instance = this;

            foreach (var trapType in trapCountSettings)
            {
                m_TrapCounts[trapType.trapType] = trapType.count;
            }

            foreach (var monsterType in monsterCountSettings)
            {
                m_MonsterCounts[monsterType.monsterType] = monsterType.count;
            }
        }
        
        public int GetTrapCount(TrapType type) => m_TrapCounts[type];
        public int GetMonsterCount(MonsterType type) => m_MonsterCounts[type];

        [DebuggerComponent.DungeonGridWindow("重置陷阱和魔物数量")]
        private static void ResetCount()
        {
            foreach (var trapType in Instance.trapCountSettings)
            {
                Instance.SetTrapCount(trapType.trapType, trapType.count);
            }

            foreach (var monsterType in Instance.monsterCountSettings)
            {
                Instance.SetMonsterCount(monsterType.monsterType, monsterType.count);
            }
        }
        
        public void SetTrapCount(TrapType type, int value)
        {
            if (m_TrapCounts[type] == value) return;
            
            m_TrapCounts[type] = value;
            OnTrapCountChanged?.Invoke(type, value);
        }

        public void SetMonsterCount(MonsterType type, int value)
        {
            if (m_MonsterCounts[type] == value) return;
            
            m_MonsterCounts[type] = value;
            OnMonsterCountChanged?.Invoke(type, value);
        }
    }
}
