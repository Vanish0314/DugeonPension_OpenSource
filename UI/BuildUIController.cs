using System;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class BuildUIController : MonoBehaviour
    {
        [System.Serializable]
        public class BuildingTypeButton
        {
            public BuildingType type;
            public Button button;
        }
        
        [SerializeField] private BuildingTypeButton[] buildingButtons;
        
        private BuildForm m_BuildForm;
        private PlaceManager m_PlaceManager;

        
        private void Awake()
        {
            m_BuildForm = GetComponent<BuildForm>();
            m_PlaceManager = PlaceManager.Instance;

            // 测试用数据
            foreach (var btn in buildingButtons)
            {
                BuildModel.Instance.SetCount(btn.type, (int)btn.type + 1);
            }

            InitializeButtons();
        }
        
        private void OnEnable()
        {
            SubscribeEvents();
            RefreshAllUI();
        }
        

        private void InitializeButtons()
        {
            foreach (var btn in buildingButtons)
            {
                btn.button.onClick.AddListener(() => OnBuildingButtonClick(btn.type));
            }
        }

        private void SubscribeEvents()
        {
            BuildModel.Instance.OnBuildingCountChanged += UpdateBuildingUI;
            m_PlaceManager.OnBuildingPlaced += ReduceBuildingCount;
        }

        private void OnDisable()
        {
            if (BuildModel.Instance != null)
            {
                BuildModel.Instance.OnBuildingCountChanged -= UpdateBuildingUI;
                m_PlaceManager.OnBuildingPlaced -= ReduceBuildingCount;
            }
        }

        private void OnBuildingButtonClick(BuildingType type)
        {
            m_PlaceManager.SelectBuildingData(type);
        }

        private void ReduceBuildingCount(BuildingData buildingData)
        {
            BuildModel.Instance.ModifyCount(buildingData.buildingType, -1);
        }

        private void UpdateBuildingUI(BuildingType type, int count)
        {
            m_BuildForm.UpdateBuildingUI(type, count);
        }

        private void RefreshAllUI()
        {
            foreach (var btn in buildingButtons)
            {
                UpdateBuildingUI(btn.type, BuildModel.Instance.GetCount(btn.type));
            }
        }
    }
}