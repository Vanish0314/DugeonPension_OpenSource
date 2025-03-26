using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon.GOAP.Sensors.Key
{
    public class HeroCoinCountSensor : LocalWorldSensorBase
    {
        public override void Created()
        {

        }

        public override void Update()
        {

        }
        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            return 0;   
        }
    }
}
