using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.Trap;
using Dungeon.GOAP.Target;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class DisarmTrapAciton : GoapActionBase<ActionDataWithTransform>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        => distance <= 1.5f;

        public override IActionRunState Perform(IMonoAgent agent, ActionDataWithTransform data, IActionContext context)
        {
            if(data.Target is DungeonTransformTarget target)
            {
                //return agent.LowLevelSystem.DisarmTrap(target,context);//TODO: For all actions , should be done in low level system. For that this is a stateless class.
                return agent.LowLevelSystem.DisarmTrap(target.transform);
            }
            else
            {
                return ActionRunState.StopAndLog("Invalid target");
            }
            
        }
    }
}
