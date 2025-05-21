using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.Monster;
using Dungeon.GOAP;
using Dungeon.GOAP.Action;
using Dungeon.GOAP.Keys.TargetKey;
using Dungeon.GOAP.Keys.WorldKey.Local;
using UnityEngine;

namespace Dungeon
{
    /// <summary>
    /// 动作如潮
    /// </summary>
    public class FlurryModeSkillFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("动作如潮");

            #region Actions

            builder.AddAction<FlurryOfBlowsAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardDungeonMonster>>() // 技能目标为最近的敌人
                .AddCondition<LocalHeroPropertyPointOf<IMagicPointProperty>>(Comparison.GreaterThanOrEqual, 14) // MP 足够
                .AddCondition<LocalSkillCooldownReadyKey<FlurryOfBlowsSkill>>(Comparison.GreaterThanOrEqual, 1) // 技能冷却完毕
                .AddCondition<LocalNearByEntityCountOf<StandardDungeonMonster>>(Comparison.GreaterThan, 0) // 周围有敌人
                // .AddEffect<LocalNearByEntityCountOf<StandardDungeonMonster>>(EffectType.Decrease) // TODO: 效果
                .SetBaseCost(14); // 基础 Cost

            #endregion

            return builder.Build();
        }
    }

    public class FlurryOfBlowsAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return agent.LowLevelSystem.IsInSkillRange(new SkillDesc("动作如潮"), distance); // 检查技能释放范围
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            return agent.LowLevelSystem.UseSkill(new SkillDesc("动作如潮"), data.PositionToUseSkill, data.DirectionToUseSkill); // 实际施放技能
        }

        public override float GetCost(IActionReceiver agent, IComponentReference references, ITarget target)
        {
            var baseCost = base.GetCost(agent, references, target);

            var trait = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().CharacterTrait;
            var dnd = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().DndSkillData;

            return baseCost + trait.Aggressive - dnd.StrengthModifyValue; // Cost = 14 - 激进 - 力量
        }
    }
}
