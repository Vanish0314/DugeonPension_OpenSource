using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity;
using Dungeon.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class KnightCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("FinishDungeonCapability");

            #region Goals
            builder.AddGoal<FinishDungeonGoal>() // 完成地牢
                   .SetBaseCost(100)
                   .AddCondition<HeroIsAtDungeonExitWorldKey>(Comparison.GreaterThanOrEqual, 1);
            builder.AddGoal<ProtectTeammatesGoal>() // 保护队友Goal
              .SetBaseCost(100)
              .AddCondition<GlobalHeroTeamHpBelow70PercentageFriendlyHeroCountKey>(Comparison.SmallerThan, 1) // 1. 血量低于70%的队友数量 < 1
              .AddCondition<GolbalHeroTeamBeingAttackedHeroCountKey>(Comparison.SmallerThan, 2); // 2. 正在被攻击的队友的数量 < 2
            
            builder.AddGoal<EliminateThreatGoal>()
                .SetBaseCost(20)
                .AddCondition<LocalNearByEntityCountOf<StandardTrap>>(Comparison.SmallerThanOrEqual, 0)
                .AddCondition<LocalNearByEntityCountOf<StandardDungeonMonster>>(Comparison.SmallerThanOrEqual, 0);

            builder.AddGoal<LightDungeonRoomGoal>()
                .SetBaseCost(10)
                .AddCondition<LocalNearByEntityCountOf<StandardTorch>>(Comparison.GreaterThanOrEqual, 9999);

            builder.AddGoal<DesireFulfillmentGoal>()
                .SetBaseCost(5)
                .AddCondition<LocalHeroCoinCountKey>(Comparison.GreaterThanOrEqual, 9999);
            #endregion

            #region Actions
            builder.AddAction<FinishDungeonAction>()
                .SetTargetKey<DungeonExitTargetKey>()
                .AddEffect<HeroIsAtDungeonExitWorldKey>(EffectType.Increase)
                .SetProperties(new FinishDungeonAction.Props())
                .SetBaseCost(10);

            builder.AddAction<DisarmTrapAciton>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardTrap>>()
                .AddEffect<LocalNearByEntityCountOf<StandardTrap>>(EffectType.Decrease)
                .AddCondition<LocalNearByEntityCountOf<StandardTrap>>(Comparison.GreaterThan, 0)
                .SetBaseCost(10);

            builder.AddAction<LightTorchAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardTorch>>()
                .AddEffect<LocalNearByEntityCountOf<StandardTorch>>(EffectType.Increase)
                .AddCondition<LocalNearByEntityCountOf<StandardTorch>>(Comparison.GreaterThan, 0)
                .SetBaseCost(1);

            builder.AddAction<OpenTreasureChestAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardDungeonTreasureChest>>()
                .AddEffect<LocalHeroCoinCountKey>(EffectType.Increase)
                .AddCondition<LocalNearByEntityCountOf<StandardDungeonTreasureChest>>(Comparison.GreaterThan, 0)
                .SetBaseCost(10);

            builder.AddAction<MeleeAttackAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardDungeonMonster>>()
                .AddEffect<LocalNearByEntityCountOf<StandardDungeonMonster>>(EffectType.Decrease)
                .AddCondition<LocalHeroPropertyPointOf<IHealthPointProperty>>(Comparison.GreaterThan, UnityEngine.Random.Range(1, 10))
                .SetBaseCost(10);
            #endregion

            #region Sensors
            builder.AddTargetSensor<DungeonExitTargetSensor>()
                .SetTargetKey<DungeonExitTargetKey>();
            builder.AddTargetSensor<NearestTrapTargetSensor>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardTrap>>();
            builder.AddTargetSensor<NearestTorchTargetSensor>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardTorch>>();
            builder.AddTargetSensor<NearestChestTargetSensor>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardDungeonTreasureChest>>();

            builder.AddWorldSensor<HeroIsAtDungeonExitSensor>()
                .SetKey<HeroIsAtDungeonExitWorldKey>();

            builder.AddMultiSensor<AgentBlackboardSensor_AutoGen>();
            builder.AddMultiSensor<HeroPropertySensor>();

            builder.AddWorldSensor<GolbalHeroTeamBeingAttackedHeroCountKeySensor>()
                    .SetKey<GolbalHeroTeamBeingAttackedHeroCountKey>();
            #endregion

            return builder.Build();
        }
    }
}
