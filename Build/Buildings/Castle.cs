using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace Dungeon
{
    public class Castle : MetropolisBuildingBase
    {
        [Header("输入设置")] 
        [SerializeField] private InputReader m_inputReader;

        [Header("生产设置")] 
        public int baseOutput = 1;
        public float productionInterval = 5f; // 生产间隔(秒)
        public int maxStock = 100;
        public int currentStock = 0;
        
        [Header("UI设置")] 
        [SerializeField] private GameObject castleUIPrefab; // UI预制体
        [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0); // UI显示偏移

        #region Override

        protected override void OnEnable()
        {
            if (currentStock >= maxStock)
            {
                currentStock = maxStock;
                EndProductionProcess();
            }
            else
            {
                hasWork = true;
            }
            base.OnEnable();
            m_inputReader.OnBuildingClickedEvent += OnBuildingClicked;
            m_inputReader.OnNoBuildingClickedEvent += HideCastleUI;
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            EndProductionProcess();
            m_inputReader.OnBuildingClickedEvent -= OnBuildingClicked;
            m_inputReader.OnNoBuildingClickedEvent -= HideCastleUI;
        }
        
        #endregion

        #region UI

        private void OnBuildingClicked(GameObject clickedBuilding)
        {
            if (CurrentState == BuildingState.Completed)
            {
                if (clickedBuilding != gameObject)
                {
                    HideCastleUI();
                    return;
                }
                ShowCastleUI();
            }
        }

        // 显示资源UI
        private void ShowCastleUI()
        {
            // 获取UI组件
            CastleUI uiComponent = castleUIPrefab.GetComponent<CastleUI>();
            if (uiComponent != null)
            {
                uiComponent.transform.position = transform.position + uiOffset;
                uiComponent.ShowAllUI();
            }
        }

        private void HideCastleUI()
        {
            CastleUI uiComponent = castleUIPrefab.GetComponent<CastleUI>();
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
                ProduceMagicPower();
            }
            EndProductionProcess();
        }

        private void EndProductionProcess()
        {
            if(currentStock >= maxStock)
                hasWork = false;
            
            FireAllWorkers();
            m_CurrentCoroutine = null;
            StopCurrentCoroutine();
        }
        
        // 生产资源
        private void ProduceMagicPower()
        {
            if (currentStock >= maxStock) 
            {
                Debug.Log($"{gameObject.name} 库存已满");
                return;
            }
            
            int productionAmount = Mathf.RoundToInt(baseOutput * currentEfficiency);
            currentStock = Mathf.Min(currentStock + productionAmount, maxStock);
        }

        public void GatherMagicPower()
        {
            TechnologyTreeManager.Instance.ModifyMagicPower(currentStock);
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
