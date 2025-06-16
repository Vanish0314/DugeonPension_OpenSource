using System;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class BuildUIController : MonoBehaviour
    {
        [Serializable]
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
            GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnBuildingPlacedEventArgs.EventId,ReduceBuildingCount);
        }

        private void OnDisable()
        {
            if (BuildModel.Instance != null)
            {
                BuildModel.Instance.OnBuildingCountChanged -= UpdateBuildingUI;
                GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnBuildingPlacedEventArgs.EventId,ReduceBuildingCount);
            }
        }

        private void OnBuildingButtonClick(BuildingType type)
        {
            m_PlaceManager.SelectBuildingData(type);
        }

        private void ReduceBuildingCount(object sender, GameEventArgs gameEventArgs)
        {
            OnBuildingPlacedEventArgs eventData = (OnBuildingPlacedEventArgs)gameEventArgs;
            BuildModel.Instance.ModifyCount(eventData.BuildingData.buildingType, -1);
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