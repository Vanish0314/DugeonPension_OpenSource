using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class UseSkillSongofValorAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return agent.LowLevelSystem.IsInSkillRange(new SkillDesc("Song of Valor"), distance);
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {

            return agent.LowLevelSystem.UseSkill(new SkillDesc("Song of Valor"), data.PositionToUseSkill, data.DirectionToUseSkill);
        }
    }
}
