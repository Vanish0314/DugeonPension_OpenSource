using System;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class PlaceArmyController : MonoBehaviour
    {
        [System.Serializable]
        public class TrapTypeButton
        {
            public TrapType type;
            public Button button;
        }

        [System.Serializable]
        public class MonsterTypeButton
        {
            public MonsterType type;
            public Button button;
        }
        
        [SerializeField] private Button trapOption;
        [SerializeField] private Button monsterOption;
        
        [SerializeField] private TrapTypeButton[] trapButtons;
        [SerializeField] private MonsterTypeButton[] monsterTypeButtons;
        
        private PlaceArmyForm m_PlaceArmyForm;
        private PlaceManager m_PlaceManager;
        
        private void Awake()
        {
            m_PlaceArmyForm = GetComponent<PlaceArmyForm>();
            m_PlaceManager = PlaceManager.Instance;

            // 测试用数据
            foreach (var btn in trapButtons)
            {
                PlaceArmyModel.Instance.SetTrapCount(btn.type, 99);
            }

            foreach (var btn in monsterTypeButtons)
            {
                PlaceArmyModel.Instance.SetMonsterCount(btn.type, 99);
            }
            
            InitializeButtons();
        }

        private void OnEnable()
        {
            m_PlaceArmyForm.ShowTrapPanel();
            SubscribeEvents();
            RefreshAllUI();
        }

        private void InitializeButtons()
        {
            trapOption.onClick.AddListener(() => m_PlaceArmyForm.ShowTrapPanel());
            monsterOption.onClick.AddListener(() => m_PlaceArmyForm.ShowMonsterPanel());
            
            foreach (var btn in trapButtons)
            {
                btn.button.onClick.AddListener(() => OnTrapButtonClick(btn.type));
            }

            foreach (var btn in monsterTypeButtons)
            {
                btn.button.onClick.AddListener(() => OnMonsterButtonClick(btn.type));
            }
        }

        private void SubscribeEvents()
        {
            PlaceArmyModel.Instance.OnTrapCountChanged += UpdateTrapUI;
            PlaceArmyModel.Instance.OnMonsterCountChanged += UpdateMonsterUI;
            GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnTrapPlacedEventArgs.EventId,ReduceTrapCount);
            GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnMonsterPlacedEventArgs.EventId,ReduceMonsterCount);
        }

        private void OnDisable()
        {
            if (PlaceArmyModel.Instance != null)
            {
                PlaceArmyModel.Instance.OnTrapCountChanged -= UpdateTrapUI;
                PlaceArmyModel.Instance.OnMonsterCountChanged -= UpdateMonsterUI;
                GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnTrapPlacedEventArgs.EventId,ReduceTrapCount);
                GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnMonsterPlacedEventArgs.EventId,ReduceMonsterCount);
            }
        }

        private void OnTrapButtonClick(TrapType type)
        {
            m_PlaceManager.SelectTrapData(type);
        }

        private void OnMonsterButtonClick(MonsterType type)
        {
            m_PlaceManager.SelectMonsterData(type);
        }

        private void ReduceTrapCount(object sender, GameEventArgs gameEventArgs)
        {
            OnTrapPlacedEventArgs e = (OnTrapPlacedEventArgs)gameEventArgs;
            PlaceArmyModel.Instance.ModifyTrapCount(e.TrapData.trapType, -1);
        }

        private void ReduceMonsterCount(object sender, GameEventArgs gameEventArgs)
        {
            OnMonsterPlacedEventArgs e = (OnMonsterPlacedEventArgs)gameEventArgs;
            PlaceArmyModel.Instance.ModifyMonsterCount(e.MonsterData.monsterType, -1);
        }

        private void UpdateTrapUI(TrapType type, int count)
        {
            m_PlaceArmyForm.UpdateTrapUI(type, count);
        }

        private void UpdateMonsterUI(MonsterType type, int count)
        {
            m_PlaceArmyForm.UpdateMonsterUI(type, count);
        }

        private void RefreshAllUI()
        {
            foreach (var btn in trapButtons)
            {
                UpdateTrapUI(btn.type, PlaceArmyModel.Instance.GetTrapCount(btn.type));
            }

            foreach (var btn in monsterTypeButtons)
            {
                UpdateMonsterUI(btn.type, PlaceArmyModel.Instance.GetMonsterCount(btn.type));
            }
        }
        
    }
}
