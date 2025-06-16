using CrashKonijn.Agent.Core;
using Dungeon.DungeonEntity;
using Dungeon.Vision2D;
using GameFramework;
using UnityEngine;

namespace Dungeon.Character
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem,ICombatable
    {
        public void DecreaseBlackboardCountOfIVisible(GameObject goWithIVisibleComponent)
        {
            #if UNITY_EDITOR
            Debug.Assert(goWithIVisibleComponent.GetComponent<IVisible>() != null, "[AgentLowLevelSystem.BlackboardHelper] goWithIVisibleComponent must have IVisible component");
            #endif

            var visible = goWithIVisibleComponent.GetComponent<IVisible>();
            var key = blackboard.GetOrRegisterKey(AgentBlackBoardEnum.GetNameOfIVisibleCountInVision(visible.GetType().Name));
            if (blackboard.TryGetValue(key, out int count))
            {
                blackboard.SetValue(key, count - 1);
            }
            else
            {
                #if UNITY_EDITOR
                GameFrameworkLog.Warning("[AgentLowLevelSystem.BlackboardHelper] 黑板有未注册的条目被修改，初始化可能出现问题");
                #endif
                blackboard.SetValue(key, 0);
            }
        }

        /// <summary>
        /// 允许传入非叶子类型，自动获取运行时具体类型
        /// </summary>
        /// <typeparam name="TValueType"></typeparam>
        public void DecreaseBlackboardCountOfIVisible<TValueType>()
        {
            #if UNITY_EDITOR
            Debug.Assert(typeof(TValueType).IsAssignableFrom(typeof(IVisible)), "TValueType must be a subclass of IVisible");
            #endif

            var key =  blackboard.GetOrRegisterKey(AgentBlackBoardEnum.GetNameOfIVisibleCountInVision(typeof(TValueType).Name));
            if(blackboard.TryGetValue(key, out int count))
            {
                blackboard.SetValue(key, count - 1);
            }
            else
            {
                #if UNITY_EDITOR
                GameFrameworkLog.Warning("[AgentLowLevelSystem.BlackboardHelper] 黑板有未注册的条目被修改，初始化可能出现问题");
                #endif
                blackboard.SetValue(key, 0);
            }
        }
    }
}
