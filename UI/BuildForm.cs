using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public enum BuildingType
    {
        Castle,
        MonsterLair,
        TrapFactory,
        Quarry,
        LoggingCamp,
        FarmLand,
        Canteen,
        Dormitory,
    }

    [System.Serializable]
    public class BuildingUI
    {
        public BuildingType type;
        public Button button;
        public Text countText;
    }

    public class BuildForm : UGuiForm
    {
        [SerializeField]
        private List<BuildingUI> buildingUIs = new List<BuildingUI>();

        private Dictionary<BuildingType, BuildingUI> m_BuildingUIDict;

        private void Awake()
        {
            InitializeBuildingDictionary();
        }

        private void InitializeBuildingDictionary()
        {
            m_BuildingUIDict = new Dictionary<BuildingType, BuildingUI>();
            foreach (var buildingUI in buildingUIs)
            {
                if (!m_BuildingUIDict.TryAdd(buildingUI.type, buildingUI))
                {
                    // Debug.LogError($"Duplicate entry for {buildingUI.type} in buildingUIs list!");
                    continue;
                }
            }
        }
        
        public void UpdateBuildingUI(BuildingType type, int count)
        {
            if (!m_BuildingUIDict.TryGetValue(type, out var buildingUI)) return;

            // 处理无限数量显示
            buildingUI.countText.text = count == int.MaxValue ? "∞" : $"X {count}";
            
            // 按钮可用条件：数量大于0 或 无限数量
            buildingUI.button.gameObject.SetActive(count > 0 || count == int.MaxValue);
        }
    }
}