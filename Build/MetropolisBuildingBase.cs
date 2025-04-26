using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Common.MonoPool;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dungeon
{
    // 工作建筑类型枚举
    public enum WorkplaceType
    {
        Farm,       // 农场
        Mine,       // 矿场
        Factory,    // 工厂
        Laboratory  // 实验室
    }
    
    // 建筑当前状态
    public enum BuildingState
    {
        None,
        Area,
        Construction,
        Completed,
    }
    
    public class MetropolisBuildingBase : MonoPoolItem
    {
        [Header("当前状态")] 
        private BuildingState currentState;
        public BuildingState CurrentState
        {
            get => currentState;
            set
            {
                if (currentState != value)
                {
                    ExitState(currentState);
                    currentState = value;
                    EnterState(value);
                }
            }
        }
        
        [Header("外观设置")]
        public Sprite constructionSprite;
        public Sprite completedSprite;

        [Header("建造设置")] 
        public float constructionProgress;
        [SerializeField] private float constructionDuration;
        [SerializeField] private float constructionSpeed;
        
        
        [Header("工作设置")]
        public WorkplaceType workplaceType;
        public ResourceType resourceType;
        public int baseOutput = 1;
        public int maxWorkers = 3;
        public float productionInterval = 5f; // 生产间隔(秒)
        
        [Header("工作状态")]
        public List<MetropolisHeroBase> workingHeroes = new List<MetropolisHeroBase>();
        public int currentStock = 0;
        public int maxStock = 100;
        public float currentEfficiency; // 当前效率系数

        private Coroutine productionCoroutine;

        private void Update()
        {
            if (workingHeroes.Count > 0 && productionCoroutine == null)
            {
                productionCoroutine = StartCoroutine(ProductionCycle());
            }
            else if (workingHeroes.Count == 0 && productionCoroutine != null)
            {
                StopCoroutine(productionCoroutine);
                productionCoroutine = null;
            }
        }

        // 生产周期协程
        private IEnumerator ProductionCycle()
        {
            while (workingHeroes.Count > 0)
            {
                yield return new WaitForSeconds(productionInterval);
                ProduceResource();
            }
            productionCoroutine = null;
        }

        // 尝试让勇者进驻
        public bool TryAssignWorker(MetropolisHeroBase hero)
        {
            if (!CanAcceptWorker() || workingHeroes.Contains(hero))
                return false;

            WorkerEnter(hero);
            return true;
        }
        
        // 新增强制进驻方法
        public bool ForceAssignWorker(MetropolisHeroBase hero)
        {
            // 如果已经是当前工人，直接返回成功
            if (workingHeroes.Contains(hero))
                return true;

            // 如果没有空位，踢出效率最低的工人
            if (!CanAcceptWorker())
            {
                MetropolisHeroBase lowestEfficiencyWorker = FindLowestEfficiencyWorker();
                if (lowestEfficiencyWorker != null)
                {
                    WorkerLeave(lowestEfficiencyWorker);
                }
            }

            // 尝试正常进驻
            return TryAssignWorker(hero);
        }

        // 找到效率最低的工人
        private MetropolisHeroBase FindLowestEfficiencyWorker()
        {
            if (workingHeroes.Count == 0)
                return null;

            MetropolisHeroBase lowest = workingHeroes[0];
            foreach (var worker in workingHeroes)
            {
                if (worker.Efficiency < lowest.Efficiency)
                {
                    lowest = worker;
                }
            }
            return lowest;
        }

        public bool CanAcceptWorker()
        {
            return workingHeroes.Count < maxWorkers;
        }
        
        private void WorkerEnter(MetropolisHeroBase hero)
        {
            workingHeroes.Add(hero);
            UpdateEfficiency();
        }

        public void WorkerLeave(MetropolisHeroBase hero)
        {
            if (workingHeroes.Remove(hero))
            {
                hero.EndWorking();
                UpdateEfficiency();
                
                if (workingHeroes.Count == 0 && productionCoroutine != null)
                {
                    StopCoroutine(productionCoroutine);
                    productionCoroutine = null;
                }
            }
        }

        // 计算当前效率
        protected virtual void UpdateEfficiency()
        {
            currentEfficiency = 0f;
            foreach (var hero in workingHeroes)
            {
                currentEfficiency += hero.Efficiency; // 累加工人效率
            }
            
            Debug.Log($"{gameObject.name} 当前效率: {currentEfficiency:P0}");
        }

        // 生产资源
        public virtual void ProduceResource()
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
            currentStock = 0;
        }

        #region ChangeState

        private void EnterState(BuildingState newState)
        {
            switch (newState)
            {
                case BuildingState.Area:
                    SetUpAreaState();
                    break;
                case BuildingState.Construction:
                    //StartConstruction();
                    break;
                case BuildingState.Completed:
                    //CompleteBuilding();
                    break;
            }
        }

        private void ExitState(BuildingState oldState)
        {
            switch (oldState)
            {
                case BuildingState.Construction:
                    StopAllCoroutines();
                    break;
            }
        }

        private void SetupPreviewState()
        {
            // 禁用碰撞和功能
            GetComponent<Collider2D>().enabled = false;
        }

        private void SetUpAreaState()
        {
            GetComponent<Collider2D>().enabled = true;
        }

        private void StartConstruction()
        {
            // 启动施工协程
            StartCoroutine(ConstructionProcess());
        }

        private IEnumerator ConstructionProcess()
        {
            constructionProgress = 0;
            while (constructionProgress < 1f)
            {
                // 根据工人数量加速
                constructionSpeed = workingHeroes.Count > 0 ? currentEfficiency : 0;
                constructionProgress += Time.deltaTime / constructionDuration * constructionSpeed;
                //UpdateConstructionProgress(constructionProgress);
                yield return null;
            }
    
            CurrentState = BuildingState.Completed;
        }

        private void CompleteBuilding()
        {

        }
        
        #endregion
        
        #region Override
        public override void OnSpawn(object data) 
        {
            currentState = BuildingState.Area;
        }

        public override void Reset()
        {
            currentState = BuildingState.None;
            
            // 清退所有工人
            foreach (var hero in workingHeroes.ToArray()) // ToArray避免修改集合
            {
                WorkerLeave(hero);
            }
            
            workingHeroes.Clear();
            currentStock = 0;
            currentEfficiency = 0f;
        }
        #endregion
        
    }
}
