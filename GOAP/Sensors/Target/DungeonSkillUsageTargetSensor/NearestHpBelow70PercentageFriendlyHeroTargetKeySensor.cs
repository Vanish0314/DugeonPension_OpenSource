using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Character;
using Dungeon.Target;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class NearestHpBelow70PercentageFriendlyHeroTargetKeySensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var me = references.GetCachedComponent<HeroEntityBase>();

            HeroEntityBase result = null;
            DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentGameProgressingHeroTeam().ForEach(
                hero =>{
                    if(hero == me)
                    {
                        return;
                    }

                    if(hero.IsAlive() && hero.GetHp() < hero.GetMaxHp() * 0.7f)
                    {
                        if(result == null || Vector3.Distance(me.transform.position, hero.transform.position) < Vector3.Distance(me.transform.position, result.transform.position))
                        {
                            result = hero;
                        }
                    }
                }
            );

            return result == null? null : new DungeonSkillUsageTarget(result.transform.position, result.transform.position - me.transform.position);
        }

        public override void Update()
        {

        }
    }
}
