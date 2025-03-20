using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class LightTorchAction : GoapActionBase<LightTorchAction.Data>
    {
        public override IActionRunState Perform(IMonoAgent agent, LightTorchAction.Data data, IActionContext context)
        {
            return ActionRunState.Completed;
        }

        [Serializable]
        public class Data : IActionData
        {
            public ITarget Target { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        }
    }
}
