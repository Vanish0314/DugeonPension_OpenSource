using System.Collections;
using System.Collections.Generic;
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
            foreach (var hero in AdvanturersGuildSystem.currentHeroTeam)
            {
                hero.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>().Hp = -1;
            }
        }
    }
}
