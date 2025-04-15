using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using DG.Tweening;
using Dungeon.SkillSystem;
using GameFramework;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem, ICombatable
    {
        private void InitSkillSystem()
        {
            m_SkillShooter = gameObject.GetOrAddComponent<SkillShooter>();
        }

        private void UseSkill(Skill skillToUse)
        {
            m_SkillShooter.Fire(skillToUse);
        }

        public void TakeSkill(Skill skill)
        {
            // skill.FuckMe()
            skill.FuckMe(this);
        }

        public GameObject GetGameObject() => gameObject;
        public int Hp
        {
            get
            {
                m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.CurrentHP, out var value);
                return value;
            }
            set
            {
                var key = m_blackboard.GetOrRegisterKey(AgentBlackBoardEnum.HpMax);
                m_blackboard.SetValue<int>(key, value);

                if (value < 0)
                {
                    GameFrameworkLog.Info("[AgentLowLevelSystem] Hp < 0, Hero Died");
                    DungeonGameEntry.DungeonGameEntry.Event.Fire(this, OnDungeonEndEventArgs.Create());
                }
            }
        }
        public int MaxHp
        {
            get
            {
                m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.HpMax, out var value);
                return value;
            }
            set
            {
                var key = m_blackboard.GetOrRegisterKey(AgentBlackBoardEnum.HpMax);
                m_blackboard.SetValue<int>(key, value);
            }
        }
        public int Mp
        {
            get
            {
                m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.CurrentMP, out var value);
                return value;
            }
            set
            {
                var key = m_blackboard.GetOrRegisterKey(AgentBlackBoardEnum.CurrentMP);
                m_blackboard.SetValue<int>(key, value);
            }
        }
        public int MaxMp
        {
            get
            {
                m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.MpMax, out var value);
                return value;
            }
            set
            {
                var key = m_blackboard.GetOrRegisterKey(AgentBlackBoardEnum.MpMax);
                m_blackboard.SetValue<int>(key, value);
            }
        }
        public float AttackSpeed
        {
            get
            {
                m_BlackboardController.GetValue<float>(AgentBlackBoardEnum.AttackSpeed, out var value);
                return value;
            }
            set
            {
                var key = m_blackboard.GetOrRegisterKey(AgentBlackBoardEnum.AttackSpeed);
                m_blackboard.SetValue<float>(key, value);
            }
        }
        public CombatorData BasicInfo
        {
            get
            {
                UpdateCombatorData();
                return m_combatorData;
            }
            set
            {
                m_combatorData = value;
            }
        }


        public void Stun(float duration)
        {
            m_StunTween?.Kill();

            m_IsStunned = true;
            this.SendMessage("OnStunned");

            SetAnimatorState(ANIMATOR_BOOL_STUN);

            m_StunTween = DOVirtual.DelayedCall(duration, () =>
            {
                m_IsStunned = false;
                this.SendMessage("OnStunnedEnd");
            });
        }


        private void UpdateCombatorData()
        {
            m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.CurrentHP, out var hp);
            m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.HpMax, out var maxHp);
            m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.CurrentMP, out var mp);
            m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.MpMax, out var maxMp);
            m_BlackboardController.GetValue<float>(AgentBlackBoardEnum.AttackSpeed, out var attackSpeed);
            m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.FireResistance, out var fireResistance);
            m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.IceResistance, out var iceResistance);
            m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.HolyResistance, out var holyResistance);
            m_BlackboardController.GetValue<int>(AgentBlackBoardEnum.PosionResistance, out var posionResistance);

            m_combatorData = new CombatorData
            {
                hp = hp,
                maxHp = maxHp,
                mp = mp,
                maxMp = maxMp,
                attackSpeed = attackSpeed,
                fireResistance = fireResistance,
                iceResistance = iceResistance,
                holyResistance = holyResistance,
                posionResistance = posionResistance
            };
        }

        private CombatorData m_combatorData;
        private SkillShooter m_SkillShooter;
    }

}

