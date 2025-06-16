using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon.Character;
using Dungeon.Evnents;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dungeon
{
    public class MetropolisHeroManager : MonoBehaviour
    {
        public static MetropolisHeroManager Instance;

        [SerializeField] private List<MetropolisHeroBase> activeHeroes = new List<MetropolisHeroBase>();

        [SerializeField]
        private Dictionary<MetropolisHeroBase, HeroState> savedHeroStates =
            new Dictionary<MetropolisHeroBase, HeroState>();

        [SerializeField] private HeroState defaultHeroState;
        
        public int workersOfQuarry = 0;
        public int workersOfLoggingCamp = 0;
        public int workersOfFarm = 0;
        public int workersOfCastle = 0;
        public int workersOfMonsterLair = 0;
        public int workersOfTrapFactory = 0;
        
        [SerializeField] private int multiParameter = 1;
        
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

        public void Initialize(MetropolisHeroBase metropolisHeroBase)
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

        public void RegisterHero(MetropolisHeroBase hero)
        {
            if (!activeHeroes.Contains(hero))
            {
                activeHeroes.Add(hero);
            }
        }

        public void UnregisterHero(MetropolisHeroBase hero)
        {
            if (activeHeroes.Contains(hero))
            {
                activeHeroes.Remove(hero);
            }
        }

        public void TransferHero(HeroEntityBase from)
        {
            var hero = MetropolisHeroTransverter.Instance.TransverseToMetropolisHero(from);

            if (hero != null && !savedHeroStates.ContainsKey(hero))
            {
                if (!savedHeroStates.TryAdd(hero, defaultHeroState))
                    GameFrameworkLog.Error("[MetropolisHeroManager] ： 没转换成功");
            }
        }
        
        private void OnEnterMetropolisProcedure(object sender, GameEventArgs e)
        {
            ReactivateHeroes();
        }

        private void OnLeaveMetropolisProcedure(object sender, GameEventArgs e)
        {
            SaveAndDeactivateHeroes();
        }

        private void SaveAndDeactivateHeroes()
        {
            savedHeroStates.Clear();
            foreach (var hero in activeHeroes)
            {
                // 待定方法
                if (hero.m_CurrentWorkType == WorkplaceType.Construction)
                {
                    hero.m_CurrentWorkType = WorkplaceType.Quarry;
                }
                // 保存英雄状态
                var state = new HeroState
                {
                    hungerLevel = hero.HungerLevel,
                };

                savedHeroStates.Add(hero, state);
                hero.gameObject.SetActive(false);
            }

            workersOfQuarry = GetHeroCountByWorkplace(WorkplaceType.Quarry);
            workersOfLoggingCamp = GetHeroCountByWorkplace(WorkplaceType.LoggingCamp);
            workersOfFarm = GetHeroCountByWorkplace(WorkplaceType.Farm);
            workersOfCastle = GetHeroCountByWorkplace(WorkplaceType.Castle);
            workersOfMonsterLair = GetHeroCountByWorkplace(WorkplaceType.MonsterLair);
            workersOfTrapFactory = GetHeroCountByWorkplace(WorkplaceType.TrapFactory);

            activeHeroes.Clear();
        }

        private void ReactivateHeroes()
        {
            var procedureTime = MetropolisBuildingManager.Instance.procedureTime;
            foreach (var kvp in savedHeroStates)
            {
                var hero = kvp.Key;
                var state = kvp.Value;
                
                // 计算回来时的英雄状态
                hero.HungerLevel = (int)Mathf.Min(
                    state.hungerLevel + procedureTime * multiParameter * Random.Range(0.5f, 1f),
                    hero.MaxHungerLevel);

                hero.gameObject.SetActive(true);
            }

            savedHeroStates.Clear();
        }
        
        private int GetHeroCountByWorkplace(WorkplaceType targetType)
        {
            int count = 0;
            foreach (var hero in activeHeroes)
            {
                // 需要确保MetropolisHeroBase有CurrentWorkType属性
                if (hero.m_CurrentWorkType == targetType) // 确保处于工作状态
                {
                    count++;
                }
            }
            return count;
        }
        
        [Serializable]
        public struct HeroState
        {
            public int hungerLevel;
        }
    }
}
