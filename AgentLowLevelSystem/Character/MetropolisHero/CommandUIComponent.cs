using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Dungeon
{
    public class CommandUIComponent : MonoBehaviour
    {
        [Header("主面板")]
        [SerializeField] private GameObject mainPanel;
        
        [Header("子面板")]
        [SerializeField] private GameObject commandPanel;
        [SerializeField] private GameObject interactPanel;
        [SerializeField] private GameObject infoPanel;

        [Header("主面板按钮")]
        [SerializeField] private Button commandButton;
        [SerializeField] private Button interactButton;
        [SerializeField] private Button infoButton;
        
        [Header("指令面板按钮")]
        [SerializeField] private Button quarryButton;
        [SerializeField] private Button loggingCampButton;
        [SerializeField] private Button farmButton;

        private MetropolisHeroBase targetHero;
        private float yOffset;

        public void Setup(MetropolisHeroBase target, float offset)
        {
            targetHero = target;
            yOffset = offset;

            // 主面板按钮绑定
            commandButton.onClick.AddListener(ShowCommandPanel);
            interactButton.onClick.AddListener(ShowInteractPanel);
            infoButton.onClick.AddListener(ShowInfoPanel);

            // 指令面板按钮绑定
            quarryButton.onClick.AddListener(() => SendCommand("quarry"));
            loggingCampButton.onClick.AddListener(() => SendCommand("loggingcamp"));
            farmButton.onClick.AddListener(() => SendCommand("farm"));

            // 默认显示主面板
            ShowMainPanel();
        }

        private void Update()
        {
            if (targetHero != null)
            {
                // UI跟随角色
                Vector3 uiPosition = targetHero.transform.position + Vector3.up * yOffset;
                mainPanel.transform.position = uiPosition;
                commandPanel.transform.position = uiPosition;
                interactPanel.transform.position = uiPosition;
                infoPanel.transform.position = uiPosition;
            }
        }

        #region 面板控制方法
        public void ShowMainPanel()
        {
            mainPanel.SetActive(true);
            commandPanel.SetActive(false);
            interactPanel.SetActive(false);
            infoPanel.SetActive(false);
        }

        private void ShowCommandPanel()
        {
            mainPanel.SetActive(false);
            commandPanel.SetActive(true);
            interactPanel.SetActive(false);
            infoPanel.SetActive(false);
        }

        private void ShowInteractPanel()
        {
            mainPanel.SetActive(false);
            commandPanel.SetActive(false);
            interactPanel.SetActive(true);
            infoPanel.SetActive(false);
        }

        private void ShowInfoPanel()
        {
            mainPanel.SetActive(false);
            commandPanel.SetActive(false);
            interactPanel.SetActive(false);
            infoPanel.SetActive(true);
        }
        #endregion

        private void SendCommand(string command)
        {
            targetHero.ReceiveCommand(command);
            gameObject.SetActive(false);
        }
    }
}
