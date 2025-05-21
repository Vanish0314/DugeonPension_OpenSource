using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Target;
using UnityEngine;

namespace Dungeon.GOAP.Sensor.Target
{
    public class NearestTorchTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var low = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

            var torch = low.GetNearestTorchInVision();
            var torchTransf = torch?.transform;
            if (torchTransf == null)
                return null;

            return new DungeonTransformTarget(torchTransf);
        }

        public override void Update()
        {

        }

    }
}
