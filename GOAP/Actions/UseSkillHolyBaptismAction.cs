using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class UseSkillHolyBaptismAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return true;
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            agent.LowLevelSystem.UseSkill(new SkillDesc("Melee Attack"),data.DirectionToUseSkill);

            return ActionRunState.WaitThenComplete(1.0f);
        }
    }
}
