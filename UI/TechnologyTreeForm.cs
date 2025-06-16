using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Dungeon
{
    public class TechnologyTreeForm : UGuiForm
    {
        [SerializeField] private GameObject blueprintPanel;
        [SerializeField] private Image buildingIcon;
        [SerializeField] private Text buildingName;
        [SerializeField] private Text buildingDescription;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Button returnButton;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI pointText;
        [SerializeField] private TextMeshProUGUI magicText;
        [SerializeField] private Slider magicSlider;
        
        [SerializeField] private BuildingBlueprintButton[] buildingBlueprintButtons;

        private void OnEnable()
        {
            unlockButton.onClick.AddListener(TechnologyTreeManager.Instance.UnlockBuildingBlueprint);
            returnButton.onClick.AddListener(OnReturnButtonClicked);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnBlueprintButtonClickedEventArgs.EventId,UpdateBlueprintUI);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnBlueprintUnlockEventArgs.EventId,UpdateButtonUI);
        }

        private void OnDisable()
        {
            unlockButton.onClick.RemoveListener(TechnologyTreeManager.Instance.UnlockBuildingBlueprint);
            returnButton.onClick.RemoveListener(OnReturnButtonClicked);
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnBlueprintButtonClickedEventArgs.EventId,UpdateBlueprintUI);
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnBlueprintUnlockEventArgs.EventId,UpdateButtonUI);
        }

        private void Update()
        {
            levelText.text = "科技树等级：" + TechnologyTreeManager.Instance.currentLevel;
            pointText.text = "研究点：" + TechnologyTreeManager.Instance.currentTechnologyPoint;
            
            var currentMagicPower = TechnologyTreeManager.Instance.currentMagicPower;
            var nextTechnologyPointMagicPower = TechnologyTreeManager.Instance.nextTechnologyPointMagicPower;
            magicSlider.value = (float)currentMagicPower/nextTechnologyPointMagicPower;
            magicText.text = $"魔能：{currentMagicPower}/{nextTechnologyPointMagicPower}";
        }

        private void OnReturnButtonClicked()
        {
            GameEntry.UI.GetUIForm(EnumUIForm.TechnologyTreeForm).Close();
        }

        private void UpdateBlueprintUI(object sender, GameEventArgs e)
        {
            if (e is OnBlueprintButtonClickedEventArgs eventArgs) 
                UpdateBlueprintUI(eventArgs.buildingID);
        }

        private void UpdateBlueprintUI(int buildingID)
        {
            blueprintPanel.SetActive(true);
            
            var buildingBlueprint = buildingBlueprintButtons[buildingID];
            BuildingBlueprintData buildingBlueprintData = buildingBlueprint.buildingBlueprintData;
            buildingIcon.sprite = buildingBlueprintData.BuildingIcon;
            buildingName.text = buildingBlueprintData.BuildingName;
            buildingDescription.text = buildingBlueprintData.BuildingDescription;
            if (buildingBlueprintData.IsUnlocked)
            {
                unlockButton.interactable = false;
                unlockButton.GetComponentInChildren<Text>().text = "已解锁";
            }
            else
            {
                unlockButton.interactable = true;
                unlockButton.GetComponentInChildren<Text>().text = "解锁";
            }
        }
        
        private void UpdateButtonUI(object sender, GameEventArgs e)
        {
            if(e is OnBlueprintUnlockEventArgs eventArgs)
                UpdateButtonUI(eventArgs.buildingID);
        }
        
        private void UpdateButtonUI(int buildingID)
        {
            var buildingBlueprint = buildingBlueprintButtons[buildingID];
            buildingBlueprint.GetComponent<Image>().color = Color.white;
            
            unlockButton.interactable = false;
            unlockButton.GetComponentInChildren<Text>().text = "已解锁";
        }
    }
}
