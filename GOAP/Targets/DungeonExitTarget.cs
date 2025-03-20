using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonGameEntry;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Targets
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
            DungeonGameEntry.DungeonGameEntry.WorldBlackboard.TryGetValue<Vector3>(DungeonGameWorldBlackboardEnum.DungeonExitWorldPositionVector3, out var exitPosInWorlCoord);
            // return Vector3.Distance(exitPosInWorlCoord, DungeonExitPositionInWorldCoord) < 0.1f;
            return exitPosInWorlCoord == DungeonExitPositionInWorldCoord;
        }
    }
}
