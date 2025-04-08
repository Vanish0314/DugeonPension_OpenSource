
//========================================
// THIS IS AUTOMATICALLY GENERATED CODE.
//========================================
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.Character.Hero;
using Dungeon.DungeonEntity.InteractiveObject;
using Dungeon.DungeonEntity.Monster;
using Dungeon.DungeonEntity.Trap;
using Dungeon.GOAP.Keys.WorldKey.Local;
using GameFramework;

namespace Dungeon.GOAP.Sensor.Multi
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
            AddLocalWorldSensor<LocalNearByEntityCountOf<DungeonTreasureChest>>(SenseNumInBlackboardDungeonTreasureChest);
            AddLocalWorldSensor<LocalNearByEntityCountOf<StandardTrap>>(SenseNumInBlackboardStandardTrap);
            AddLocalWorldSensor<LocalNearByEntityCountOf<Torch>>(SenseNumInBlackboardTorch);
            AddLocalWorldSensor<LocalNearByEntityCountOf<StandardDungeonMonster>>(SenseNumInBlackboardStandardDungeonMonster);
            AddLocalWorldSensor<LocalNearByEntityCountOf<HeroEntityBase>>(SenseNumInBlackboardHeroEntityBase);
        }



        private SenseValue SenseNumInBlackboardDungeonTreasureChest(IActionReceiver agent, IComponentReference references)
        {
            var blackboard = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
#if UNITY_EDITOR
            {
                if (blackboard == null)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");

                var hasKey = blackboard.CheckIfKeyExists("NumOfDungeonTreasureChestInVision");
                if (!hasKey)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard key 'NumOfDungeonTreasureChestInVision' does not exist");
                else
                {
                    var Ekey = blackboard.GetOrRegisterKey("NumOfDungeonTreasureChestInVision");
                    bool getSuccessful = blackboard.TryGetValue<int>(Ekey, out var Evalue);
                    if (!getSuccessful)
                        GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Failed to get value from blackboard key 'NumOfDungeonTreasureChestInVision'");
                }
            }
#endif
            var key = blackboard.GetOrRegisterKey("NumOfDungeonTreasureChestInVision");
            var hasValue = blackboard.TryGetValue<int>(key, out var value);
            if (!hasValue)
                blackboard.SetValue(key, 0);

            return hasValue ? value : 0;
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

        private SenseValue SenseNumInBlackboardHeroEntityBase(IActionReceiver agent, IComponentReference references)
        {
            var blackboard = references.GetCachedComponent<AgentLowLevelSystem.AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
#if UNITY_EDITOR
            {
                if (blackboard == null)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");

                var hasKey = blackboard.CheckIfKeyExists("NumOfHeroEntityBaseInVision");
                if (!hasKey)
                    GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard key 'NumOfHeroEntityBaseInVision' does not exist");
                else
                {
                    var Ekey = blackboard.GetOrRegisterKey("NumOfHeroEntityBaseInVision");
                    bool getSuccessful = blackboard.TryGetValue<int>(Ekey, out var Evalue);
                    if (!getSuccessful)
                        GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Failed to get value from blackboard key 'NumOfHeroEntityBaseInVision'");
                }
            }
#endif
            var key = blackboard.GetOrRegisterKey("NumOfHeroEntityBaseInVision");
            var hasValue = blackboard.TryGetValue<int>(key, out var value);
            if (!hasValue)
                blackboard.SetValue(key, 0);

            return hasValue ? value : 0;
        }
    }
}
