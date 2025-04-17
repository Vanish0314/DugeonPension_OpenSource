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
        public void TakeSkill(Skill skill)
        {
            // skill.FuckMe()
            skill.FuckMe(this);
        }
        public bool IsInSkillRange(SkillDesc skillDesc, float distance)
        {
            if (m_skillDict.TryGetValue(skillDesc.name, out var skillData))
            {
                return skillData.IsInRange(distance);
            }

            return false;
        }

        private void InitSkillSystem()
        {
            m_SkillShooter = gameObject.GetOrAddComponent<SkillShooter>();

            foreach (var skillData in m_skills)
            {
                m_skillDict.Add(skillData.skillName, skillData);
            }

#if UNITY_EDITOR
            if (m_skillDict.Count == 0)
                GameFrameworkLog.Warning("[AgentLowLevelSystem] 勇者一个技能都没有,你确定吗?:" + name);
#endif
        }

        public GameObject GetGameObject() => gameObject;
        public int Hp
        {
            get
            {
                return m_combatorData.hp;
            }
            set
            {
                value = Mathf.Clamp(value, 0, MaxHp);
                m_combatorData.hp = value;
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
                return m_combatorData.maxHp;
            }
            set
            {
                m_combatorData.maxHp = value;
                UpdateCombatorData();
            }
        }
        public int Mp
        {
            get
            {
                return m_combatorData.mp;
            }
            set
            {
                value = Mathf.Clamp(value, 0, MaxMp);
                m_combatorData.mp = value;
                UpdateCombatorData();
            }
        }
        public int MaxMp
        {
            get
            {
                return m_combatorData.maxMp;
            }
            set
            {
                m_combatorData.maxMp = value;
                UpdateCombatorData();
            }
        }        
        public float AttackSpeed
        {
            get
            {
                return m_combatorData.attackSpeed;
            }
            set 
            {
                m_combatorData.attackSpeed = value;
                UpdateCombatorData();
            }
        }
        public CombatorData BasicInfo
        {
            get => m_combatorData;
            set
            {
                m_combatorData = value;
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

            m_blackboard.SetValue<int>(keyHp, m_combatorData.hp);
            m_blackboard.SetValue<int>(keyMaxHp, m_combatorData.maxHp);
            m_blackboard.SetValue<int>(keyMp, m_combatorData.mp);
            m_blackboard.SetValue<int>(keyMaxMp, m_combatorData.maxMp);
            m_blackboard.SetValue<float>(keyAtkSpeed, m_combatorData.attackSpeed);
        }


        [Header("战斗相关")]
        [SerializeField] private CombatorData m_combatorData;
        [SerializeField] private List<SkillData> m_skills;
        private Dictionary<string, SkillData> m_skillDict = new();
        private SkillShooter m_SkillShooter;
    }

}

