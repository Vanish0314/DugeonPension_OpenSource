using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Character;
using Dungeon.DungeonEntity;
using Dungeon.GOAP;

namespace Dungeon
{
    public class TauntSkillFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("TauntCapability");

            #region Actions

            builder.AddAction<TauntAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardDungeonMonster>>() // 任意敌人目标
                .AddCondition<LocalHeroPropertyPointOf<IMagicPointProperty>>(Comparison.GreaterThanOrEqual, 5) // MP 检查
                .AddCondition<LocalSkillCooldownReadyKey<TauntSkill>>(Comparison.GreaterThanOrEqual, 1) // 技能冷却
                .AddCondition<LocalNearByEntityCountOf<StandardDungeonMonster>>(Comparison.GreaterThan, 0) // 范围内有敌人
                //TODO 效果
                .AddEffect<GolbalHeroTeamBeingAttackedHeroCountKey>(EffectType.Decrease)
                .SetBaseCost(5); // 基础 Cost

            #endregion

            return builder.Build();
        }
    }
    public class TauntAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return agent.LowLevelSystem.IsInSkillRange(new SkillDesc("嘲讽"), distance); // 范围判断
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            return agent.LowLevelSystem.UseSkill(new SkillDesc("嘲讽"), data.PositionToUseSkill, data.DirectionToUseSkill); // 技能执行
        }

        public override float GetCost(IActionReceiver agent, IComponentReference references, ITarget target)
        {
            var baseCost = base.GetCost(agent, references, target);

            var trait = references.GetCachedComponent<AgentLowLevelSystem>().CharacterTrait;

            return baseCost + trait.Sympathy; // Cost = 5 - 同理
        }
    }

}
