using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Character;
using Dungeon.Target;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class NearestTeammateTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var low = references.GetCachedComponent<AgentLowLevelSystem>();

            var teammate = low.GetNearestTeammate();
            
            return teammate == null? null : new DungeonSkillUsageTarget(teammate.transform.position, teammate.transform.position - agent.Transform.position);
        }

        public override void Update()
        {

        }
    }
}
