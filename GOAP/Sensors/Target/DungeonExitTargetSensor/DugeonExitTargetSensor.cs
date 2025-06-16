using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Character;
using Dungeon.DungeonEntity;
using Dungeon.DungeonGameEntry;
using Dungeon.GOAP;
using Dungeon.GridSystem;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class DungeonExitTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }
        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            if (existingTarget != null)
            {
                var pos = existingTarget.Position;
                var low = references.GetCachedComponent<AgentLowLevelSystem>();
                DungeonGameEntry.DungeonGameEntry.GridSystem.GetRoomAt(pos, out var room);
                if (!low.HaveVisitedRoom(room))
                    return existingTarget;
            }

            var exitPos = references.GetCachedComponent<AgentLowLevelSystem>().GetUnvisitedRoomCenterPos();
            return exitPos.HasValue ? new DungeonExitTarget(exitPos.Value) : null;
        }

        public override void Update()
        {
        }
    }
}
