using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Character;
using Dungeon.GOAP;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class NearestChestTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var low = references.GetCachedComponent<AgentLowLevelSystem>();

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
