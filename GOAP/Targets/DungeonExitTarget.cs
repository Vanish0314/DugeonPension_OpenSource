using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonGameEntry;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class DungeonExitTarget : ITarget
    {
        public Vector3 DungeonExitPositionInWorldCoord { get;private set; }

        public Vector3 Position => new (DungeonExitPositionInWorldCoord.x,DungeonExitPositionInWorldCoord.y,0);

        public DungeonExitTarget(Vector3 exitPosInWorlCoord)
        {
            DungeonExitPositionInWorldCoord = exitPosInWorlCoord;
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
