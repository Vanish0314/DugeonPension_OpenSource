using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class UseSkillHeightenAwarenessAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return true;
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
             agent.LowLevelSystem.UseSkill(new SkillDesc("Heighten Awareness"),data.DirectionToUseSkill);

            return ActionRunState.WaitThenComplete(1.0f);
        }
    }
}
