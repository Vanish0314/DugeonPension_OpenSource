using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.AgentLowLevelSystem;
using Dungeon.DungeonEntity;
using Dungeon.DungeonEntity.Torch;
using Dungeon.GOAP.Target;
using UnityEngine;

namespace Dungeon.GOAP.Sensors.Target
{
    public class NearestTorchTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var lowLevel = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

            lowLevel.GetNearestTorch(out Transform nearestTorch);
            if (nearestTorch == null)
                return new DungeonTransformTarget(null);
            else
                return new DungeonTransformTarget(nearestTorch);
        }

        public override void Update()
        {

        }

    }
}
