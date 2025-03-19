using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Goals;
using Dungeon.GOAP.Keys.WorldKeys;
using Dungeon.GOAP.Sensors.Key;
using Dungeon.GOAP.Sensors.Target;
using Dungeon.GOAP.Targets;
using UnityEngine;

namespace Dungeon.GOAP.Factories.CapabilityFactory
{
    public class FinishDungeonCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("FinishDungeonCapability");

            builder.AddGoal<FinishDungeonGoal>()
                .SetBaseCost(30)
                .AddCondition<HeroIsAtDungeonExitWorldKey>(Comparison.GreaterThanOrEqual, 1);

            //FIXME(vanish): It is a bad practice to just coupling the action and target keys like this.
            //It should be done in a more generic way.
            builder.AddAction<FinishDungeonAction>()
                .SetTargetKey<DungeonExitTargetKey>()
                .AddEffect<HeroIsAtDungeonExitWorldKey>(EffectType.Increase)
                .SetProperties(new FinishDungeonAction.Props());

            builder.AddTargetSensor<DungeonExitTargetSensor>()
                .SetTargetKey<DungeonExitTargetKey>();

            builder.AddWorldSensor<HeroIsAtDungeonExitSensor>()
                .SetKey<HeroIsAtDungeonExitWorldKey>();

            return builder.Build();
        }
    }
}
