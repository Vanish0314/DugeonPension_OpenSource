using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.Monster;
using Dungeon.GOAP.Action;
using Dungeon.GOAP.Goals;
using Dungeon.GOAP.Keys.TargetKey;
using Dungeon.GOAP.Keys.WorldKey.Local;
using Dungeon.GOAP.Sensor.Target;
using Dungeon.GOAP.Sensors.Multi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Dungeon.GOAP
{
    /// <summary>
    /// 至圣斩
    /// </summary>
    public class HolySlashCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("至圣斩");

            #region Actions
            builder.AddAction<HolySlashAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardDungeonMonster>>() // 目标是最近的敌人
                .AddCondition<LocalSkillCooldownReadyKey<HolySlashSkill>>(Comparison.SmallerThan, 1) // 冷却完毕
                .AddCondition<LocalHeroPropertyPointOf<IMagicPointProperty>>(Comparison.GreaterThan, 12) //  mp > 12
                .AddCondition<LocalNearByEntityCountOf<StandardDungeonMonster>>(Comparison.GreaterThan, 0) // 周围有敌人
                .AddEffect<LocalNearByEntityCountOf<StandardDungeonMonster>>(EffectType.Decrease) // 敌人周围的敌人数量减少
                .AddEffect<LocalHeroPropertyPointOf<IMagicPointProperty>>(EffectType.Decrease) // mp 减少
                .SetBaseCost(10);
            #endregion

            return builder.Build();
        }
    }

    public class HolySlashAction : GoapActionBase<ActionDataForSkillUsage>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return agent.LowLevelSystem.IsInSkillRange(new SkillDesc("至圣斩"), distance);
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataForSkillUsage data, IActionContext context)
        {
            return agent.LowLevelSystem.UseSkill(new SkillDesc("至圣斩"), data.PositionToUseSkill, data.DirectionToUseSkill);
        }

        public override float GetCost(IActionReceiver agent, IComponentReference references, ITarget target)
        {
            var baseCost = base.GetCost(agent, references, target);

            var characterTrait = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().CharacterTrait;
            var dndSkill = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().DndSkillData;
            return baseCost + characterTrait.Aggressive - dndSkill.Strength; // 10 - 激进 - 战斗
        }
    }
}
