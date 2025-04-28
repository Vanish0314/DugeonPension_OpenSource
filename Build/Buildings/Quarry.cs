using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dungeon
{
    public class Quarry : MetropolisBuildingBase
    {
        [SerializeField] private InputReader m_inputReader;
        
        [Header("UI Settings")] [SerializeField]
        private GameObject resourceUIPrefab; // UI预制体

        [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0); // UI显示偏移

        protected override void OnEnable()
        {
            base.OnEnable();
            m_inputReader.OnBuildingClickedEvent += OnBuildingClicked;
            m_inputReader.OnNoBuildingClickedEvent += HideResourceUI;
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            m_inputReader.OnBuildingClickedEvent -= OnBuildingClicked;
            m_inputReader.OnNoBuildingClickedEvent -= HideResourceUI;
        }
        
        private void OnBuildingClicked(GameObject clickedBuilding)
        {
            // if(EventSystem.current.IsPointerOverGameObject())
            //     return;
            
            if (clickedBuilding != this.gameObject)
            {
                HideResourceUI();
                return;
            }
            ShowResourceUI();
        }

        // 显示资源UI
        private void ShowResourceUI()
        {
            // 获取UI组件
            ResourceUI uiComponent = resourceUIPrefab.GetComponent<ResourceUI>();
            if (uiComponent != null)
            {
                uiComponent.ShowAllUI();
            }
        }

        private void HideResourceUI()
        {
            ResourceUI uiComponent = resourceUIPrefab.GetComponent<ResourceUI>();
            if (uiComponent != null)
            {
                uiComponent.HideAllUI();
            }
        }

        #region CompleteState

        public override void StartCompletedBehavior()
        {
            
        }
        
        

        #endregion
        
        
    }
}
