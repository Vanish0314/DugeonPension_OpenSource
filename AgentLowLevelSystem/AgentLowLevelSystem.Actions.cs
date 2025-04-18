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
            var chest = chestTransf.GetComponent<StandardDungeonTreasureChest>();
            chest.Open(agent.LowLevelSystem);
            DecreaseBlackboardCountOfIVisible<StandardDungeonTreasureChest>();

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

            SetAnimatorState(ANIMATOR_BOOL_INTERACT,timeToDisarmTrap);

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
            if (m_SkillShooter.IsUsingSkill(skillDesc.name))
            {
                return ActionRunState.Continue;
            }

            if (m_SkillShooter.IsUsingSkill())
            {
                return ActionRunState.Stop;
            }

            if (currentTween != null && currentTween.IsActive())
            {
                return m_IsStunned ? ActionRunState.Stop : ActionRunState.Continue;
            }

            if (currentTween != null && !currentTween.IsActive())
            {
                currentTween = null;
                return ActionRunState.Completed;
            }

            if (m_SkillDict.TryGetValue(skillDesc.name, out SkillData data))
            {
                GameFrameworkLog.Info("[AgentLowLevelSystem] UseSkill: " + data.name);
                BumpUseSkillBubbule(data.name);

                SetAnimatorState(ANIMATOR_BOOL_ATTACKING,data.TotalUsageTime);

                var method = SkillDeployMethod.CreateSkillDeployMethod(data, m_SkillShooter, posToUse, dirToUse);
                var skill = new Skill(data, method, this);

                m_SkillShooter.Fire(skill);

                currentTween = m_SkillShooter.CurrentSkillTween;
                SkillTween = currentTween;
                WipTweens.Add(currentTween);

                return ActionRunState.Continue;
            }
            else
            {
                GameFrameworkLog.Error("[UseSkill] Skill not found: " + skillDesc.name + " 勇者没有这个技能");
                return ActionRunState.Stop;
            }
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

        /// <summary>
        /// 释放技能Tween
        /// </summary>
        private Tween SkillTween;

        /// <summary>
        /// animator Tween ,用于控制动画状态
        /// </summary>
        private Tween AnimatorTween;

        /// <summary>
        /// 当前动画状态
        /// </summary>
        private string CurrentAnimatorState = ANIMATOR_BOOL_IDLE;
    }
}
