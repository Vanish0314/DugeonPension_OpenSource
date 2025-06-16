using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class MonsterLair : MetropolisBuildingBase
    {
        [Header("输入设置")] 
        [SerializeField] private InputReader m_inputReader;

        [Header("当前状态")]
        public int maxStock;
        public int currentStock;
        public int baseOutput;
        [SerializeField] private float productionInterval;
        [SerializeField] private float productionTimer;
        [SerializeField] private float defaultCountdown; // 照看冷却
        [SerializeField] private float careCountdownTimer;  // 照看倒计时
        [SerializeField] private float careTime;       // 照看需要的时间
        [SerializeField] private bool isCareNeeded;    // 需要照看
        
        [Header("UI设置")] 
        [SerializeField] private MonsterLairUI monsterLairUI; // UI预制体
        [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0); // UI显示偏移

        private bool isCaring; // 是否在照看
        
        private bool isProducing;        // 是否在生长中
        
        #region Override

        protected override void OnEnable()
        {
            base.OnEnable();
            if (currentStock >= maxStock)
            {
                currentStock = maxStock;
                EndProduce();
            }
            m_inputReader.OnBuildingClickedEvent += OnBuildingClicked;
            m_inputReader.OnNoBuildingClickedEvent += HideUI;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            m_inputReader.OnBuildingClickedEvent -= OnBuildingClicked;
            m_inputReader.OnNoBuildingClickedEvent -= HideUI;
        }
        
        #endregion
        
        #region UI

        private void OnBuildingClicked(GameObject clickedBuilding)
        {
            if (CurrentState == BuildingState.Completed)
            {
                if (clickedBuilding != gameObject)
                {
                    HideUI();
                    return;
                }
                
                ShowUI();
            }
        }

        // 显示资源UI
        private void ShowUI()
        {
            if (monsterLairUI != null && !isCareNeeded)
            {
                monsterLairUI.transform.position = transform.position + uiOffset;
                monsterLairUI.ShowMonsterLairUI();
            }
        }

        private void HideUI()
        {
            if (monsterLairUI != null && !isCareNeeded)
            {
                monsterLairUI.CloseAllUI();
            }
        }

        #endregion
        
        #region 生产
        
        private void StartProducing()
        {
            if (isProducing)
                return;
            isProducing = true;
            StartCoroutine(ProduceRoutine());
        }

        private IEnumerator ProduceRoutine()
        {
            while (currentStock < maxStock)
            {
                if (!isCareNeeded)
                {
                    // 更新浇水倒计时
                    careCountdownTimer -= Time.deltaTime;
                    if (careCountdownTimer <= 0)
                    {
                        isCareNeeded = true;
                        hasWork = true;
                        monsterLairUI.ShowWarningUI();
                    }
                    
                    // 更新生产计时器
                    productionTimer += Time.deltaTime;
                    if (productionTimer >= productionInterval)
                    {
                        productionTimer = 0f;
                        currentStock += baseOutput;
                    }
                }
                yield return null;
            }

            EndProduce();
        }

        private void EndProduce()
        {
            isProducing = false;
        }
        #endregion

        #region 照看

        private bool CareProduce()
        {
            if (!isCareNeeded || isCaring) return false;

            StartCoroutine(StartCare());
            return true;
        }

        private IEnumerator StartCare()
        {
            isCaring = true;
            
            yield return new WaitForSeconds(careTime);
            isCareNeeded = false;

            EndCare();
        }

        private void EndCare()
        {
            hasWork = false;
            
            careCountdownTimer = defaultCountdown;
            isCaring = false;
            FireAllWorkers();
            monsterLairUI.CloseCurrentUI();
        }
        
        #endregion

        #region 收获
        
        public void GatherResource()
        {
            // 生成资源icon，先扩散后飞入右上角资源图标处，然后消失
            Sprite sprite = ResourceModel.Instance.GetResourceSprite(nameof(ResourceType.MagicPower));
            Vector2 targetTransform = ResourceModel.Instance.GetResourceTransform(nameof(ResourceType.MagicPower));
            GatherEffectHelper.Instance.OnGatherEffect(sprite, currentStock,transform.position,targetTransform);
            
            ResourceModel.Instance.GatherResource(ResourceType.MagicPower,currentStock);
    
            // 重置状态
            currentStock = 0;
            isCareNeeded = false;
            isProducing = false;
            monsterLairUI.CloseAllUI();
            StopAllCoroutines();
        }
        
        #endregion

        #region 建筑状态
        public override void StartCompletedBehavior()
        {
            base.StartCompletedBehavior();
            careCountdownTimer = defaultCountdown;
        }

        public override void UpdateCompletedBehavior()
        {
            base.UpdateCompletedBehavior();

            if (currentStock < maxStock)
            {
                StartProducing();
            }
            
            if (workingHeroes.Count > 0)
            {
                CareProduce();
            }
        }

        public override void Reset()
        {
            base.Reset();
            StopAllCoroutines();
            currentStock = 0;
            isCareNeeded = false;
        }
        #endregion
    }
}
