// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using CrashKonijn.Agent.Core;
// using CrashKonijn.Goap.Core;
// using CrashKonijn.Goap.Runtime;
// using Dungeon.Character;
// using Dungeon.BlackBoardSystem;
// using GameFramework;
// using UnityEngine;
// using UnityEngine.Rendering;

// namespace Dungeon
// {
//     /// <summary>
//     /// 
//     /// </summary>
//     /// <typeparam name="TVisible">能被看见的类型，比如填入IVisible则拥有看见所有事物的能力。填入幽灵则拥有看见所有幽灵系敌人的能力。填入陷阱则拥有看见所有陷阱的能力。</typeparam>
//     public class HeroAgentVisionSensor<TVisible> : MultiSensorBase where TVisible : IVisible
//     {
//         /// <summary>
//         /// Have to make sure it's leaf type
//         /// </summary>
//         private static readonly Dictionary<Type, Action<HeroAgentVisionSensor<TVisible>>> SensorTypes = new ()
//         {
//             { typeof(IVisible), sensor => sensor.AddLocalWorldSensor<LocalNearByCountOf<IVisible>>(sensor.SenseVisibleInVision) },
//         };
//         public HeroAgentVisionSensor()
//         {
//             var leafTypes = GetLeafTypesImplementingInterface<TVisible>();
//             foreach (var leafType in leafTypes)
//             {
//                 if (SensorTypes.TryGetValue(leafType, out var action))
//                 {
//                     action(this);
//                 }
//                 else
//                 {
//                     GameFrameworkLog.Error("[HeroAgentVisionSensor] No sensor found for type " + leafType.Name + "Please register a sensor for this type in HeroAgentVisionSensor.SensorTypes");
//                 }
//             }
//         }
//         public override void Created()
//         {
//             throw new System.NotImplementedException();
//         }

//         public override void Update()
//         {
//             throw new System.NotImplementedException();
//         }

//         private SenseValue SenseVisibleInVision(IActionReceiver agent, IComponentReference references)
//         {
//             var blackboard = agent.Transform.GetComponent<AgentLowLevelSystem>()?.GetBlackboard();

//             return 0;
//         }
//     }
// }
