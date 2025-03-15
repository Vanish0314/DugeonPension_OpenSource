using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Goals;
using UnityEngine;

namespace Dungeon
{
    public class FinishDungeonAction : GoapActionBase<FinishDungeonAction.Data,FinishDungeonAction.Properties>
    {
        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            throw new NotImplementedException();
        }


        [Serializable]
        public class Properties : IActionProperties
        {
            public float Cost { get; set; }
        }
        [Serializable]
        public class Data : IActionData
        {
            public ITarget Target { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        }
    }
}
