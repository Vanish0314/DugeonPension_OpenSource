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
    /// 火球术
    /// </summary>
    public class FireballSpellSkillFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("火球术");

            #region Actions

            builder.AddAction<FireballAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardDungeonMonster>>() // 以怪物为目标
                .AddCondition<LocalHeroPropertyPointOf<IMagicPointProperty>>(Comparison.GreaterThanOrEqual, 15) // MP 条件
                .AddCondition<LocalSkillCooldownReadyKey<FireballSkill>>(Comparison.GreaterThanOrEqual, 1) // 冷却判断
                .AddCondition<LocalNearByEntityCountOf<StandardDungeonMonster>>(Comparison.GreaterThan, 0) // 范围内存在敌人
                .AddEffect<LocalHeroPropertyPointOf<IMagicPointProperty>>(EffectType.Decrease) // MP 减少
                .AddEffect<LocalNearByEntityCountOf<StandardDungeonMonster>>(EffectType.Decrease) // 敌人减少
                .SetBaseCost(15);

            #endregion

            return builder.Build();
        }
    }
    public class FireballAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return agent.LowLevelSystem.IsInSkillRange(new SkillDesc("火球术"), distance);
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            return agent.LowLevelSystem.UseSkill(new SkillDesc("火球术"), data.PositionToUseSkill, data.DirectionToUseSkill);
        }

        public override float GetCost(IActionReceiver agent, IComponentReference references, ITarget target)
        {
            var baseCost = base.GetCost(agent, references, target);

            var trait = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().CharacterTrait;
            var dnd = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().DndSkillData;

            return baseCost - trait.Aggressive - dnd.StrengthModifyValue; // Cost = 15 - 激进 - 力量
        }
    }

}
