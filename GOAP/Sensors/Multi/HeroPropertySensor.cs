using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.AgentLowLevelSystem;
using Dungeon.GOAP.Keys.WorldKeys.Local;
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

        private SenseValue SenseHP(IActionReceiver agent, IComponentReference references)
        {
            var blackboard = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
#if UNITY_EDITOR
            {
                if (blackboard == null)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");
            }
#endif

            var key = blackboard.GetOrRegisterKey(AgentBlackBoardEnum.CurrentHP);
            var hasValue = blackboard.TryGetValue<int>(key, out var value);
            if (!hasValue)
            {
                blackboard.SetValue(key, 100);
            }
            return value;
        }
        private SenseValue SenseMP(IActionReceiver agent, IComponentReference references)
        {
            var blackboard = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
#if UNITY_EDITOR
            {
                if (blackboard == null)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");
            }
#endif

            var key = blackboard.GetOrRegisterKey(AgentBlackBoardEnum.CurrentMP);
            var hasValue = blackboard.TryGetValue<int>(key, out var value);
            if (!hasValue)
            {
                blackboard.SetValue(key, 100);
            }
            return value;
        }
    }
}
