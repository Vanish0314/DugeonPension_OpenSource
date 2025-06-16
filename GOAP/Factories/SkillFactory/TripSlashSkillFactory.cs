using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Character;
using Dungeon.DungeonEntity;

namespace Dungeon.GOAP
{
    /// <summary>
    /// 摔绊斩击
    /// </summary>
    public class TripSlashCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("摔绊斩击");

            #region Actions
            builder.AddAction<TripSlashAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardDungeonMonster>>()
                .AddCondition<LocalHeroPropertyPointOf<IMagicPointProperty>>(Comparison.GreaterThanOrEqual, 12) // 魔力值大于等于12
                .AddCondition<LocalSkillCooldownReadyKey<TripSlashSkill>>(Comparison.GreaterThanOrEqual, 1) // 冷却完毕
                .AddCondition<LocalNearByEntityCountOf<StandardDungeonMonster>>(Comparison.GreaterThan, 0) // 周围有怪物
                .AddEffect<LocalNearByEntityCountOf<StandardDungeonMonster>>(EffectType.Decrease) // 周围怪物数量减少
                .AddEffect<LocalHeroPropertyPointOf<IMagicPointProperty>>(EffectType.Decrease) // mp减少
                .SetBaseCost(12);
            #endregion

            return builder.Build();
        }
    }
    public class TripSlashAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return agent.LowLevelSystem.IsInSkillRange(new SkillDesc("摔绊斩击"), distance);
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            return agent.LowLevelSystem.UseSkill(new SkillDesc("摔绊斩击"), data.PositionToUseSkill, data.DirectionToUseSkill);
        }

        public override float GetCost(IActionReceiver agent, IComponentReference references, ITarget target)
        {
            var baseCost = base.GetCost(agent, references, target);

            var characterTrait = references.GetCachedComponent<AgentLowLevelSystem>().CharacterTrait;
            var dndSkill = references.GetCachedComponent<AgentLowLevelSystem>().DndSkillData;

            return baseCost + characterTrait.Aggressive - dndSkill.StrengthModifyValue; // 12 - 激进 - 战斗
        }
    }
}
