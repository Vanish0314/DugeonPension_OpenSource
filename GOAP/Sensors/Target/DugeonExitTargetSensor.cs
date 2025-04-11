using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.Trap;
using Dungeon.DungeonGameEntry;
using Dungeon.GOAP.Targets;
using Dungeon.GridSystem;
using UnityEngine;

namespace Dungeon.GOAP.Sensor.Target
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
                var low = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>();
                DungeonGameEntry.DungeonGameEntry.GridSystem.GetRoomAt(pos, out var room);
                if (!low.HaveVisitedRoom(room))
                    return existingTarget;
            }

            return new DungeonExitTarget(
                references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().GetUnvisitedRoomCenterPos()
                );
        }

        public override void Update()
        {
        }
    }
}
