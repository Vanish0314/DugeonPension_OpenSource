using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using DG.Tweening;
using Dungeon.Character;
using Dungeon.GOAP;
using Dungeon.Target;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class NearestMonsterTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var low = references.GetCachedComponent<AgentLowLevelSystem>();

            var monster = low.GetNearestTorchInVision();
            var monsterTransform = monster?.transform;
            if (monsterTransform == null)
                return null;

            return new DungeonSkillUsageTarget(monsterTransform.position, monsterTransform.position - agent.Transform.position);
        }

        public override void Update()
        {

        }
    }
}
