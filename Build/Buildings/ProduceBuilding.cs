using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dungeon
{
    public class ProduceBuilding : MetropolisBuildingBase
    {
        [Header("输入设置")] 
        [SerializeField] private InputReader m_inputReader;

        [Header("生产设置")] 
        [SerializeField] private ResourceType procedureResourceType;
        [SerializeField] private int baseOutput = 1;
        public float productionInterval = 5f; // 生产间隔(秒)
        public int maxStock = 100;
        public int currentStock = 0;
        
        [Header("UI设置")] 
        [SerializeField] private GameObject resourceUIPrefab; // UI预制体
        [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0); // UI显示偏移

        #region Override

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
        
        #endregion

        #region UI

        private void OnBuildingClicked(GameObject clickedBuilding)
        {
            if (CurrentState == BuildingState.Completed)
            {
                if (clickedBuilding != gameObject)
                {
                    HideResourceUI();
                    return;
                }
                ShowResourceUI();
            }
        }

        // 显示资源UI
        private void ShowResourceUI()
        {
            // 获取UI组件
            ResourceUI uiComponent = resourceUIPrefab.GetComponent<ResourceUI>();
            if (uiComponent != null)
            {
                uiComponent.transform.position = transform.position + uiOffset;
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

        #endregion
        
        #region Production
        
        public void StartProductionProcess()
        {
            if (m_CurrentCoroutine != null)
                return;
            m_CurrentCoroutine = StartCoroutine(ProductionProcess());
        }
        
        // 生产周期协程
        private IEnumerator ProductionProcess()
        {
            while (workingHeroes.Count > 0 && currentStock < maxStock)
            {
                yield return new WaitForSeconds(productionInterval);
                ProduceResource();
            }
            EndProductionProcess();
        }

        private void EndProductionProcess()
        {
            FireAllWorkers();
            hasWork = false;
            m_CurrentCoroutine = null;
            StopCurrentCoroutine();
        }
        
        // 生产资源
        protected virtual void ProduceResource()
        {
            if (currentStock >= maxStock) 
            {
                Debug.Log($"{gameObject.name} 库存已满");
                return;
            }
            
            int productionAmount = Mathf.RoundToInt(baseOutput * currentEfficiency);
            currentStock = Mathf.Min(currentStock + productionAmount, maxStock);
        }

        public void GatherResources()
        {
            // 为资源管理器增加对应的资源
            ResourceModel.Instance.GatherResource(procedureResourceType, currentStock);
            currentStock = 0;
            hasWork = true;
        }
        
        #endregion

        #region CompleteState

        public override void StartCompletedBehavior()
        {
            base.StartCompletedBehavior();
            GameFrameworkLog.Info("进入完成状态");
            hasWork = true;
        }

        public override void UpdateCompletedBehavior()
        {
            if (workingHeroes.Count > 0)
            {
                StartProductionProcess();
            }
        }

        #endregion
    }
}
