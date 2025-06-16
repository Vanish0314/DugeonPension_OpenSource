using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Character;
using Dungeon.GOAP;
using UnityEngine;

namespace Dungeon
{
    public class LightBallSkillFactory : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return agent.LowLevelSystem.IsInSkillRange(new SkillDesc("光球术"), distance);
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            return agent.LowLevelSystem.UseSkill(new SkillDesc("光球术"), data.PositionToUseSkill, data.DirectionToUseSkill);
        }

        public override float GetCost(IActionReceiver agent, IComponentReference references, ITarget target)
        {
            var baseCost = base.GetCost(agent, references, target);

            var characterTrait = references.GetCachedComponent<AgentLowLevelSystem>().CharacterTrait;
            var dndSkill = references.GetCachedComponent<AgentLowLevelSystem>().DndSkillData;
            return baseCost + characterTrait.Aggressive - dndSkill.DexterityModifyValue;
        }
    }
}
