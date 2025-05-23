using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.Monster;
using Dungeon.GOAP;
using Dungeon.GOAP.Keys.TargetKey;
using Dungeon.GOAP.Sensor.Target;
using Dungeon.GOAP.Sensors.Multi;
using UnityEngine;

namespace Dungeon
{
    public class UseSkillCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("Use Skill Capability Factory");

            #region Sensors

            #region Target Sensors
            
            builder.AddTargetSensor<NearestMonsterTargetSensor>()
                .SetTargetKey<NearestEntityTransformTargetKeyOf<StandardDungeonMonster>>();

            builder.AddTargetSensor<NearestTeammateTargetSensor>()
                .SetTargetKey<NearestTeammateTargetKey>();
            #endregion

            builder.AddMultiSensor<HeroSkillColdDownSensor>();
            #endregion

            return builder.Build();
        }
    }
}
