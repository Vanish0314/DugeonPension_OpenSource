using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.Monster;
using Dungeon.DungeonEntity.Torch;
using Dungeon.DungeonEntity.Trap;
using Dungeon.GOAP.Action;
using Dungeon.GOAP.Goals;
using Dungeon.GOAP.Keys.TargetKey;
using Dungeon.GOAP.Keys.WorldKeys;
using Dungeon.GOAP.Keys.WorldKeys.Local;
using Dungeon.GOAP.Sensors.Key;
using Dungeon.GOAP.Sensors.Multi;
using Dungeon.GOAP.Sensors.Target;
using Dungeon.GOAP.Targets;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Dungeon.GOAP.Factories.CapabilityFactory
{
    public class FinishDungeonCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("FinishDungeonCapability");

#region Goals
            builder.AddGoal<FinishDungeonGoal>()
                .SetBaseCost(100)
                .AddCondition<HeroIsAtDungeonExitWorldKey>(Comparison.GreaterThanOrEqual, 1);

            builder.AddGoal<AliveGoal>()
                .SetBaseCost(10)
                .AddCondition<LocalHeroPropertyPointOf<IHealthPointProperty>>(Comparison.GreaterThanOrEqual, 1);

            builder.AddGoal<EliminateThreatGoal>()
                .SetBaseCost(20)
                .AddCondition<LocalNearByEntityCountOf<SpikeTrap>>(Comparison.SmallerThanOrEqual , 0)
                .AddCondition<LocalNearByEntityCountOf<DungeonMonsterBase>>(Comparison.SmallerThanOrEqual, 0);

            builder.AddGoal<LightDungeonRoomGoal>()
                .SetBaseCost(10)
                .AddCondition<LocalNearByEntityCountOf<Torch>>(Comparison.GreaterThanOrEqual, 9999);

            builder.AddGoal<DesireFulfillmentGoal>()
                .SetBaseCost(5)
                .AddCondition<LocalHeroCoinCountKey>(Comparison.GreaterThanOrEqual, 9999);
#endregion

#region Actions
            //FIXME(vanish): It is a bad practice to just coupling the action and target keys like this.
            //It should be done in a more generic way.
            builder.AddAction<FinishDungeonAction>()
                .SetTargetKey<DungeonExitTargetKey>()
                .AddEffect<HeroIsAtDungeonExitWorldKey>(EffectType.Increase)
                .SetProperties(new FinishDungeonAction.Props())
                .SetBaseCost(10);

            builder.AddAction<DisarmTrapAciton>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<SpikeTrap>>()
                .AddEffect<LocalNearByEntityCountOf<SpikeTrap>>(EffectType.Decrease)
                .AddCondition<LocalNearByEntityCountOf<SpikeTrap>>(Comparison.GreaterThan, 0)
                .SetBaseCost(10);

            builder.AddAction<LightTorchAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<Torch>>()
                .AddEffect<LocalNearByEntityCountOf<Torch>>(EffectType.Increase)
                .AddCondition<LocalNearByEntityCountOf<Torch>>(Comparison.GreaterThan, 0)
                .SetBaseCost(1);

            builder.AddAction<OpenTreasureChestAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<DungeonTreasureChest>>()
                .AddEffect<LocalHeroCoinCountKey>(EffectType.Increase)
                .AddCondition<LocalNearByEntityCountOf<DungeonTreasureChest>>(Comparison.GreaterThan, 0)
                .SetBaseCost(10);

            builder.AddAction<MeleeAttackAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<DungeonMonsterBase>>()
                .AddEffect<LocalNearByEntityCountOf<DungeonMonsterBase>>(EffectType.Decrease)
                .AddCondition<LocalHeroPropertyPointOf<IHealthPointProperty>>(Comparison.GreaterThan, 0)
                .SetBaseCost(10);
            
            builder.AddAction<RangedAttackAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<DungeonMonsterBase>>()
                .AddEffect<LocalNearByEntityCountOf<DungeonMonsterBase>>(EffectType.Decrease)
                .AddCondition<LocalHeroPropertyPointOf<IHealthPointProperty>>(Comparison.GreaterThan, 0)
                .AddCondition<LocalHeroPropertyPointOf<IMagicPointProperty>>(Comparison.GreaterThan, 0)
                .SetBaseCost(10);
            
            builder.AddAction<HealMagicAction>()
                .AddEffect<LocalHeroPropertyPointOf<IHealthPointProperty>>(EffectType.Increase)
                .AddCondition<LocalHeroPropertyPointOf<IHealthPointProperty>>(Comparison.GreaterThan, 0)
                .AddCondition<LocalHeroPropertyPointOf<IMagicPointProperty>>(Comparison.GreaterThan, 10)
                .SetBaseCost(10);

#endregion

#region Sensors
            builder.AddTargetSensor<DungeonExitTargetSensor>()
                .SetTargetKey<DungeonExitTargetKey>();
            builder.AddTargetSensor<NearestTrapTargetSensor>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<SpikeTrap>>();
            builder.AddTargetSensor<NearestTorchTargetSensor>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<Torch>>();
            builder.AddTargetSensor<NearestChestTargetSensor>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<DungeonTreasureChest>>();
            builder.AddTargetSensor<NearestMonsterTargetSensor>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<DungeonMonsterBase>>();
            

            builder.AddWorldSensor<HeroIsAtDungeonExitSensor>()
                .SetKey<HeroIsAtDungeonExitWorldKey>();

            builder.AddMultiSensor<AgentBlackboardSensor_AutoGen>();
            builder.AddMultiSensor<HeroPropertySensor>();
#endregion

            return builder.Build();
        }
    }
}
