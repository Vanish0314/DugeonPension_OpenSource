using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using DG.Tweening;
using Dungeon.GOAP.Target;
using Dungeon.Target;
using UnityEngine;

namespace Dungeon.GOAP.Sensor.Target
{
    public class NearestMonsterTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var low = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

            var monster = low.GetNearestMonsterInVision();
            var monsterTransform = monster?.transform;
            if (monsterTransform == null)
                return null;

            return new DungeonSkillUsageTarget(monsterTransform.position);
        }

        public override void Update()
        {

        }
    }
}
