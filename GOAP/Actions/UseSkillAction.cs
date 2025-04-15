using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public abstract class UseSkillAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public sealed override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return IsSkillInRange(agent, distance, data, references);
        }

        public sealed override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            agent.LowLevelSystem.UseSkill(new SkillDesc(mSkillName),data.PositionToUseSkill,data.DirectionToUseSkill);

            return ActionRunState.WaitThenComplete(1.0f);
        }

        protected string mSkillName;
        protected abstract bool IsSkillInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references);
    }
}
