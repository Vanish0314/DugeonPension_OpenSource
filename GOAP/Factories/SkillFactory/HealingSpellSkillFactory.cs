using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Goap;
using Dungeon.GOAP.Action;
using Dungeon.GOAP.Keys.WorldKey.Local;
using PlasticGui.Help.Actions;
using UnityEngine;

namespace Dungeon.GOAP
{
    /// <summary>
    /// 治疗术
    /// </summary>
    public class HealingSpellSkillFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("治疗术");

            #region Actions

            builder.AddAction<HealAction>()
                .SetTargetKey<NearestHpBelow70PercentageFriendlyHeroTargetKey>() // · 释放位置：最近的生命值 < 70% 的友军 ·
                .AddCondition<LocalHeroPropertyPointOf<IMagicPointProperty>>(Comparison.GreaterThanOrEqual, 12) // · 释放条件：有足够MP ·
                .AddCondition<LocalSkillCooldownReadyKey<HealSkill>>(Comparison.GreaterThanOrEqual, 1) // · 释放条件：技能冷却完毕 ·
                .AddEffect<GlobalHeroTeamHpBelow70PercentageFriendlyHeroCountKey>(EffectType.Decrease) // 减少队伍中70%血量以下人数的数量
                .AddEffect<LocalHeroPropertyPointOf<IMagicPointProperty>>(EffectType.Decrease) // 减少mp
                .SetBaseCost(12); // · 基础Cost=12 ·

            #endregion

            #region Sensors
            // TODO
            builder.AddTargetSensor<NearestHpBelow70PercentageFriendlyHeroTargetKeySensor>()
                .SetTargetKey<NearestHpBelow70PercentageFriendlyHeroTargetKey>();
            builder.AddWorldSensor<WorldHeroTeamHpBelow70PercentageFriendlyHeroCountKeySensor>()
                .SetKey<GlobalHeroTeamHpBelow70PercentageFriendlyHeroCountKey>();
            #endregion

            return builder.Build();
        }
    }

    public class HealAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return agent.LowLevelSystem.IsInSkillRange(new SkillDesc("治疗术"), distance); // · 检查技能是否在释放范围内 ·
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            return agent.LowLevelSystem.UseSkill(new SkillDesc("治疗术"), data.PositionToUseSkill, data.DirectionToUseSkill); // · 实际释放技能 ·
        }

        public override float GetCost(IActionReceiver agent, IComponentReference references, ITarget target)
        {
            var baseCost = base.GetCost(agent, references, target); // · 获取基础Cost ·

            var trait = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().CharacterTrait;
            var dnd = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().DndSkillData;

            return baseCost + trait.Sympathy - dnd.Wisdom; // · Cost=12 + 同理 - 感知 ·
        }
    }

}
