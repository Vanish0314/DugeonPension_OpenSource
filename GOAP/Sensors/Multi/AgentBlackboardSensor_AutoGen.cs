
//========================================
// THIS IS AUTOMATICALLY GENERATED CODE.
//========================================
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.InteractiveObject;
using Dungeon.DungeonEntity.Monster;
using Dungeon.DungeonEntity.Trap;
using Dungeon.GOAP.Keys.WorldKey.Local;
using GameFramework;

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
        AddLocalWorldSensor<LocalNearByEntityCountOf<StandardTrap>>(SenseNumInBlackboardStandardTrap);
        AddLocalWorldSensor<LocalNearByEntityCountOf<StandardDungeonMonster>>(SenseNumInBlackboardStandardDungeonMonster);
        AddLocalWorldSensor<LocalNearByEntityCountOf<StandardTorch>>(SenseNumInBlackboardStandardTorch);
        AddLocalWorldSensor<LocalNearByEntityCountOf<StandardDungeonTreasureChest>>(SenseNumInBlackboardDungeonTreasureChestBase);
        }



        private SenseValue SenseNumInBlackboardStandardTrap(IActionReceiver agent, IComponentReference references)
        {
            var blackboard = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
#if UNITY_EDITOR
            {
                if (blackboard == null)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");

                var hasKey = blackboard.CheckIfKeyExists("NumOfStandardTrapInVision");
                if (!hasKey)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard key 'NumOfStandardTrapInVision' does not exist");
                else
                {
                    var Ekey = blackboard.GetOrRegisterKey("NumOfStandardTrapInVision");
                    bool getSuccessful = blackboard.TryGetValue<int>(Ekey, out var Evalue);
                    if (!getSuccessful)
                        GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Failed to get value from blackboard key 'NumOfStandardTrapInVision'");
                }
            }
#endif
            var key = blackboard.GetOrRegisterKey("NumOfStandardTrapInVision");
            var hasValue = blackboard.TryGetValue<int>(key, out var value);
            if (!hasValue)
                blackboard.SetValue(key, 0);

            return hasValue ? value : 0;
        }

        private SenseValue SenseNumInBlackboardStandardDungeonMonster(IActionReceiver agent, IComponentReference references)
        {
            var blackboard = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
#if UNITY_EDITOR
            {
                if (blackboard == null)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");

                var hasKey = blackboard.CheckIfKeyExists("NumOfStandardDungeonMonsterInVision");
                if (!hasKey)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard key 'NumOfStandardDungeonMonsterInVision' does not exist");
                else
                {
                    var Ekey = blackboard.GetOrRegisterKey("NumOfStandardDungeonMonsterInVision");
                    bool getSuccessful = blackboard.TryGetValue<int>(Ekey, out var Evalue);
                    if (!getSuccessful)
                        GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Failed to get value from blackboard key 'NumOfStandardDungeonMonsterInVision'");
                }
            }
#endif
            var key = blackboard.GetOrRegisterKey("NumOfStandardDungeonMonsterInVision");
            var hasValue = blackboard.TryGetValue<int>(key, out var value);
            if (!hasValue)
                blackboard.SetValue(key, 0);

            return hasValue ? value : 0;
        }

        private SenseValue SenseNumInBlackboardStandardTorch(IActionReceiver agent, IComponentReference references)
        {
            var blackboard = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
#if UNITY_EDITOR
            {
                if (blackboard == null)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");

                var hasKey = blackboard.CheckIfKeyExists("NumOfStandardTorchInVision");
                if (!hasKey)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard key 'NumOfStandardTorchInVision' does not exist");
                else
                {
                    var Ekey = blackboard.GetOrRegisterKey("NumOfStandardTorchInVision");
                    bool getSuccessful = blackboard.TryGetValue<int>(Ekey, out var Evalue);
                    if (!getSuccessful)
                        GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Failed to get value from blackboard key 'NumOfStandardTorchInVision'");
                }
            }
#endif
            var key = blackboard.GetOrRegisterKey("NumOfStandardTorchInVision");
            var hasValue = blackboard.TryGetValue<int>(key, out var value);
            if (!hasValue)
                blackboard.SetValue(key, 0);

            return hasValue ? value : 0;
        }

        private SenseValue SenseNumInBlackboardDungeonTreasureChestBase(IActionReceiver agent, IComponentReference references)
        {
            var blackboard = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
#if UNITY_EDITOR
            {
                if (blackboard == null)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");

                var hasKey = blackboard.CheckIfKeyExists("NumOfDungeonTreasureChestBaseInVision");
                if (!hasKey)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard key 'NumOfDungeonTreasureChestBaseInVision' does not exist");
                else
                {
                    var Ekey = blackboard.GetOrRegisterKey("NumOfDungeonTreasureChestBaseInVision");
                    bool getSuccessful = blackboard.TryGetValue<int>(Ekey, out var Evalue);
                    if (!getSuccessful)
                        GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Failed to get value from blackboard key 'NumOfDungeonTreasureChestBaseInVision'");
                }
            }
#endif
            var key = blackboard.GetOrRegisterKey("NumOfDungeonTreasureChestBaseInVision");
            var hasValue = blackboard.TryGetValue<int>(key, out var value);
            if (!hasValue)
                blackboard.SetValue(key, 0);

            return hasValue ? value : 0;
        }

    }
}
