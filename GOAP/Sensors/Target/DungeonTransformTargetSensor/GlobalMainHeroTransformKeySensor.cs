using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonGameEntry;
using Dungeon.GOAP;
using UnityEngine;

namespace Dungeon
{
    public class GlobalMainHeroTransformKeySensor : GlobalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(ITarget target)
        {
            var hero = DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentMainHero();

            if (hero != null && hero.IsAlive())
            {
                return new DungeonTransformTarget(hero.transform);
            }
            else return null;
        }
    }
}
