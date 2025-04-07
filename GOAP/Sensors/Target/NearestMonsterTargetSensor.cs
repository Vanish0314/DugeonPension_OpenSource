using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Target;
using UnityEngine;

namespace Dungeon.GOAP.Sensors.Target
{
    public class NearestMonsterTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var low = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

            low.GetNearestMonster(out Transform monsterTransform);
            if (monsterTransform == null)
                return null;

            if( UnityEngine.Random.Range(0,2) < 1)
            {
                return new DungeonSkillUsageTarget(monsterTransform.position,new SkillDesc());
            }
            else
            {
                return new DungeonSkillUsageTarget(monsterTransform.position,new SkillDesc());
            }
        }

        public override void Update()
        {

        }
    }
}
