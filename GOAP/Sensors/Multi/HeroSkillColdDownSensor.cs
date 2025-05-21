using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Goap;
using Dungeon.GOAP;
using GameFramework;
using UnityEngine;

namespace Dungeon
{
    public class HeroSkillColdDownSensor : MultiSensorBase
    {
        public override void Created()
        {

        }

        public override void Update()
        {
            
        }

        public HeroSkillColdDownSensor() 
        {
            AddLocalWorldSensor<LocalSkillCooldownReadyKey<TripSlashSkill>>(SenseSkillColdDown("摔绊斩击"));
            AddLocalWorldSensor<LocalSkillCooldownReadyKey<HolySlashSkill>>(SenseSkillColdDown("至圣斩"));
            AddLocalWorldSensor<LocalSkillCooldownReadyKey<HealSkill>>(SenseSkillColdDown("治疗术"));
            AddLocalWorldSensor<LocalSkillCooldownReadyKey<FlurryOfBlowsSkill>>(SenseSkillColdDown("动作如潮"));
            AddLocalWorldSensor<LocalSkillCooldownReadyKey<TauntSkill>>(SenseSkillColdDown("嘲讽"));
            AddLocalWorldSensor<LocalSkillCooldownReadyKey<FireballSkill>>(SenseSkillColdDown("火球术"));
            AddLocalWorldSensor<LocalSkillCooldownReadyKey<MindShieldSkill>>(SenseSkillColdDown("心灵护盾"));
        }

        private Func<IActionReceiver, IComponentReference, SenseValue> SenseSkillColdDown(string skillName)
        {
            return (receiver, reference) =>
            {
                var result = reference.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().CheckIsSkillReady(skillName);
                return result? 1 : 0;
            };
        }
    }
}