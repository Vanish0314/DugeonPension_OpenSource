using System;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    [Serializable]
    public class BuildingCountSetting
    {
        public BuildingType buildingType;
        public int count;
    }
    
    public class BuildModel : MonoBehaviour
    {
        public static BuildModel Instance { get; private set; }
         
        [SerializeField] private BuildingCountSetting[] buildingCountSettings;

        // 统一建筑变更事件
        public event Action<BuildingType, int> OnBuildingCountChanged;

        private readonly Dictionary<BuildingType, int> m_BuildingCounts = new Dictionary<BuildingType, int>();

        private void Awake() 
        {
            if (Instance == null) Instance = this;
            
            // 初始化所有建筑数量
            foreach (var buildingCountSetting in buildingCountSettings)
            {
                m_BuildingCounts[buildingCountSetting.buildingType] = buildingCountSetting.count;
            }
        }
        
        [DebuggerComponent.DungeonGridWindow("重置建筑数量")]
        private static void ResetCount()
        {
            GameFrameworkLog.Debug("Reset count");
            foreach (var buildingCountSetting in Instance.buildingCountSettings)
            {
                Instance.m_BuildingCounts[buildingCountSetting.buildingType] = buildingCountSetting.count;
            }
        }

        private void OnDisable()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnBlueprintUnlockEventArgs.EventId, OnBlueprintUnlock);
        }

        public void Initialize()
        {
            Subscribe();
        }
        
        private void Subscribe()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnBlueprintUnlockEventArgs.EventId, OnBlueprintUnlock);
        }

        private void OnBlueprintUnlock(object sender, GameEventArgs e)
        {
            if (e is OnBlueprintUnlockEventArgs eventArgs)
            {
                var buildingType = (BuildingType)eventArgs.buildingID;

                SetCount(buildingType, eventArgs.buildingID is 0 or 1 or 2 ? 1 : int.MaxValue);
            }
        }

        public int GetCount(BuildingType type) => m_BuildingCounts[type];

        private void SetCount(BuildingType type, int value)
        {
            if (m_BuildingCounts[type] == value) return;
            
            m_BuildingCounts[type] = value;
            OnBuildingCountChanged?.Invoke(type, value);
        }

        public void ModifyCount(BuildingType type, int delta)
        {
            if(m_BuildingCounts[type] == int.MaxValue) return;
            SetCount(type, m_BuildingCounts[type] + delta);
        }
    }
}