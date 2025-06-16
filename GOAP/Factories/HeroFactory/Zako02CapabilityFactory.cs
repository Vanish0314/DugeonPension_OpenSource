using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity;
using Dungeon.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class Zako02CapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("FinishDungeonCapability");
            #region Goals
            builder.AddGoal<FinishDungeonGoal>() // ÕÍ≥…µÿ¿Œ
                   .SetBaseCost(100)
                   .AddCondition<HeroIsAtDungeonExitWorldKey>(Comparison.GreaterThanOrEqual, 1);

            builder.AddGoal<EliminateThreatGoal>()
                .SetBaseCost(20)
                .AddCondition<LocalNearByEntityCountOf<StandardTrap>>(Comparison.SmallerThanOrEqual, 0)
                .AddCondition<LocalNearByEntityCountOf<StandardDungeonMonster>>(Comparison.SmallerThanOrEqual, 0);


            #endregion

            #region Actions
            builder.AddAction<FinishDungeonAction>()
                .SetTargetKey<DungeonExitTargetKey>()
                .AddEffect<HeroIsAtDungeonExitWorldKey>(EffectType.Increase)
                .SetProperties(new FinishDungeonAction.Props())
                .SetBaseCost(10);

            builder.AddAction<MeleeAttackAction>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardDungeonMonster>>()
                .AddEffect<LocalNearByEntityCountOf<StandardDungeonMonster>>(EffectType.Decrease)
                //.AddCondition<LocalHeroPropertyPointOf<IHealthPointProperty>>(Comparison.GreaterThan, UnityEngine.Random.Range(1, 10))
                .SetBaseCost(10);




            #endregion

            #region Sensors
            builder.AddTargetSensor<DungeonExitTargetSensor>()
               .SetTargetKey<DungeonExitTargetKey>();
            builder.AddWorldSensor<HeroIsAtDungeonExitSensor>()
               .SetKey<HeroIsAtDungeonExitWorldKey>();

            builder.AddMultiSensor<AgentBlackboardSensor_AutoGen>();
            builder.AddMultiSensor<HeroPropertySensor>();
            #endregion
            return builder.Build();
        }


    }
}
