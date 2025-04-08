using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.InteractiveObject;
using Dungeon.GOAP.Target;
using Dungeon.Vision2D;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class LightTorchAction : GoapActionBase<ActionDataWithTransform>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return distance <= 1.2f;
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataWithTransform data, IActionContext context)
        {
            if(data.Target is DungeonTransformTarget target)
            {
                var torch = target.transform.GetComponent<Torch>();
                #if UNITY_EDITOR
                if(torch.IsLightining())
                    GameFrameworkLog.Error("[LightTorchAction] 火把已经点亮!,可能是sensor出错或是没有及时更新状态");
                #endif
                torch.LightUp();
                var low = agent.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();
                low.DecreaseBlackboardCountOfIVisible<Torch>();
                return ActionRunState.Completed;
            }
            else
            {
                GameFrameworkLog.Error("[LightTorchAction] 传入的Target不是DungeonTransformTarger类型,Sensor必须确保传入正确类型!");
                return ActionRunState.Stop;
            }
        }
    }
}
