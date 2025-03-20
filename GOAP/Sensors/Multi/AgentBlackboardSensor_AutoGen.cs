
// //========================================
// // THIS IS AUTOMATICALLY GENERATED CODE.
// //========================================
// using System.Collections;
// using System.Collections.Generic;
// using CrashKonijn.Agent.Core;
// using CrashKonijn.Goap.Core;
// using CrashKonijn.Goap.Runtime;
// using Dungeon;
// using Dungeon.AgentLowLevelSystem;
// using GameFramework;
// using UnityEngine;

// namespace Dungeon.GOAP.Sensors.Multi
// {
//     public class AgentBlackboardSensor_AutoGen : MultiSensorBase
//     {
//         public override void Created()
//         {
//             throw new System.NotImplementedException();
//         }

//         public override void Update()
//         {
//             throw new System.NotImplementedException();
//         }

//         public AgentBlackboardSensor_AutoGen()
//         {
//         AddLocalWorldSensor<LocalNearByCountOf<IVisible>>(SenseNumInBlackboardIVisible);
//         AddLocalWorldSensor<LocalNearByCountOf<IFuck>>(SenseNumInBlackboardIFuck);
//         }



//         private SenseValue SenseNumInBlackboardIVisible(IActionReceiver agent, IComponentReference references)
//         {
//             var blackboard = references.GetCachedComponent<AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
// #if UNITY_EDITOR
//             {
//                 if (blackboard == null)
//                     GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");

//                 var hasKey = blackboard.CheckIfKeyExists("NumOfIVisibleInVision");
//                 if (!hasKey)
//                     GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard key 'NumOfIVisibleInVision' does not exist");
//                 else
//                 {
//                     var Ekey = blackboard.GetOrRegisterKey("NumOfIVisibleInVision");
//                     bool getSuccessful = blackboard.TryGetValue<int>(Ekey, out var Evalue);
//                     if (!getSuccessful)
//                         GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Failed to get value from blackboard key 'NumOfIVisibleInVision'");
//                 }
//             }
// #endif
//             var key = blackboard.GetOrRegisterKey("NumOfIVisibleInVision");
//             blackboard.TryGetValue<int>(key, out var value);
//             return value;
//         }

//         private SenseValue SenseNumInBlackboardIFuck(IActionReceiver agent, IComponentReference references)
//         {
//             var blackboard = references.GetCachedComponent<AgentLowLevelSystem>()?.GetBlackboard()?.GetBlackboard();
// #if UNITY_EDITOR
//             {
//                 if (blackboard == null)
//                     GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard is null");

//                 var hasKey = blackboard.CheckIfKeyExists("NumOfIFuckInVision");
//                 if (!hasKey)
//                     GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Blackboard key 'NumOfIFuckInVision' does not exist");
//                 else
//                 {
//                     var Ekey = blackboard.GetOrRegisterKey("NumOfIFuckInVision");
//                     bool getSuccessful = blackboard.TryGetValue<int>(Ekey, out var Evalue);
//                     if (!getSuccessful)
//                         GameFrameworkLog.Error("[AgentBlackboardSensor_AutoGen] Failed to get value from blackboard key 'NumOfIFuckInVision'");
//                 }
//             }
// #endif
//             var key = blackboard.GetOrRegisterKey("NumOfIFuckInVision");
//             blackboard.TryGetValue<int>(key, out var value);
//             return value;
//         }
//     }
// }
