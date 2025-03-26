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

    }
}
