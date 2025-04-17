using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Target;
using UnityEngine;

namespace Dungeon.GOAP.Sensor.Target
{
    public class NearestChestTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var low = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

            var chest = low.GetNearestTreasureChest();
            var chestTransf = chest?.transform;
            if (chestTransf == null)
                return null;

            return new DungeonTransformTarget(chestTransf);
        }

        public override void Update()
        {

        }
    }
}
