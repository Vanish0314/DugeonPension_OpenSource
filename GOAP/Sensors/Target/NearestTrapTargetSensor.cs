using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Target;
using UnityEngine;

namespace Dungeon.GOAP.Sensors.Target
{
    public class NearestTrapTargetSensor : LocalTargetSensorBase

    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            if (existingTarget is DungeonTransformTarget target)
            {
                if (target.transform != null)
                    return target;
            }

            var lowLevel = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

            lowLevel.GetNearestTrap(out Transform nearestTrap);
            if (nearestTrap == null)
                return new DungeonTransformTarget(null);
            else
                return new DungeonTransformTarget(nearestTrap);
        }

        public override void Update()
        {

        }
    }
}
