using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dungeon.AgentLowLevelSystem;
using Dungeon.Common;
using Dungeon.Evnents;
using Dungeon.Overload;
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
            Event.Fire(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEvent.Create(
                AdvanturersGuildSystem.GetCurrentMainHero()
            ));
        }
        [DungeonGridWindow("杀死所有勇者")]
        public static void KillAllHeros()
        {
            foreach (var hero in AdvanturersGuildSystem.currentBehavouringHeroTeam)
            {
                hero.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>().Hp = -1;
            }
        }
        [DungeonGridWindow("对某个勇者使用咒力-说服")]
        public static void UseSpellToConvince()
        {
            var hero = AdvanturersGuildSystem.currentBehavouringHeroTeam.FirstOrDefault();
            if (hero == null)
                return;

            OverloadPower.SpellCurse(hero, CurseType.Convince);
        }
        [DungeonGridWindow("对某个勇者使用咒力-捕获")]
        public static void UseSpellToCapture()
        {
            var hero = AdvanturersGuildSystem.currentBehavouringHeroTeam.FirstOrDefault();
            if (hero == null)
                return;

            OverloadPower.SpellCurse(hero, CurseType.Capture);
        }
    }
}
