using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon
{
    public class HeroBlackBoardSensor : MultiSensorBase
    {
        private static readonly Dictionary<IWorldKey,string> blackBoardSenseItems = new(){
            
        };
        public HeroBlackBoardSensor()
        {
            
        }
        private SenseValue SenseBlackBoard(IActionReceiver agent, IComponentReference references)
        {
            return 0;
        }
        public override void Created()
        {

        }

        public override void Update()
        {

        }

    }
}


