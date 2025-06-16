using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Character;
using Dungeon.GOAP;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class NearestTorchTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var low = references.GetCachedComponent<AgentLowLevelSystem>();

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
