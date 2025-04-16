using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class GameStartForm : UGuiForm
    {
        [SerializeField] private Button m_StartGameButton;
        [SerializeField] private Button m_QuitGameButton;

        private void Awake()
        {
            m_StartGameButton.onClick.AddListener(StartGame);
            m_QuitGameButton.onClick.AddListener(QuitGame);
        }

        private void StartGame()
        {
            // 发送事件
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnStartNewGameButtonClickEvent.Create());
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}
