using System;
using System.Data.Common;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.AgentLowLevelSystem;
using Dungeon.GOAP.Action;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class HealMagicAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return true;
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            agent.LowLevelSystem.UseSkill(new SkillDesc("Heal"),data.PositionToUseSkill);

            return ActionRunState.WaitThenComplete(1.0f);
        }
    }
}
