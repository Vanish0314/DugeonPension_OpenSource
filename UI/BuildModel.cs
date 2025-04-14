using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class BuildModel : MonoBehaviour
    {
        public static BuildModel Instance { get; private set; }
        
        [SerializeField] private BuildingType[] buildingTypes;

        // 统一建筑变更事件
        public event Action<BuildingType, int> OnBuildingCountChanged;

        private readonly Dictionary<BuildingType, int> m_BuildingCounts = new Dictionary<BuildingType, int>();

        private void Awake() 
        {
            if (Instance == null) Instance = this;
            
            // 初始化所有建筑数量
            foreach (BuildingType type in buildingTypes)
            {
                m_BuildingCounts[type] = 0;
            }
        }

        public int GetCount(BuildingType type) => m_BuildingCounts[type];

        public void SetCount(BuildingType type, int value)
        {
            if (m_BuildingCounts[type] == value) return;
            
            m_BuildingCounts[type] = value;
            OnBuildingCountChanged?.Invoke(type, value);
        }

        public void ModifyCount(BuildingType type, int delta)
        {
            SetCount(type, m_BuildingCounts[type] + delta);
        }
    }
}