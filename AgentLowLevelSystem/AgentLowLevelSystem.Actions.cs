using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using DG.Tweening;
using Dungeon.DungeonCalculator;
using Dungeon.DungeonEntity;
using Dungeon.DungeonEntity;
using Dungeon.DungeonEntity;
using Dungeon.SkillSystem;
using GameFramework;
using UnityEngine;

namespace Dungeon.Character
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem, ICombatable
    {
        public IActionRunState OpenTreasureChest(Transform chestTransf)
        {
            var chest = chestTransf.GetComponent<StandardDungeonTreasureChest>();
            chest.Open(this);
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

            SetAnimatorState(ANIMATOR_BOOL_INTERACT, timeToDisarmTrap);
            m_CurrentDisarmingTrap = trap.GetComponent<DungeonTrapBase>();

            currentTween = DOVirtual.DelayedCall(timeToDisarmTrap, () =>
            {
                Destroy(trap.gameObject);
                GameFrameworkLog.Info("[DisarmTrap] Trap disarmed");
                DecreaseBlackboardCountOfIVisible(trap.gameObject);
            }, false);

            currentTween.onComplete += (() =>
            {
                GetExperience(DungeonGameEntry.DungeonGameEntry.DungeonResultCalculator.GetDropExpForLevel(
                    trap.GetComponent<DungeonTrapBase>().trapLevel));
                m_CurrentDisarmingTrap = null;
            });

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
                if(!data.IsCooledDown())
                {
                    GameFrameworkLog.Error($"[AgentLowLevelSystem] 技能冷却中,无法使用,但是goap发出了使用技能的指令.\n 勇者物体:{gameObject.name},技能:{skillDesc.name},勇者名称:{HeroName}");
                    return ActionRunState.Stop;
                }

                GameFrameworkLog.Info("[AgentLowLevelSystem] UseSkill: " + data.name);
                BumpUseSkillBubbule(data.skillName);

                SetAnimatorState(ANIMATOR_BOOL_ATTACKING, data.TotalUsageTime);

                var method = SkillDeployMethod.CreateSkillDeployMethod(data, m_SkillShooter, posToUse, dirToUse);
                var skill = new Skill(data, method, this);

                m_SkillShooter.Fire(skill);
                SetSpriteDirection(skill.skillDeployMethod.SkillDirection.x >= 0);

                currentTween = m_SkillShooter.CurrentSkillTween;
                SkillTween = currentTween;
                WipTweens.Add(currentTween);

                return ActionRunState.Continue;
            }
#if UNITY_EDITOR
            else
            {
                if (m_LevelSkillData.ContainsSkill(skillDesc.name))
                {
                    GameFrameworkLog.Error($"[AgentLowLevelSystem] 勇者还没有升级解锁这个技能,但是goap已经发出使用技能的指令.\n勇者物体:{gameObject.name},技能:{skillDesc.name},勇者名称:{HeroName}");
                }
                else
                {

                    GameFrameworkLog.Error($"[AgentLowLevelSystem] 勇者没有这个技能,但是goap发出使用技能的指令.\n勇者物体:{gameObject.name},技能:{skillDesc.name},勇者名称:{HeroName}");
                }
            }
#endif
            return ActionRunState.Stop;
        }

        public bool IsDisarmingTrap(DungeonTrapBase trap)
        {
            return m_CurrentDisarmingTrap == trap;
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

        private DungeonTrapBase m_CurrentDisarmingTrap = null;
    }
}
