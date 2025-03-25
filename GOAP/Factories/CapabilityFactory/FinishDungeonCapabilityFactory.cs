using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.Torch;
using Dungeon.DungeonEntity.Trap;
using Dungeon.GOAP.Action;
using Dungeon.GOAP.Goals;
using Dungeon.GOAP.Keys.TargetKey;
using Dungeon.GOAP.Keys.WorldKeys;
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
                .SetBaseCost(2)
                .AddCondition<HeroIsAtDungeonExitWorldKey>(Comparison.GreaterThanOrEqual, 1);

            builder.AddGoal<AliveGoal>()
                .SetBaseCost(10)
                .AddCondition<LocalHeroPropertyPointOf<IHealthPointProperty>>(Comparison.GreaterThanOrEqual, 1);

            builder.AddGoal<EliminateThreatGoal>()
                .SetBaseCost(20)
                .AddCondition<LocalNearByCountOf<SpikeTrap>>(Comparison.SmallerThanOrEqual , 0);

            builder.AddGoal<LightDungeonRoomGoal>()
                .SetBaseCost(10)
                .AddCondition<LocalNearByCountOf<Torch>>(Comparison.GreaterThanOrEqual, 9999);
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
                .AddEffect<LocalNearByCountOf<SpikeTrap>>(EffectType.Decrease)
                .SetBaseCost(1);

            builder.AddAction<LightTorchAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<Torch>>()
                .AddEffect<LocalNearByCountOf<Torch>>(EffectType.Increase)
                .SetBaseCost(1);

#endregion

#region Sensors
            builder.AddTargetSensor<DungeonExitTargetSensor>()
                .SetTargetKey<DungeonExitTargetKey>();
            builder.AddTargetSensor<NearestTrapTargetSensor>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<SpikeTrap>>();
            builder.AddTargetSensor<NearestTorchTargetSensor>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<Torch>>();
            

            builder.AddWorldSensor<HeroIsAtDungeonExitSensor>()
                .SetKey<HeroIsAtDungeonExitWorldKey>();

            builder.AddMultiSensor<AgentBlackboardSensor_AutoGen>();
            builder.AddMultiSensor<HeroPropertySensor>();
#endregion

            return builder.Build();
        }
    }
}
