using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using DG.Tweening;
using Dungeon.Character;
using Dungeon.DungeonGameEntry;
using Dungeon.SkillSystem;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Character
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem, ICombatable
    {
        public bool CheckIsSkillReady(string skillName)
        {
            if (m_SkillDict.TryGetValue(skillName, out var skillData))
            {
                return skillData.IsCooledDown();
            }
            else
            {
                return false;
            }
        }

        public bool TakeSkill(Skill skill)
        {
            skill.FuckMe(this);
            CombatEvents.OnBeAttacked?.Invoke(skill);

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
            m_SkillDict.Clear();

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

        private void UpdateSkillCoolDown()
        {
            foreach (var skillData in m_SkillDict.Values)
            {
                skillData.UpdateCoolDown(Time.deltaTime);
            }
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
                if (Hp <= 0)
                    return;

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

        public StatusBarSetting StatusBarSetting
        {
            get => m_Properties.statusBarSetting;
            set { }
        }

        public CombataEvent CombatEvents
        {
            get;
            set;
        } = new ();

        public void Stun(float duration)
        {
            StunTween?.Kill();
            m_IsStunned = true;
            this.SendMessage("OnStunned", SendMessageOptions.RequireReceiver);

            currentTween?.Kill();
            foreach (var tween in WipTweens)
            {
                tween.Kill();
            }

            SetAnimatorState(ANIMATOR_BOOL_STUN, duration);

            StunTween = DOVirtual.DelayedCall(duration, () =>
            {
                m_IsStunned = false;
                this.SendMessage("OnStunnedEnd", SendMessageOptions.RequireReceiver);
            });
        }

        public void FaintMe(float duration)
        {
            StunTween?.Kill();
            IsFainted = true;
            this.SendMessage("OnStunned",SendMessageOptions.RequireReceiver);

            currentTween?.Kill();
            foreach (var tween in WipTweens)
            {
                tween.Kill();
            }

            SetAnimatorState(ANIMATOR_BOOL_DIE, duration);

            StunTween = DOVirtual.DelayedCall(duration, () =>
            {
                IsFainted = false;
                m_Properties.Submissiveness = 50;
                this.SendMessage("OnStunnedEnd", SendMessageOptions.RequireReceiver);
                SetAnimatorState(ANIMATOR_BOOL_IDLE, 1);
            });
        }

        public bool IsAlive()
        {
            return Hp > 0;
        }
        public void OnKillSomebody(ICombatable killed)
        {
            var exp = DungeonGameEntry.DungeonGameEntry.DungeonResultCalculator.GetDropExpForLevel(killed.BasicInfo.currentLevel);
            GetExperience(exp);

            GameFrameworkLog.Info($"[{name}] 被 {killed.GetGameObject().name} 杀死了,获得了 {exp} 经验");
        }


        private void UpdateCombatorData()
        {
            var keyHp = blackboard.GetOrRegisterKey(AgentBlackBoardEnum.CurrentHP);
            var keyMaxHp = blackboard.GetOrRegisterKey(AgentBlackBoardEnum.HpMax);
            var keyMp = blackboard.GetOrRegisterKey(AgentBlackBoardEnum.CurrentMP);
            var keyMaxMp = blackboard.GetOrRegisterKey(AgentBlackBoardEnum.MpMax);
            var keyAtkSpeed = blackboard.GetOrRegisterKey(AgentBlackBoardEnum.AttackSpeed);

            blackboard.SetValue<int>(keyHp, CombatorData.hp);
            blackboard.SetValue<int>(keyMaxHp, CombatorData.maxHp);
            blackboard.SetValue<int>(keyMp, CombatorData.mp);
            blackboard.SetValue<int>(keyMaxMp, CombatorData.maxMp);
            blackboard.SetValue<float>(keyAtkSpeed, CombatorData.attackSpeed);
        }

        [SerializeField] private CombatorData CombatorData => m_Properties.combatorData;
        private SkillShooter m_SkillShooter;

#if UNITY_EDITOR
        [BoxGroup("状态显示"), ShowInInspector, ReadOnly,LabelText("血量")] private int odin_hp => m_Properties.combatorData.hp;
        [BoxGroup("状态显示"), ShowInInspector, ReadOnly,LabelText("血量最大值")] private int odin_hpMax => m_Properties.combatorData.maxHp;
        [BoxGroup("状态显示"), ShowInInspector, ReadOnly,LabelText("魔法")] private int odin_mp => m_Properties.combatorData.mp;
        [BoxGroup("状态显示"), ShowInInspector, ReadOnly,LabelText("魔法最大值")] private int odin_mpMax => m_Properties.combatorData.maxMp;
        [BoxGroup("状态显示"), ShowInInspector, ReadOnly,LabelText("当前屈服度")] private int odin_submissiveness => m_Properties.Submissiveness;
        [BoxGroup("状态显示"), ShowInInspector, ReadOnly,LabelText("当前等级")] private int odin_level => m_Properties.combatorData.currentLevel;
        
        #endif
    }

}

