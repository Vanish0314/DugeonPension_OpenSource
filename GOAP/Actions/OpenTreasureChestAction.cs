using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity.InteractiveObject;
using Dungeon.GOAP.Target;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class OpenTreasureChestAction : GoapActionBase<ActionDataWithTransform>
    {
        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return distance <= 1.5f;
        }

        public override IActionRunState Perform(IMonoAgent agent, ActionDataWithTransform data, IActionContext context)
        {
            var chest = data.transform.GetComponent<DungeonTreasureChest>();
#if UNITY_EDITOR
            if (chest.IsOpened())
                GameFrameworkLog.Error("[OpenTreasureChestAction] 箱子已经点亮!,可能是sensor出错或是没有及时更新状态");
#endif
            chest.Open(agent.LowLevelSystem);
            var low = agent.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();
            low.DecreaseBlackboardCountOfIVisible<DungeonTreasureChest>();
            return ActionRunState.Completed;

        }
    }
}
