using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon
{
    public class DungeonDontNeedTarget : GlobalTargetSensorBase
    {
        public override void Created()
        {
            
        }

        public override ITarget Sense(ITarget target)
        {
            return null;
        }
    }
}
