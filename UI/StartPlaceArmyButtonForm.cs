using System.Collections;
using System.Collections.Generic;
using Dungeon.Evnents;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class StartPlaceArmyButtonForm : UGuiForm
    {
        [SerializeField] private Button m_StartPlaceArmyButton;

        private void Awake()
        {
            m_StartPlaceArmyButton.onClick.AddListener(StartPlaceArmy);
        }

        public void StartPlaceArmy()
        {
            // 发送事件
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnPlayerSwitchToDungeonEvent.Create());
        }
    }
}
