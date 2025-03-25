using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Target;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class DisarmTrapAciton : GoapActionBase<DisarmTrapAciton.Data>
    {
        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            if(data.Target is DungeonTransformTarger target)
            {
                GameObject.Destroy(target.transform.gameObject);
                GameFrameworkLog.Info("[DisarmTrap] Trap disarmed");
                return ActionRunState.Completed;
            }
            else
            {
                return ActionRunState.StopAndLog("Invalid target");
            }
            
        }

        public class Data : IActionData
        {
            public ITarget Target { get ; set; }
        }
    }
}
