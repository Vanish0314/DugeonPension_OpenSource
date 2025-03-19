using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Targets;
using Dungeon.GridSystem;
using UnityEngine;

namespace Dungeon.GOAP.Sensors.Target
{
    public class DungeonExitTargetSensor : GlobalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(ITarget target)
        {
            return new DungeonExitTarget(GridSystem.GridSystem.Instance.GetDungeonExitWorldPosition());
        }

    }
}
