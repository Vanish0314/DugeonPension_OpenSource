
//========================================
// THIS IS AUTOMATICALLY GENERATED CODE.
//========================================
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon;
using Dungeon.AgentLowLevelSystem;
using Dungeon.DungeonEntity.Torch;
using Dungeon.DungeonEntity.Trap;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Sensors.Multi
{
    public class AgentBlackboardSensor_AutoGen : MultiSensorBase
    {
        public override void Created()
        {
        }

        public override void Update()
        {
        }

        public AgentBlackboardSensor_AutoGen()
        {
        AddLocalWorldSensor<LocalNearByCountOf<SpikeTrap>>(SenseNumInBlackboardSpikeTrap);
        AddLocalWorldSensor<LocalNearByCountOf<Torch>>(SenseNumInBlackboardTorch);
        }



        private SenseValue SenseNumInBlackboardSpikeTrap(IActionReceiver agent, IComponentReference references)
        {
            var blackboard = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
#if UNITY_EDITOR
            {
                if (blackboard == null)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");

                var hasKey = blackboard.CheckIfKeyExists("NumOfSpikeTrapInVision");
                if (!hasKey)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard key 'NumOfSpikeTrapInVision' does not exist");
                else
                {
                    var Ekey = blackboard.GetOrRegisterKey("NumOfSpikeTrapInVision");
                    bool getSuccessful = blackboard.TryGetValue<int>(Ekey, out var Evalue);
                    if (!getSuccessful)
                        GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Failed to get value from blackboard key 'NumOfSpikeTrapInVision'");
                }
            }
#endif
            var key = blackboard.GetOrRegisterKey("NumOfSpikeTrapInVision");
            var hasValue = blackboard.TryGetValue<int>(key, out var value);
            if (!hasValue)
                blackboard.SetValue(key, 0);

            return hasValue ? value : 0;
        }

        private SenseValue SenseNumInBlackboardTorch(IActionReceiver agent, IComponentReference references)
        {
            var blackboard = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
#if UNITY_EDITOR
            {
                if (blackboard == null)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");

                var hasKey = blackboard.CheckIfKeyExists("NumOfTorchInVision");
                if (!hasKey)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard key 'NumOfTorchInVision' does not exist");
                else
                {
                    var Ekey = blackboard.GetOrRegisterKey("NumOfTorchInVision");
                    bool getSuccessful = blackboard.TryGetValue<int>(Ekey, out var Evalue);
                    if (!getSuccessful)
                        GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Failed to get value from blackboard key 'NumOfTorchInVision'");
                }
            }
#endif
            var key = blackboard.GetOrRegisterKey("NumOfTorchInVision");
            var hasValue = blackboard.TryGetValue<int>(key, out var value);
            if (!hasValue)
                blackboard.SetValue(key, 0);

            return hasValue ? value : 0;
        }

    }
}
