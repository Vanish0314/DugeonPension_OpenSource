using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.AgentLowLevelSystem;
using Dungeon.GOAP.Action;
using UnityEngine;

namespace Dungeon
{
    public class MeleeAttackAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return agent.LowLevelSystem.IsInSkillRange(new SkillDesc("近战猛击"), distance);
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            return agent.LowLevelSystem.UseSkill(new SkillDesc("近战猛击"), data.PositionToUseSkill, data.DirectionToUseSkill);
        }

        public override float GetCost(IActionReceiver agent, IComponentReference references, ITarget target)
        {
            var baseCost = base.GetCost(agent, references, target);

            var characterTrait = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().CharacterTrait;
            var dndSkill = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().DndSkillData;
            return baseCost + characterTrait.Aggressive - dndSkill.DexterityModifyValue;
        }
    }
}