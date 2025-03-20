using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Goals;
using Dungeon.GOAP.Sensors.Target;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class FinishDungeonAction : GoapActionBase<FinishDungeonAction.Data,FinishDungeonAction.Props>
    {
        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            return ActionRunState.Completed;
        }


        [Serializable]
        public class Props : IActionProperties
        {
            public float Cost { get; set; }
        }
        [Serializable]
        public class Data : IActionData
        {
            public ITarget Target { get ; set; }
        }
    }
}
