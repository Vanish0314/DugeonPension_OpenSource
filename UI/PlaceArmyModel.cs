using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class PlaceArmyModel : MonoBehaviour
    {
        public static PlaceArmyModel Instance { get; private set; }
        
        [SerializeField] private TrapType[] trapTypes;
        [SerializeField] private MonsterType[] monsterTypes;

        public event Action<TrapType, int> OnTrapCountChanged;
        public event Action<MonsterType, int> OnMonsterCountChanged;

        private readonly Dictionary<TrapType, int> m_TrapCounts = new Dictionary<TrapType, int>();
        private readonly Dictionary<MonsterType, int> m_MonsterCounts = new Dictionary<MonsterType, int>();

        private void Awake() 
        {
            if (Instance == null) Instance = this;
            
            foreach (TrapType type in trapTypes)
            {
                m_TrapCounts[type] = 0;
            }

            foreach (MonsterType type in monsterTypes)
            {
                m_MonsterCounts[type] = 0;
            }
        }

        public int GetTrapCount(TrapType type) => m_TrapCounts[type];
        public int GetMonsterCount(MonsterType type) => m_MonsterCounts[type];

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

        public void ModifyTrapCount(TrapType type, int delta)
        {
            SetTrapCount(type, m_TrapCounts[type] + delta);
        }

        public void ModifyMonsterCount(MonsterType type, int delta)
        {
            SetMonsterCount(type, m_MonsterCounts[type] + delta);
        }
    }
}
