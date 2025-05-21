using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Target;
using UnityEngine;

namespace Dungeon.GOAP.Sensor.Target
{
    public class NearestTrapTargetSensor : LocalTargetSensorBase

    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var low = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

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
