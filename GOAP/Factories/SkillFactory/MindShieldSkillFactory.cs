using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP;
using Dungeon.GOAP.Action;
using Dungeon.GOAP.Keys.WorldKey.Local;
using UnityEngine;

namespace Dungeon
{
    /// <summary>
    /// 心灵护盾
    /// </summary>
    public class MindShieldSkillFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("心灵护盾");

            #region Actions

            builder.AddAction<MindShieldAction>()
                .SetTargetKey<NearestTeammateTargetKey>() // · 释放地点: 最近队友 ·
                .AddCondition<LocalHeroPropertyPointOf<IMagicPointProperty>>(Comparison.GreaterThanOrEqual, 12) // · 释放条件：有足够MP ·
                .AddCondition<LocalSkillCooldownReadyKey<MindShieldSkill>>(Comparison.GreaterThanOrEqual, 1) // · 释放条件：技能冷却完毕 ·
                .AddEffect<LocalHeroPropertyPointOf<IMagicPointProperty>>(EffectType.Decrease) // 减少mp
                .SetBaseCost(12); // · 基础Cost=12 ·

            #endregion

            #region Sensors
            // TODO
            // NearestTeammateTargetKey
            #endregion

            return builder.Build();
        }
    }

    public class MindShieldAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return agent.LowLevelSystem.IsInSkillRange(new SkillDesc("心灵护盾"), distance); // · 检查技能是否在释放范围内 ·
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            return agent.LowLevelSystem.UseSkill(new SkillDesc("心灵护盾"), data.PositionToUseSkill, data.DirectionToUseSkill); // · 实际释放技能 ·
        }

        public override float GetCost(IActionReceiver agent, IComponentReference references, ITarget target)
        {
            var baseCost = base.GetCost(agent, references, target); // · 获取基础Cost ·

            var trait = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().CharacterTrait;

            return baseCost - trait.Sympathy; // · Cost=12 - 同理 ·
        }
    }
}
