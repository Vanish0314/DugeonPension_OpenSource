using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Target;
using UnityEngine;

namespace Dungeon.GOAP.Sensors.Target
{
    public class NearestChestTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var lowLevel = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

            lowLevel.GetNearestTreasureChest(out Transform nearestChest);
            if (nearestChest == null)
                return new DungeonTransformTarget(null);
            else
                return new DungeonTransformTarget(nearestChest);
        }

        public override void Update()
        {

        }
    }
}
