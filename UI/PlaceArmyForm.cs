using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public enum TrapType
    {
        Spike
    }

    [System.Serializable]
    public class TrapUI
    {
        public TrapType type;
        public Button button;
        public Text countText;
    }
    
    public enum MonsterType
    {
        Slime
    }
    
    [System.Serializable]
    public class MonsterUI
    {
        public MonsterType type;
        public Button button;
        public Text countText;
    }
    public class PlaceArmyForm : UGuiForm
    {
        [SerializeField] private GameObject mTrapPanel;
        [SerializeField] private GameObject mMonsterPanel;
        
        [SerializeField]private List<TrapUI> trapUIs = new List<TrapUI>();
        [SerializeField]private List<MonsterUI> monsterUIs = new List<MonsterUI>();

        private Dictionary<TrapType, TrapUI> m_TrapUIDict;
        private Dictionary<MonsterType, MonsterUI> m_MonsterUIDict;

        private void Awake()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            m_TrapUIDict = new Dictionary<TrapType, TrapUI>();
            foreach (var trapUI in trapUIs)
            {
                if (!m_TrapUIDict.TryAdd(trapUI.type, trapUI))
                {
                    GameFrameworkLog.Error($"Duplicate entry for {trapUI.type} in trapUIs list!");
                    continue;
                }
            }
            
            m_MonsterUIDict = new Dictionary<MonsterType, MonsterUI>();
            foreach (var monsterUI in monsterUIs)
            {
                if (!m_MonsterUIDict.TryAdd(monsterUI.type, monsterUI))
                {
                    GameFrameworkLog.Error($"Duplicate entry for {monsterUI.type} in trapUIs list!");
                    continue;
                }
            }
        }

        public void UpdateTrapUI(TrapType type, int count)
        {
            if (!m_TrapUIDict.TryGetValue(type, out var trapUI))
            {
                GameFrameworkLog.Error($"TrapUI not found for type: {type}");
                return;
            }

            trapUI.countText.text = $"X {count}";
            trapUI.button.gameObject.SetActive(count > 0);
        }

        public void UpdateMonsterUI(MonsterType type, int count)
        {
            if (!m_MonsterUIDict.TryGetValue(type, out var monsterUI))
            {
                GameFrameworkLog.Error($"MonsterUI not found for type: {type}");
                return;
            }
            
            monsterUI.countText.text = $"X {count}";
            monsterUI.button.gameObject.SetActive(count > 0);
        }

        public void ShowTrapPanel()
        {
            mTrapPanel.SetActive(true);
            mMonsterPanel.SetActive(false);
        }

        public void ShowMonsterPanel()
        {
            mMonsterPanel.SetActive(true);
            mTrapPanel.SetActive(false);
        }
    }
}
