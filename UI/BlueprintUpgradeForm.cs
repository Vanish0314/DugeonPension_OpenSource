using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class BlueprintUpgradeForm : UGuiForm
    {
        [SerializeField] private GameObject blueprintPanel;
        [SerializeField] private Image blueprintIcon;
        [SerializeField] private Text blueprintName;
        [SerializeField] private Text blueprintDescription;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button returnButton;
        
        [SerializeField] private UpgradeBlueprintButton[] upgradeBlueprintButtons;

        private void OnEnable()
        {
            unlockButton.onClick.AddListener(UpgradeBlueprintManager.Instance.UnlockUpgradeBlueprint);
            upgradeButton.onClick.AddListener(UpgradeBlueprintManager.Instance.UpgradeUpgradeBlueprint);
            returnButton.onClick.AddListener(OnReturnButtonClicked);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnUpgradeBlueprintButtonClickedEventArgs.EventId,UpdateBlueprintUI);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnUpgradeBlueprintUnlockEventArgs.EventId,UpdateBlueprintUI);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnUpgradeBlueprintUpgradeEventArgs.EventId,UpdateBlueprintUI);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnUpgradeBlueprintUnlockEventArgs.EventId,UpdateButtonUI);
        }

        private void OnDisable()
        {
            unlockButton.onClick.RemoveListener(UpgradeBlueprintManager.Instance.UnlockUpgradeBlueprint);
            upgradeButton.onClick.RemoveListener(UpgradeBlueprintManager.Instance.UpgradeUpgradeBlueprint);
            returnButton.onClick.RemoveListener(OnReturnButtonClicked);
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnUpgradeBlueprintButtonClickedEventArgs.EventId,UpdateBlueprintUI);
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnUpgradeBlueprintUnlockEventArgs.EventId,UpdateBlueprintUI);
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnUpgradeBlueprintUpgradeEventArgs.EventId,UpdateBlueprintUI);
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnUpgradeBlueprintUnlockEventArgs.EventId,UpdateButtonUI);
        }

        protected virtual void OnReturnButtonClicked()
        {
            GameEntry.UI.GetUIForm(EnumUIForm.BlueprintUpgradeForm).Close();
        }

        private void UpdateBlueprintUI(object sender, GameEventArgs e)
        {
            if (e is OnUpgradeBlueprintButtonClickedEventArgs eventArgs) 
                UpdateBlueprintUI(eventArgs.blueprintID);
            else if (e is OnUpgradeBlueprintUnlockEventArgs eventArgs1)
            {
                if (eventArgs1.isMonster)
                {
                    PlaceArmyModel.Instance.SetMonsterCount((MonsterType)eventArgs1.blueprintID, int.MaxValue);
                }
                else
                {
                    PlaceArmyModel.Instance.SetTrapCount((TrapType)eventArgs1.blueprintID, int.MaxValue);
                }
                
                UpdateBlueprintUI(eventArgs1.blueprintID);
            }
            else if (e is OnUpgradeBlueprintUpgradeEventArgs eventArgs2)
                UpdateBlueprintUI(eventArgs2.blueprintID);
        }

        private void UpdateBlueprintUI(int blueprintID)
        {
            blueprintPanel.SetActive(true);
            
            var upgradeBlueprintButton = upgradeBlueprintButtons[blueprintID];
            UpgradeBlueprintData blueprintData = upgradeBlueprintButton.blueprintData;
            
            blueprintIcon.sprite = blueprintData.blueprintIcon;
            blueprintName.text = blueprintData.blueprintName;

            if (blueprintData.isUnlocked)
            {
                upgradeButton.gameObject.SetActive(true);
                unlockButton.gameObject.SetActive(false);
            }
            else
            {
                upgradeButton.gameObject.SetActive(false);
                unlockButton.gameObject.SetActive(true);
            }
            
            switch (blueprintData.blueprintLevel)
            {
                case 0:
                    blueprintDescription.text = blueprintData.blueprintDescription0;
                    break;
                case 1:
                    blueprintDescription.text = blueprintData.blueprintDescription1;
                    break;
                case 2:
                    blueprintDescription.text = blueprintData.blueprintDescription2;
                    break;
                case 3:
                    blueprintDescription.text = blueprintData.blueprintDescription3;
                    upgradeButton.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
            
        }
        
        private void UpdateButtonUI(object sender, GameEventArgs e)
        {
            if(e is OnUpgradeBlueprintUnlockEventArgs eventArgs)
                UpdateButtonUI(eventArgs.blueprintID);
        }
        
        private void UpdateButtonUI(int buildingID)
        {
            var buildingBlueprint = upgradeBlueprintButtons[buildingID];
            buildingBlueprint.GetComponent<Image>().color = Color.white;
        }
    }
}
