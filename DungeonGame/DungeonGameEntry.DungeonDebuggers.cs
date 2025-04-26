using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dungeon.AgentLowLevelSystem;
using Dungeon.Common;
using Dungeon.Evnents;
using UnityEngine;
using static UnityGameFramework.Runtime.DebuggerComponent;

namespace Dungeon.DungeonGameEntry
{
    public partial class DungeonGameEntry : MonoBehaviour
    {
        private static void InitDungeonDebuggers()
        {

        }

        [DungeonGridWindow("切换到地牢")]
        public static void SwitchToDungeon()
        {
            Event.Fire(OnPlayerSwitchToDungeonEvent.EventId, OnPlayerSwitchToDungeonEvent.Create());
        }
        [DungeonGridWindow("切换到工厂")]
        public static void SwitchToMetroplis()
        {
            Event.Fire(OnPlayerSwitchToMetroplisEvent.EventId, OnPlayerSwitchToMetroplisEvent.Create());
        }
        [DungeonGridWindow("勇者现在到达地牢")]
        public static void HeroArriveNow()
        {
            Event.Fire(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEvent.Create());
        }
        [DungeonGridWindow("生成勇者")]
        public static void SpawnHero()
        {
            AdvanturersGuildSystem.SpawnHero(Vector3.zero);
        }
        [DungeonGridWindow("杀死所有勇者")]
        public static void KillAllHeros()
        {
            foreach (var hero in AdvanturersGuildSystem.currentBehavouringHeroTeam)
            {
                hero.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>().Hp = -1;
            }
        }
        [DungeonGridWindow("进入战斗流程")]
        public static void EnterCombatProcedure()
        {
            Task.Run(async () =>
            {
                Event.Fire(OnOpenningLogoEndEvent.EventId, OnOpenningLogoEndEvent.Create());
                await Task.Delay(TimeSpan.FromSeconds(0.5));

                Event.Fire(OnContinueGameButtonClickEvent.EventId, OnContinueGameButtonClickEvent.Create());
                await Task.Delay(TimeSpan.FromSeconds(0.5));

                Event.Fire(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEvent.Create());
                await Task.Delay(TimeSpan.FromSeconds(0.5));

                Event.Fire(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEvent.Create());
            });

        }
    }
}
