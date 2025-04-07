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
        => IsInRangeDefault(distance);

        public override IActionRunState Perform(IMonoAgent agent, ActionDataWithTransform data, IActionContext context)
        {
            if(data.Target is DungeonTransformTarget target)
            {
                //return agent.LowLevelSystem.DisarmTrap(target,context);//TODO: For all actions , should be done in low level system. For that this is a stateless class.

                if(timeDisarming < 2.0f)
                {
                    timeDisarming += Time.deltaTime;
                    agent.LowLevelSystem.BumpActioningBubbule("拆陷阱", timeDisarming / 2.0f);
                    return ActionRunState.Continue;
                }
                else
                {
                    timeDisarming = 0;
                }

                GameObject.Destroy(target.transform.gameObject);
                GameFrameworkLog.Info("[DisarmTrap] Trap disarmed");
                var low = agent.LowLevelSystem;
                low.DecreaseBlackboardCountOfIVisible(target.transform.gameObject);
                return ActionRunState.Completed;
            }
            else
            {
                return ActionRunState.StopAndLog("Invalid target");
            }
            
        }

        [Obsolete("This should be a state less class")]
        private float timeDisarming = 0;
    }
}
