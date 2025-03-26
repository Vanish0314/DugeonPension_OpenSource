using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Target;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class OpenTreasureChestAction : GoapActionBase<ActionDataWithTransform>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return distance <= 1.2f;
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataWithTransform data, IActionContext context)
        {
            if(data.Target is DungeonTransformTarget target)
            {
                var chest = target.transform.GetComponent<DungeonTreasureChest>();
                #if UNITY_EDITOR
                if(chest.IsOpened())
                    GameFrameworkLog.Error("[OpenTreasureChestAction] 箱子已经点亮!,可能是sensor出错或是没有及时更新状态");
                #endif
                chest.Open(agent.LowLevelSystem);
                var low = agent.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();
                low.DecreaseBlackboardCountOfIVisible<DungeonTreasureChest>();
                return ActionRunState.Completed;
            }
            else
            {
                GameFrameworkLog.Error("[OpenTreasureChestAction] 传入的Target不是DungeonTransformTarger类型,Sensor必须确保传入正确类型!");
                return ActionRunState.Stop;
            }
        }
    }
}
