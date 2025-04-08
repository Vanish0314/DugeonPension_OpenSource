using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.AgentLowLevelSystem;
using Dungeon.BlackBoardSystem;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Sensor.Key
{
    public class MonstersInDungeonSensor : LocalWorldSensorBase
    {
        public override void Created()
        {

        }

        public override void Update()
        {

        }

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            var agentBlackBoard = agent.Transform.GetComponent<BlackboardController>();

            if(agentBlackBoard == null)
            {
                GameFrameworkLog.Error("[MonstersInDungeonSensor] Agent does not have a BlackboardController component.");
                return 0;
            }

            agentBlackBoard.GetValue<int>(AgentBlackBoardEnum.MonstersInDungeon,out var value);

            return value;
        }


    }
}
