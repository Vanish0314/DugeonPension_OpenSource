using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Character;
using Dungeon.GOAP;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class NearestTrapTargetSensor : LocalTargetSensorBase

    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var low = references.GetCachedComponent<AgentLowLevelSystem>();

            var trap = low.GetNearestTrapInVision();
            var trapTransf = trap?.transform;
            if (trapTransf == null)
                return null;

            return new DungeonTransformTarget(trapTransf);
        }

        public override void Update()
        {

        }
    }
}
