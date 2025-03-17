using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Targets
{
    public class DungeonExitTarget : ITarget
    {
        public Vector2Int DungeonExitPositionInWorldCoord { get;private set; }

        public Vector3 Position => new (DungeonExitPositionInWorldCoord.x,DungeonExitPositionInWorldCoord.y,0);

        public DungeonExitTarget(Vector2Int exitPosInWorlCoord)
        {
            DungeonExitPositionInWorldCoord = exitPosInWorlCoord;
        }

        public bool IsValid()
        {
            GameFrameworkLog.Warning("DungeonExitTarget.IsValid() is not implemented");
            return true;
        }
    }
}
