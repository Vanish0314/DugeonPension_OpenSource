using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public enum BuildingType
    {
        Castle,
        Quarry,
        Factory,
        MonsterLair,
        Monitor,
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
            if (!m_BuildingUIDict.TryGetValue(type, out var buildingUI))
            {
                // Debug.LogError($"BuildingUI not found for type: {type}");
                return;
            }

            buildingUI.countText.text = $"X {count}";
            buildingUI.button.gameObject.SetActive(count > 0);
        }
    }
}