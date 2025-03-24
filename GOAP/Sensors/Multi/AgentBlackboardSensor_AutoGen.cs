
//========================================
// THIS IS AUTOMATICALLY GENERATED CODE.
//========================================
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.Trap;
using GameFramework;

namespace Dungeon.GOAP.Sensors.Multi
{
    public class AgentBlackboardSensor_AutoGen : MultiSensorBase
    {
        public override void Created()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public AgentBlackboardSensor_AutoGen()
        {
        AddLocalWorldSensor<LocalNearByCountOf<SpikeTrap>>(SenseNumInBlackboardSpikeTrap);
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
            blackboard.TryGetValue<int>(key, out var value);
            return value;
        }
    }
}
