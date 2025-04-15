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
        public void TakeSkill(Skill skill)
        {
            // skill.FuckMe()
            skill.FuckMe(this);
        }
        public bool IsInSkillRange(SkillDesc skillDesc, float distance)
        {
            if(m_skillDict.TryGetValue(skillDesc.name, out var skillData))
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
            if(m_skillDict.Count == 0)
                GameFrameworkLog.Warning("[AgentLowLevelSystem] 勇者一个技能都没有,你确定吗?:" + name);
            #endif
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
            StunTween?.Kill();
            m_IsStunned = true;
            this.SendMessage("OnStunned");

            currentTween?.Kill();
            foreach(var tween in WipTweens)
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

        [Header("战斗相关")]
        [SerializeField]private CombatorData m_combatorData;
        [SerializeField]private List<SkillData> m_skills;
        private Dictionary<string, SkillData> m_skillDict = new();
        private SkillShooter m_SkillShooter;
    }

}

