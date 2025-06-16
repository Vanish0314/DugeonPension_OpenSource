using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Evnents;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class GameOverForm : UGuiForm
    {
        [SerializeField] private Button m_ReturnGameStartButton;
        [SerializeField] private Button m_QuitGameButton;

        private void Awake()
        {
            m_ReturnGameStartButton.onClick.AddListener(ReturnGameStart);
            m_QuitGameButton.onClick.AddListener(QuitGame);
        }

        private void ReturnGameStart()
        {
            // 发送事件
            GameEntry.Event.Fire(this, OnReturnGameStartButtonClickEvent.Create());
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}
