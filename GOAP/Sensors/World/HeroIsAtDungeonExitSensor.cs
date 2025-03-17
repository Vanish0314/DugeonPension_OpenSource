using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using GameFramework;
using UnityEngine;

namespace Dungeon
{
    public class HeroIsAtDungeonExitSensor : GlobalWorldSensorBase
    {
        public Vector2Int exitPos = new Vector2Int(0, 0);

        public override void Created()
        {
            GameFrameworkLog.Warning("HeroIsAtDungeonExitSensor Created,but not implemented");
            exitPos = new Vector2Int(0, 0);
        }

        public override SenseValue Sense()
        {
            return 0;
        }
    }
}
