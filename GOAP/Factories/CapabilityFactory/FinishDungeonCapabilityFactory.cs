using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.InteractiveObject;
using Dungeon.DungeonEntity.Monster;
using Dungeon.DungeonEntity.Trap;
using Dungeon.GOAP.Action;
using Dungeon.GOAP.Goals;
using Dungeon.GOAP.Keys.TargetKey;
using Dungeon.GOAP.Keys.WorldKey;
using Dungeon.GOAP.Keys.WorldKey.Local;
using Dungeon.GOAP.Sensor.Key;
using Dungeon.GOAP.Sensor.Target;
using Dungeon.GOAP.Sensors.Multi;

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
                .AddCondition<LocalNearByEntityCountOf<StandardTrap>>(Comparison.SmallerThanOrEqual , 0)
                .AddCondition<LocalNearByEntityCountOf<StandardDungeonMonster>>(Comparison.SmallerThanOrEqual, 0);

            builder.AddGoal<LightDungeonRoomGoal>()
                .SetBaseCost(10)
                .AddCondition<LocalNearByEntityCountOf<StandardTorch>>(Comparison.GreaterThanOrEqual, 9999);

            builder.AddGoal<DesireFulfillmentGoal>()
                .SetBaseCost(5)
                .AddCondition<LocalHeroCoinCountKey>(Comparison.GreaterThanOrEqual, 9999);
#endregion

#region Actions
            //FIXME: It is a bad practice to just coupling the action and target keys like this.
            //It should be done in a more generic way.
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
                .AddCondition<LocalHeroPropertyPointOf<IHealthPointProperty>>(Comparison.GreaterThan, 0)
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
            builder.AddTargetSensor<NearestMonsterTargetSensor>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardDungeonMonster>>();
            

            builder.AddWorldSensor<HeroIsAtDungeonExitSensor>()
                .SetKey<HeroIsAtDungeonExitWorldKey>();

            builder.AddMultiSensor<AgentBlackboardSensor_AutoGen>();
            builder.AddMultiSensor<HeroPropertySensor>();
#endregion

            return builder.Build();
        }
    }
}
