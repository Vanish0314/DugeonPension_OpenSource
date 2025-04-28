using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Common.MonoPool;
using GameFramework;
using GameFramework.Event;
using GameFramework.Fsm;
using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;

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
        UnBuilt,
        Completed,
    }
    
    public class MetropolisBuildingBase : MonoPoolItem
    {
        private IFsm<MetropolisBuildingBase> m_BuildingFsm;
        
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

        private Coroutine m_CurrentCoroutine;

        #region FSM
        // 初始化状态机
        private void InitFsm()
        {
            FsmComponent fsmComponent = GameEntry.Fsm.GetComponent<FsmComponent>();
    
            // 生成唯一名称：类型名+实例ID
            string fsmName = $"{GetType().Name}_FSM_{this.GetInstanceID()}";

            if (fsmComponent.HasFsm<MetropolisBuildingBase>(fsmName))
            {
                m_BuildingFsm = fsmComponent.GetFsm<MetropolisBuildingBase>(fsmName);
            }
            else
            {
                m_BuildingFsm = fsmComponent.CreateFsm(
                    fsmName, // 唯一名称
                    this,
                    new UnBuiltState(),
                    new CompletedState()
                );
            }
            
            m_BuildingFsm.Start<UnBuiltState>();
        }
        
        // 添加状态查询接口
        public BuildingState CurrentState
        {
            get
            {
                if (m_BuildingFsm == null || m_BuildingFsm.IsDestroyed)
                    return BuildingState.None;

                var stateType = m_BuildingFsm.CurrentState.GetType();
                
                if (stateType == typeof(UnBuiltState))
                    return BuildingState.UnBuilt;
                if (stateType == typeof(CompletedState))
                    return BuildingState.Completed;

                return BuildingState.None;
            }
        }

        #endregion
        
        #region WorkerLogic
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
                
                if (workingHeroes.Count == 0 && m_CurrentCoroutine != null)
                {
                    StopCoroutine(m_CurrentCoroutine);
                    m_CurrentCoroutine = null;
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
        
        #endregion

        public void StopCurrentCoroutine()
        {
            if (m_CurrentCoroutine != null)
                StopCoroutine(m_CurrentCoroutine);
            m_CurrentCoroutine = null;
        }
        
        #region UnBuiltState

        public void StartConstructionProcess()
        {
            if (m_CurrentCoroutine != null)
                return;
            m_CurrentCoroutine = StartCoroutine(ConstructionProcess());
        }
        
        private IEnumerator ConstructionProcess()
        {
            while (constructionProgress < 1f)
            {
                // 根据在场工人数量计算实际施工速度
                float effectiveSpeed = constructionSpeed * workingHeroes.Count;
                constructionProgress = Mathf.Clamp01(constructionProgress + effectiveSpeed * Time.deltaTime / constructionDuration);
                
                yield return null;
            }
            
            constructionProgress = 1f;
            GameEntry.Event.Fire(this, OnConstructionCompletedEvent.Create());
        }
        
        #endregion
        
        #region CompletedState

        public void StartProductionProcess()
        {
            if (m_CurrentCoroutine != null)
                return;
            m_CurrentCoroutine = StartCoroutine(ProductionProcess());
        }
        
        // 生产周期协程
        private IEnumerator ProductionProcess()
        {
            while (workingHeroes.Count > 0)
            {
                yield return new WaitForSeconds(productionInterval);
                ProduceResource();
            }
            m_CurrentCoroutine = null;
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
        
        #endregion
        
        #region Override
        public override void OnSpawn(object data) 
        {
            InitFsm();
            constructionProgress = 0;
            currentStock = 0;
            currentEfficiency = 0f;
            m_CurrentCoroutine = null;
        }

        public override void Reset()
        {
            // 清退所有工人
            foreach (var hero in workingHeroes.ToArray()) // ToArray避免修改集合
            {
                WorkerLeave(hero);
            }
            
            workingHeroes.Clear();
            currentStock = 0;
            currentEfficiency = 0f;
            m_CurrentCoroutine = null;
        }
        #endregion
        
    }
    
    public sealed class OnConstructionCompletedEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnConstructionCompletedEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnConstructionCompletedEvent Create()
        {
            OnConstructionCompletedEvent onConstructionCompletedEvent = ReferencePool.Acquire<OnConstructionCompletedEvent>();
            return onConstructionCompletedEvent;
        }

        public override void Clear()
        {
        }
    }
}
