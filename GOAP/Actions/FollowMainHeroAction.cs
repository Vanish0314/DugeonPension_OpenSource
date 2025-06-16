using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class FollowMainHeroAction : GoapActionBase<ActionDataWithTransform>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return (distance <= 1f);
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataWithTransform data, IActionContext context)
        {
            return ActionRunState.Completed;
        }
    }
}
