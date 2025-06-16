using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
namespace Dungeon.GOAP
{
    public class ZakuCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("FinishDungeonCapability");

            builder.AddGoal<FollowMainHeroGoal>()
                   .SetBaseCost(30)
                   .AddCondition<LocalDistanceToMainHeroKey>(Comparison.SmallerThan, 3);

            builder.AddAction<FollowMainHeroAction>()
                   .SetTargetKey<GlobalMainHeroTransformKey>()
                   .SetBaseCost(20)
                   .AddEffect<LocalDistanceToMainHeroKey>(EffectType.Decrease)
                   .AddCondition<LocalDistanceToMainHeroKey>(Comparison.GreaterThan, 5);

            builder.AddTargetSensor<GlobalMainHeroTransformKeySensor>()
                   .SetTargetKey<GlobalMainHeroTransformKey>();

            builder.AddWorldSensor<LocalDistanceToMainHeroKeySensor>()
                   .SetKey<LocalDistanceToMainHeroKey>();

            return builder.Build();
        }
    }
}
