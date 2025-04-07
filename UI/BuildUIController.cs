using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class BuildUIController : MonoBehaviour
    {
        private BuildForm m_BuildForm;
        
        private BuildManager m_BuildManager;
         private void Start()
        {
            m_BuildForm = GetComponent<BuildForm>();
            m_BuildManager = FindObjectOfType<BuildManager>();

            //测试用-------------------------
            BuildModel.Instance.CastleCount = 1;
            BuildModel.Instance.QuarryCount = 2;
            BuildModel.Instance.FactoryCount = 3;
            BuildModel.Instance.MonsterLairCount = 4;
            BuildModel.Instance.MonitorCount = 5;
            BuildModel.Instance.DormitoryCount = 6;
            BuildModel.Instance.TrapCount = 7;
            m_BuildManager.OnBuildCompleted += UpdateBuildingCount;
            GameEntry.Event.GetComponent<EventComponent>()
                .Subscribe(OnFightSceneLoadEventArgs.EventId, ShowPlaceArmyUI);
            
            // 绑定按钮点击事件（通过Button名字关联 buildingData）
            m_BuildForm.castleButton.onClick.AddListener(() => OnBuildingButtonClick(m_BuildForm.castleButton.name));
            m_BuildForm.quarryButton.onClick.AddListener(() => OnBuildingButtonClick(m_BuildForm.quarryButton.name));
            m_BuildForm.factoryButton.onClick.AddListener(() => OnBuildingButtonClick(m_BuildForm.factoryButton.name));
            m_BuildForm.monsterLairButton.onClick.AddListener(() => OnBuildingButtonClick(m_BuildForm.monsterLairButton.name));
            m_BuildForm.monitorButton.onClick.AddListener(() => OnBuildingButtonClick(m_BuildForm.monitorButton.name));
            m_BuildForm.dormitoryButton.onClick.AddListener(() => OnBuildingButtonClick(m_BuildForm.dormitoryButton.name));
            m_BuildForm.trapButton.onClick.AddListener(() => OnBuildingButtonClick(m_BuildForm.trapButton.name));
            // ... 其他Button ...
            
            // 订阅事件
            BuildModel.Instance.OnCastleCountChanged += UpdateCastleUI;
            BuildModel.Instance.OnQuarryCountChanged += UpdateQuarryUI;
            BuildModel.Instance.OnFactoryCountChanged += UpdateFactoryUI;
            BuildModel.Instance.OnMonsterLairCountChanged += UpdateMonsterLairUI;
            BuildModel.Instance.OnMonitorCountChanged += UpdateMonitorUI;
            BuildModel.Instance.OnDormitoryCountChanged += UpdateDormitoryUI;
            BuildModel.Instance.OnTrapCountChanged += UpdateTrapUI;

            // 初始化UI
            RefreshAllUI();
        }

        private void OnDestroy()
        {
            if (BuildModel.Instance != null)
            {
                // 取消订阅
                m_BuildManager.OnBuildCompleted -= UpdateBuildingCount;
                
                BuildModel.Instance.OnCastleCountChanged -= UpdateCastleUI;
                BuildModel.Instance.OnQuarryCountChanged -= UpdateQuarryUI;
                BuildModel.Instance.OnFactoryCountChanged -= UpdateFactoryUI;
                BuildModel.Instance.OnMonsterLairCountChanged -= UpdateMonsterLairUI;
                BuildModel.Instance.OnMonitorCountChanged -= UpdateMonitorUI;
                BuildModel.Instance.OnDormitoryCountChanged -= UpdateDormitoryUI;
                BuildModel.Instance.OnTrapCountChanged -= UpdateTrapUI;
            }
        }

        // 通过建筑名字获取buildingData
        private void OnBuildingButtonClick(string buildingName)
        {
            BuildingData buildingData = m_BuildManager.GetBuildingData(buildingName);
        }

        // 根据建筑名字作为id写入对应count数据
        private void UpdateBuildingCount(BuildingData buildingData)//-----------------暂时
        {
            switch (buildingData.buildingId)
            {
                case "Castle":
                    BuildModel.Instance.CastleCount--;
                    break;
                case "Quarry":
                    BuildModel.Instance.QuarryCount--;
                    break;
                case "Factory":
                    BuildModel.Instance.FactoryCount--;
                    break;
                case "MonsterLair":
                    BuildModel.Instance.MonsterLairCount--;
                    break;
                case "Monitor":
                    BuildModel.Instance.MonitorCount--;
                    break;
                case "Dormitory":
                    BuildModel.Instance.DormitoryCount--;
                    break;
                case "Trap":
                    BuildModel.Instance.TrapCount--;
                    break;
                default:
                    break;
            }
        }
        
        private void UpdateCastleUI()
        {
            m_BuildForm.UpdateCastleUI(BuildModel.Instance.CastleCount);
        }
        
        private void UpdateQuarryUI()
        {
            m_BuildForm.UpdateQuarryUI(BuildModel.Instance.QuarryCount);
        }

        private void UpdateFactoryUI()
        {
            m_BuildForm.UpdateFactoryUI(BuildModel.Instance.FactoryCount);
        }

        private void UpdateMonsterLairUI()
        {
            m_BuildForm.UpdateMonsterLairUI(BuildModel.Instance.MonsterLairCount);
        }

        private void UpdateMonitorUI()
        {
            m_BuildForm.UpdateMonitorUI(BuildModel.Instance.MonitorCount);
        }

        private void UpdateDormitoryUI()
        {
            m_BuildForm.UpdateDormitoryUI(BuildModel.Instance.DormitoryCount);
        }

        private void UpdateTrapUI()
        {
            m_BuildForm.UpdateTrapUI(BuildModel.Instance.TrapCount);
        }

        private void ShowPlaceArmyUI(object sender, GameEventArgs e)
        {
            m_BuildForm.PlaceArmyUI();
        }

        private void ShowBuildUI()
        {
            m_BuildForm.BuildUI();
        }
        
        // 初始化时刷新所有UI
        private void RefreshAllUI()
        {
            UpdateCastleUI();
            UpdateQuarryUI();
            UpdateFactoryUI();
            UpdateMonsterLairUI();
            UpdateMonitorUI();
            UpdateDormitoryUI();
        }
    }
}
