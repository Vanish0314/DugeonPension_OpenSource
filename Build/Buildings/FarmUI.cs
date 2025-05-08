using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dungeon
{
    [Serializable]
    public struct SeedButton
    {
        public CropType cropType;
        public Button cropSeedButton;
    }
    public class FarmUI : MonoBehaviour
    {
        [Header("种植面板")]
        [SerializeField] private GameObject plantUIPrefab;
        [SerializeField] private SeedButton[] seedButtons;
       
        [Header("种子状态")]
        public GameObject statusUIPrefab;
        public Slider statusSlider;
        
        public Button gatherButtonPrefab;// 收获按钮
        
        [Header("缺水提醒")]
        public GameObject warningUIPrefab;

        private Farm targetFarm;
        private GameObject currentUI;

        private void Start()
        {
            targetFarm = transform.parent.GetComponent<Farm>();
            InitSeedButtons();
        }

        private void Update()
        {
            if (targetFarm.currentCrop != null)
            {
                statusSlider.value = targetFarm.growthProgress;
            }
        }

        private void InitSeedButtons()
        {
            foreach (var seedButton in seedButtons)
            {
                seedButton.cropSeedButton.onClick.AddListener(() => OnSeedSelected(seedButton.cropType));
            }
            
            gatherButtonPrefab.onClick.AddListener(() => targetFarm.Harvest());
        }

        // 点击时调用的
        public void ShowFarmUI()
        {
            CloseCurrentUI();

            if (targetFarm.currentCrop == null)
            {
                ShowPlantUI();
            }
            else
            {
                ShowStatusUI();
            }
        }

        private void ShowPlantUI()
        {
            currentUI = plantUIPrefab;
            currentUI.SetActive(true);
            
            // 根据各种种子的数量判断要active哪些具体的按钮
            foreach (var seedButton in seedButtons)
            {
                if (ResourceModel.Instance.GetSeedCount(seedButton.cropType) > 0)
                {
                    seedButton.cropSeedButton.interactable = true;
                }
                else
                {
                    seedButton.cropSeedButton.interactable = false;
                }
            }
        }

        private void ShowStatusUI()
        {
            currentUI = statusUIPrefab;
            currentUI.SetActive(true);
        }

        // 成熟时自动调用
        public void ShowGatherUI()
        {
            gatherButtonPrefab.gameObject.SetActive(true);
        }

        // 缺水时自动调用的
        public void ShowWarningUI()
        {
            CloseCurrentUI();
            currentUI = warningUIPrefab;
            currentUI.SetActive(true);
        }

        private void OnSeedSelected(CropType cropType)
        {
            // 根据种子button的类型来调用下面这个函数
            targetFarm.PlantCrop(cropType);
            CloseCurrentUI();
        }

        public void CloseCurrentUI()
        {
            if (currentUI != null)
            {
                currentUI.SetActive(false);
            }
        }
    }
}