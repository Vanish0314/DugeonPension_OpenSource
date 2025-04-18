using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using DG.Tweening;
using Dungeon.Character.Hero;
using Dungeon.SkillSystem;
using GameFramework;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem, ICombatable
    {
        public bool TakeSkill(Skill skill)
        {
            // skill.FuckMe()
            skill.FuckMe(this);

            return Hp < 0;
        }
        public bool IsInSkillRange(SkillDesc skillDesc, float distance)
        {
            if (m_SkillDict.TryGetValue(skillDesc.name, out var skillData))
            {
                return skillData.IsInRange(distance);
            }

            return false;
        }

        private void InitSkillSystem()
        {
            m_SkillShooter = gameObject.GetOrAddComponent<SkillShooter>();

            foreach (var skillData in CurrentOwnedSkills())
            {
                m_SkillDict.Add(skillData.skillName, skillData);
            }

#if UNITY_EDITOR
            if (m_SkillDict.Count == 0)
                GameFrameworkLog.Warning("[AgentLowLevelSystem] 勇者一个技能都没有,你确定吗?:" + name);
#endif
        }

        public GameObject GetGameObject() => gameObject;
        public int Hp
        {
            get
            {
                return CombatorData.hp;
            }
            set
            {
                value = Mathf.Clamp(value, 0, MaxHp);
                CombatorData.hp = value;
                UpdateCombatorData();

                if(value <= 0)
                {
                    OnDied();
                }
            }
                
        }

        public int MaxHp
        {
            get
            {
                return CombatorData.maxHp;
            }
            set
            {
                CombatorData.maxHp = value;
                UpdateCombatorData();
            }
        }
        public int Mp
        {
            get
            {
                return CombatorData.mp;
            }
            set
            {
                value = Mathf.Clamp(value, 0, MaxMp);
                CombatorData.mp = value;
                UpdateCombatorData();
            }
        }
        public int MaxMp
        {
            get
            {
                return CombatorData.maxMp;
            }
            set
            {
                CombatorData.maxMp = value;
                UpdateCombatorData();
            }
        }        
        public float AttackSpeed
        {
            get
            {
                return CombatorData.attackSpeed;
            }
            set 
            {
                CombatorData.attackSpeed = value;
                UpdateCombatorData();
            }
        }
        public CombatorData BasicInfo
        {
            get => CombatorData;
            set
            {
                m_Properties.combatorData = value;
                UpdateCombatorData();
            }
        }



        public void Stun(float duration)
        {
            StunTween?.Kill();
            m_IsStunned = true;
            this.SendMessage("OnStunned");

            currentTween?.Kill();
            foreach (var tween in WipTweens)
            {
                tween.Kill();
            }

            SetAnimatorState(ANIMATOR_BOOL_STUN, duration);

            StunTween = DOVirtual.DelayedCall(duration, () =>
            {
                m_IsStunned = false;
                this.SendMessage("OnStunnedEnd");
            });
        }


        private void UpdateCombatorData()
        {
            var keyHp = m_blackboard.GetOrRegisterKey(AgentBlackBoardEnum.CurrentHP);
            var keyMaxHp = m_blackboard.GetOrRegisterKey(AgentBlackBoardEnum.HpMax);
            var keyMp = m_blackboard.GetOrRegisterKey(AgentBlackBoardEnum.CurrentMP);
            var keyMaxMp = m_blackboard.GetOrRegisterKey(AgentBlackBoardEnum.MpMax);
            var keyAtkSpeed = m_blackboard.GetOrRegisterKey(AgentBlackBoardEnum.AttackSpeed);

            m_blackboard.SetValue<int>(keyHp, CombatorData.hp);
            m_blackboard.SetValue<int>(keyMaxHp, CombatorData.maxHp);
            m_blackboard.SetValue<int>(keyMp, CombatorData.mp);
            m_blackboard.SetValue<int>(keyMaxMp, CombatorData.maxMp);
            m_blackboard.SetValue<float>(keyAtkSpeed, CombatorData.attackSpeed);
        }

        [SerializeField] private CombatorData CombatorData => m_Properties.combatorData;
        private SkillShooter m_SkillShooter;
    }

}

