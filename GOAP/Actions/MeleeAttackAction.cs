using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Action;
using UnityEngine;

namespace Dungeon
{
    public class MeleeAttackAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return agent.IsInSkillRange(new SkillDesc("Melee Attack"), distance);
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            return agent.LowLevelSystem.UseSkill(new SkillDesc("Melee Attack"), data.PositionToUseSkill, data.DirectionToUseSkill);
        }
    }
}