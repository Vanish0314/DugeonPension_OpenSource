using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Evnents;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class StartFightButtonForm : UGuiForm
    {
        [SerializeField] private Button m_StartFightButton;

        private void Awake()
        {
            m_StartFightButton.onClick.AddListener(StartFight);
        }

        private void StartFight()
        {
            // 发送事件
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnHeroArrivedInDungeonEvent.Create(DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentMainHero()));
        }
    }
}
