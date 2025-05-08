using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dungeon
{
    [Serializable]
    public enum CropType
    {
        Corn,
        Wheat,
        Mushroom,
    }
    
    [Serializable]
    public class CropData
    {
        public CropType cropType;
        public float matureTime;    // 成熟时间（秒）
        public float waterInterval;// 浇水间隔（秒）
        public int yield;           // 单次产量
    }
    
    public class Farm : MetropolisBuildingBase
    {
        [Header("输入设置")] 
        [SerializeField] private InputReader m_inputReader;
        
        [Header("作物配置")]
        public CropData[] availableCrops;
        
        [Header("当前状态")]
        public CropData currentCrop;
        public float growthProgress;  // 生长进度（0-1）
        [SerializeField] private float waterCountdown;  // 浇水倒计时
        [SerializeField] private float waterTime;       //浇水需要的时间
        [SerializeField] private bool isWaterNeeded;    // 需要浇水
        [SerializeField] private bool isMature;         // 是否成熟
        
        [Header("UI设置")] 
        [SerializeField] private FarmUI farmUI; // UI预制体
        [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0); // UI显示偏移

        private bool isWatering; // 是否在浇水
        
        private float growthTimer;     // 累计生长时间
        private bool isGrowing;        // 是否在生长中

        #region Override

        protected override void OnEnable()
        {
            base.OnEnable();
            m_inputReader.OnBuildingClickedEvent += OnBuildingClicked;
            m_inputReader.OnNoBuildingClickedEvent += HideUI;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            m_inputReader.OnBuildingClickedEvent -= OnBuildingClicked;
            m_inputReader.OnNoBuildingClickedEvent -= HideUI;
        }
        
        #endregion
        
        #region UI

        private void OnBuildingClicked(GameObject clickedBuilding)
        {
            Debug.Log(clickedBuilding.name);
            if (CurrentState == BuildingState.Completed)
            {
                if (clickedBuilding != gameObject)
                {
                    HideUI();
                    return;
                }

                ShowUI();
            }
        }

        // 显示资源UI
        private void ShowUI()
        {
            if (farmUI != null)
            {
                farmUI.transform.position = transform.position + uiOffset;
                farmUI.ShowFarmUI();
            }
        }

        private void HideUI()
        {
            if (farmUI != null)
            {
                farmUI.CloseCurrentUI();
            }
        }

        #endregion
        
        #region 种植逻辑

        public CropData GetCropData(CropType type)
        {
            foreach (var crop in availableCrops)
            {
                if (crop.cropType == type) return crop;
            }
            return null;
        }

        public void PlantCrop(CropType type)
        {
            CropData data = GetCropData(type);
            if(data != null) PlantCrop(data);
        }
        
        // 开始种植新作物
        public void PlantCrop(CropData crop)
        {
            if (isGrowing) return;

            currentCrop = crop;
            ResetCropState();
            StartGrowing();
        }

        private void ResetCropState()
        {
            growthTimer = 0f;
            growthProgress = 0f;
            waterCountdown = currentCrop.waterInterval;
            isWaterNeeded = false;
            isMature = false;
        }

        private void StartGrowing()
        {
            isGrowing = true;
            StartCoroutine(GrowthRoutine());
        }

        private IEnumerator GrowthRoutine()
        {
            while (growthProgress < 1f)
            {
                if (!isWaterNeeded)
                {
                    growthTimer += Time.deltaTime;
                    growthProgress = growthTimer / currentCrop.matureTime;
                    
                    // 更新浇水倒计时
                    waterCountdown -= Time.deltaTime;
                    if (waterCountdown <= 0)
                    {
                        isWaterNeeded = true;
                        waterCountdown = currentCrop.waterInterval;
                        farmUI.ShowWarningUI();
                    }
                }
                yield return null;
            }

            OnCropMature();
        }

        private void OnCropMature()
        {
            isGrowing = false;
            isMature = true;
            farmUI.ShowGatherUI();
            Debug.Log($"{currentCrop.cropType}已成熟！");
        }
        #endregion

        #region 浇水

        private bool WaterCrop()
        {
            if (!isWaterNeeded || isWatering) return false;

            StartCoroutine(StartWaterCrop());
            return true;
        }

        private IEnumerator StartWaterCrop()
        {
            isWatering = true;
            
            yield return new WaitForSeconds(waterTime);
            isWaterNeeded = false;

            EndWaterCrop();
        }

        private void EndWaterCrop()
        {
            waterCountdown = currentCrop.waterInterval;
            isWatering = false;
            farmUI.CloseCurrentUI();
        }
        
        #endregion

        #region 收获作物
        
        public void Harvest()
        {
            if (!isMature) return;

            // 添加到全局资源
            ResourceModel.Instance.AddCrop(currentCrop.cropType, currentCrop.yield);
            Debug.Log($"收获{currentCrop.yield}个{currentCrop.cropType}");

            // 重置状态
            currentCrop = null;
            isMature = false;
            HideUI();
            StopAllCoroutines();
        }
        
        #endregion

        #region 建筑状态
        public override void StartCompletedBehavior()
        {
            // 建筑完成后初始化农田
            currentCrop = null;
        }

        public override void UpdateCompletedBehavior()
        {
            base.UpdateCompletedBehavior();

            if (workingHeroes.Count > 0)
            {
                WaterCrop();
            }
        }

        public override void Reset()
        {
            base.Reset();
            StopAllCoroutines();
            currentCrop = null;
            growthProgress = 0f;
            isWaterNeeded = false;
            isMature = false;
        }
        #endregion

        // 可视化调试
        private void OnGUI()
        {
            if (currentCrop == null) return;
            
            GUI.Label(new Rect(10, 100, 300, 20), 
                $"当前作物: {currentCrop.cropType} 进度: {growthProgress:P0}");
            if (isWaterNeeded)
                GUI.Label(new Rect(10, 120, 300, 20), "需要浇水！");
        }
    }
}