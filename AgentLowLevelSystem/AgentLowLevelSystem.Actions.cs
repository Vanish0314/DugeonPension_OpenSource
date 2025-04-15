using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using DG.Tweening;
using Dungeon.DungeonEntity.InteractiveObject;
using Dungeon.SkillSystem;
using GameFramework;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem, ICombatable
    {
        public IActionRunState OpenTreasureChest(Transform chestTransf)
        {
            var chest = chestTransf.GetComponent<DungeonTreasureChest>();
            chest.Open(agent.LowLevelSystem);
            DecreaseBlackboardCountOfIVisible<DungeonTreasureChest>();

            return ActionRunState.Completed;
        }
        public IActionRunState DisarmTrap(Transform trap)
        {
            if (currentTween != null && currentTween.IsActive())
            {
                if (m_IsStunned)
                {
                    return ActionRunState.Stop;
                }

                return ActionRunState.Continue;
            }

            if (currentTween != null && !currentTween.IsActive())
            {
                currentTween = null;
                return ActionRunState.Completed;
            }

            currentTween = DOVirtual.DelayedCall(timeToDisarmTrap, () =>
            {
                Destroy(trap.gameObject);
                GameFrameworkLog.Info("[DisarmTrap] Trap disarmed");
                DecreaseBlackboardCountOfIVisible(trap.gameObject);
            }, false);

            WipTweens.Add(currentTween);

            return ActionRunState.Continue;
        }
        public IActionRunState UseSkill(SkillDesc skillDesc, Vector3 posToUse, Vector3 dirToUse)
        {
            if (StunTween != null && StunTween.IsActive())
            {
                return ActionRunState.Stop;
            }

            if (currentTween != null && currentTween.IsActive())
            {
                return ActionRunState.Continue;
            }

            if (currentTween != null && !currentTween.IsActive())
            {
                currentTween = null;
                return ActionRunState.Completed;
            }

            currentTween = DOVirtual.DelayedCall(timeToDisarmTrap, () =>
            {
#if UNITY_EDITOR
                CheckIfAnimatorHasParameter(skillDesc.name);
#endif

                m_AgentAnimator.SetTrigger(skillDesc.name);

                GameFrameworkLog.Info("[AgentLowLevelSystem] UseSkill: " + skillDesc.name);
                BumpUseSkillBubbule(skillDesc.name);

                var skillData = SkillData.GetFromSkillDesc(skillDesc);
                var method = SkillDeployMethod.CreateSkillDeployMethod(skillData, m_SkillShooter, posToUse, dirToUse);

                var skill = new Skill(skillData, method, this);
            }, false);

            currentTween.onKill += () =>
            {

            };

            WipTweens.Add(currentTween);

            return ActionRunState.Continue;
        }

        [Header("常量设置")]
        [SerializeField] private float timeToDisarmTrap = 1f;
        [SerializeField] private float timeToOpenChest = 1f;

        /*
        1. StunTween执行开始时会kill所有WipTweens,和currentTween
        2. StunTween执行结束时会设置m_IsStunned为false
        3. StunTween执行开始时会设置m_IsStunned为true
        */

        /// <summary>
        /// 正在等待被执行的tween
        /// </summary>
        private List<Tween> WipTweens = new();
        /// <summary>
        /// 正在被打断的Tween
        /// </summary>
        private Tween StunTween;
        /// <summary>
        /// 当前正在执行的tween
        /// </summary>
        private Tween currentTween;
        /// <summary>
        /// 是否处于Stun状态
        /// </summary>
        private bool m_IsStunned;
    }
}
