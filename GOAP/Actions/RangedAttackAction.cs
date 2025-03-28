using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Action;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class RangedAttackAction : GoapActionBase<ActionDataForSkillUsage>
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

        public class Data : IActionData // TODO : 每个都用一个
        {
            public ITarget Target { get; set; }
        }
    }
}
