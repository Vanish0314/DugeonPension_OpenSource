using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Character.Hero;
using UnityEngine;

namespace Dungeon
{
    /// <summary>
    /// 包括自己
    /// </summary>
    public class WorldHeroTeamHpBelow70PercentageFriendlyHeroCountKeySensor : GlobalWorldSensorBase
    {
        public override void Created()
        {

        }

        public override SenseValue Sense()
        {
            int count = 0;

            DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentGameProgressingHeroTeam.ForEach(
                hero =>{
                    if(hero.IsAlive() && hero.GetHp() < hero.GetMaxHp() * 0.7f)
                    {
                        count++;
                    }
                }
            );

            return count;
        }
    }
}
