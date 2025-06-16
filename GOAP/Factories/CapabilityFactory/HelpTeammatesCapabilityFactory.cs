using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP;
using UnityEngine;

namespace Dungeon
{
    public class HelpTeammatesCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("FinishDungeonCapability");

            #region  Goals
            builder.AddGoal<ProtectTeammatesGoal>() // 保护队友Goal
               .SetBaseCost(100)
               .AddCondition<GlobalHeroTeamHpBelow70PercentageFriendlyHeroCountKey>(Comparison.SmallerThan, 1) // 1. 血量低于70%的队友数量 < 1
               .AddCondition<GolbalHeroTeamBeingAttackedHeroCountKey>(Comparison.SmallerThan, 2); // 2. 正在被攻击的队友的数量 < 2

            builder.AddGoal<StrengthenTeammatesGoal>() // 强化队友Goal
                 .SetBaseCost(100)
                 .AddCondition<GlobalHeroTeamDontHasPositiveBuffHeroCountKey>(Comparison.SmallerThan, 1);
            #endregion

            #region Sensor
            builder.AddWorldSensor<GolbalHeroTeamBeingAttackedHeroCountKeySensor>()
                    .SetKey<GolbalHeroTeamBeingAttackedHeroCountKey>();

            builder.AddWorldSensor<GlobalHeroTeamDontHasPositiveBuffHeroCountKeySensor>()
                    .SetKey<GlobalHeroTeamDontHasPositiveBuffHeroCountKey>();
            #endregion

            return builder.Build();
        }
    }
}
