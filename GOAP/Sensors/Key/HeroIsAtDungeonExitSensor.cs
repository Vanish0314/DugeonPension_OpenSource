using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Sensors.Key
{
    public class HeroIsAtDungeonExitSensor : GlobalWorldSensorBase
    {
        public override void Created()
        {

        }

        public override SenseValue Sense()
        {
            return 0;
        }
    }
}
