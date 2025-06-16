using System;
using System.Collections.Generic;
using Dungeon.Evnents;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public class MetropolisBuildingManager : MonoBehaviour
    {
        public static MetropolisBuildingManager Instance;
        
        [SerializeField] private List<MetropolisBuildingBase> activeBuildings = new List<MetropolisBuildingBase>();

        [SerializeField]
        private Dictionary<MetropolisBuildingBase, BuildingStatus> savedBuildingStates =
            new Dictionary<MetropolisBuildingBase, BuildingStatus>();

        [SerializeField] private int multiParameter = 1;
        public int procedureTime = 0;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            transform.position = Vector3.zero;
        }
        
        public void Initialize(MetropolisBuildingBase metropolisBuildingBase)
        {
            Subscribe();
        }

        private void Subscribe()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnSwitchedToMetroplisProcedureEvent.EventId,
                OnEnterMetropolisProcedure);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnLeaveMetroplisProcedureEvent.EventId,
                OnLeaveMetropolisProcedure);
        }

        private void OnDestroy()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnSwitchedToMetroplisProcedureEvent.EventId,
                OnEnterMetropolisProcedure);
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnLeaveMetroplisProcedureEvent.EventId,
                OnLeaveMetropolisProcedure);
        }

        public void RegisterBuilding(MetropolisBuildingBase building)
        {
            if (!activeBuildings.Contains(building))
            {
                activeBuildings.Add(building);
            }
        }

        public void UnregisterBuilding(MetropolisBuildingBase building)
        {
            if (activeBuildings.Contains(building))
            {
                activeBuildings.Remove(building);
            }
        }
        
        private void OnEnterMetropolisProcedure(object sender, GameEventArgs e)
        {
            ReactivateBuildings();
        }

        private void OnLeaveMetropolisProcedure(object sender, GameEventArgs e)
        {
            SaveAndDeactivateBuildings();
        }

        private void SaveAndDeactivateBuildings()
        {
            savedBuildingStates.Clear();
            foreach (var building in activeBuildings)
            {
                if (building.CurrentState == BuildingState.UnBuilt)
                {
                    building.ForceCompleteConstructionProcess();
                }
                // 保存建筑状态
                var state = new BuildingStatus()
                {
                    workplaceType = building.workplaceType,
                };

                savedBuildingStates.Add(building, state);
                building.gameObject.SetActive(false);
            }

            activeBuildings.Clear();
        }

        private void ReactivateBuildings()
        {
            foreach (var kvp in savedBuildingStates)
            {
                var building = kvp.Key;
                
                building.gameObject.SetActive(true);
            }
            
            SimulateAssignHeroesAndProcedure(WorkplaceType.Quarry, MetropolisHeroManager.Instance.workersOfQuarry);
            SimulateAssignHeroesAndProcedure(WorkplaceType.LoggingCamp, MetropolisHeroManager.Instance.workersOfLoggingCamp);
            SimulateAssignHeroesAndProcedure(WorkplaceType.Farm, MetropolisHeroManager.Instance.workersOfFarm);
            SimulateAssignHeroesAndProcedure(WorkplaceType.Castle, MetropolisHeroManager.Instance.workersOfCastle);
            SimulateAssignHeroesAndProcedure(WorkplaceType.MonsterLair, MetropolisHeroManager.Instance.workersOfMonsterLair);
            SimulateAssignHeroesAndProcedure(WorkplaceType.TrapFactory, MetropolisHeroManager.Instance.workersOfTrapFactory);
            
            savedBuildingStates.Clear();
        }

        private void SimulateAssignHeroesAndProcedure(WorkplaceType targetType, int availableHeroes)
        {
            // 获取符合条件的建筑快照
            var validBuildings = new List<BuildingCapacity>();
            foreach (var building in savedBuildingStates.Keys)
            {
                if (building != null && building.workplaceType == targetType)
                {
                    validBuildings.Add(new BuildingCapacity(
                        building.maxWorkers,
                        building
                    ));
                }
            }

            // 没有可用建筑时返回
            if (validBuildings.Count == 0) return;

            // 计算总权重（使用当前剩余容量）
            int totalWeight = 0;
            foreach (var bc in validBuildings)
            {
                totalWeight += bc.remaining;
            }

            // 模拟分配流程
            int remainingHeroes = Mathf.Min(availableHeroes, totalWeight);
            System.Random rand = new System.Random();

            // 创建可修改的临时容量列表
            var tempCapacities = new List<BuildingCapacity>(validBuildings);

            while (remainingHeroes > 0 && tempCapacities.Count > 0)
            {
                // 计算当前总权重
                int currentTotal = 0;
                foreach (var bc in tempCapacities)
                {
                    currentTotal += bc.remaining;
                }

                // 随机选择建筑
                int randomValue = rand.Next(currentTotal);
                int cumulative = 0;
                BuildingCapacity selected = null;

                foreach (var bc in tempCapacities)
                {
                    cumulative += bc.remaining;
                    if (randomValue < cumulative)
                    {
                        selected = bc;
                        break;
                    }
                }

                if (selected == null) break;

                // 计算可分配数量（1到剩余容量和英雄的最小值）
                int maxAssign = Mathf.Min(selected.remaining, remainingHeroes);
                int assign = maxAssign > 0 ? rand.Next(1, maxAssign + 1) : 0;

                // 执行模拟分配
                remainingHeroes -= assign;
                selected.remaining -= assign;


                switch (targetType)
                {
                    case WorkplaceType.Quarry:
                        var quarry = (ProduceBuilding)selected.building;
                        quarry.currentStock =
                            Mathf.Min(quarry.currentStock + multiParameter * procedureTime * quarry.baseOutput,
                                quarry.maxStock);
                        break;
                    case WorkplaceType.LoggingCamp:
                        var loggingCamp = (ProduceBuilding)selected.building;
                        loggingCamp.currentStock =
                            Mathf.Min(
                                loggingCamp.currentStock + multiParameter * procedureTime * loggingCamp.baseOutput,
                                loggingCamp.maxStock);
                        break;
                    case WorkplaceType.Farm:
                        var farm = (Farm)selected.building;
                        farm.growthProgress += Mathf.Clamp01(multiParameter * procedureTime);
                        break;
                    case WorkplaceType.Castle:
                        var castle = (Castle)selected.building;
                        castle.currentStock =
                            Mathf.Min(castle.currentStock + multiParameter * procedureTime * castle.baseOutput,
                                castle.maxStock);
                        break;
                    case WorkplaceType.MonsterLair:
                        var monsterLair = (MonsterLair)selected.building;
                        monsterLair.currentStock =
                            Mathf.Min(monsterLair.currentStock + multiParameter * procedureTime * monsterLair.baseOutput,
                                monsterLair.maxStock);
                        break;
                    case WorkplaceType.TrapFactory:
                        var trapFactory = (MonsterLair)selected.building;
                        trapFactory.currentStock =
                            Mathf.Min(trapFactory.currentStock + multiParameter * procedureTime * trapFactory.baseOutput,
                                trapFactory.maxStock);
                        break;
                    case WorkplaceType.Canteen:
                        break;
                    default:
                        break;
                }
                //获取selected具体building实例
                //对building进行修改

                // 用一个新的列表记录待移除项，避免直接删除
                tempCapacities.RemoveAll(b => b.remaining <= 0);
            }
        }
        
        // 辅助类用于临时存储容量
        private class BuildingCapacity
        {
            public int remaining;
            
            public MetropolisBuildingBase building;

            public BuildingCapacity(int capacity , MetropolisBuildingBase targetBuilding)
            {
                remaining = capacity;
                building = targetBuilding;
            }
        }

        [Serializable]
        public struct BuildingStatus
        {
            public WorkplaceType workplaceType;
        }
    }
}
