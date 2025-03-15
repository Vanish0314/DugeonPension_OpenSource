using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Targets;
using UnityEngine;

namespace Dungeon.GOAP.Sensors.Targets
{
    public class DugeonExitTargetSensor : GlobalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(ITarget target)
        {
            return new DungeonExitTarget(new Vector2Int(10,10));
        }

    }
}
