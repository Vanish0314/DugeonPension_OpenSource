using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.AgentLowLevelSystem;
using Dungeon.GOAP.Keys.WorldKey.Local;
using GameFramework;
using UnityEngine;

namespace Dungeon
{
    public class HeroPropertySensor : MultiSensorBase
    {
        public override void Created()
        {

        }

        public override void Update()
        {
            
        }

        public HeroPropertySensor() 
        {
            AddLocalWorldSensor<LocalHeroPropertyPointOf<IHealthPointProperty>>(SenseHP);
            AddLocalWorldSensor<LocalHeroPropertyPointOf<IMagicPointProperty>>(SenseMP);
        }

        private SenseValue SenseMP(IActionReceiver receiver, IComponentReference reference)
        {
            return reference.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().Mp;
        }

        private SenseValue SenseHP(IActionReceiver receiver, IComponentReference reference)
        {
            return reference.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>().Hp;
        }
    }
}
