using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Dungeon.AgentLowLevelSystem;
using Dungeon.Character.Hero;
using Dungeon.DungeonGameEntry;
using Dungeon.SkillSystem;
using Dungeon.Vision2D;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dungeon.DungeonEntity.Monster
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(DungeonMonsterBehaviourTreeHelper))]
    [RequireComponent(typeof(DungeonEntityMotor), typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public abstract class DungeonMonsterBase : DungeonVisibleEntity, ICombatable
    {
        public int Hp
        {
            get => basicInfo.hp; set
            {
                if (value <= 0)
                    OnDead();
                else
                    basicInfo.hp = value > basicInfo.maxHp ? basicInfo.maxHp : value;
            }
        }
        public int MaxHp { get => basicInfo.maxHp; set => basicInfo.maxHp = value; }
        public int Mp { get => basicInfo.mp; set => basicInfo.mp = value; }
        public int MaxMp { get => basicInfo.maxMp; set => basicInfo.maxMp = value; }
        public float AttackSpeed { get => basicInfo.attackSpeed; set => basicInfo.attackSpeed = value; }
        public CombatorData BasicInfo { get => basicInfo; set => basicInfo = value; }
        public StatusBarSetting StatusBarSetting { get => statusBarSetting; set => statusBarSetting = value; }

        public GameObject GetGameObject() => gameObject;
        public override VisitInformation OnUnvisited(VisitInformation visiter)
        {
            visiter.visited = gameObject;
            return visiter;
        }
        public override VisitInformation OnVisited(VisitInformation visiter)
        {
            if (visibleCheckTarget == null)
            {
                visiter.visited = gameObject;
                return visiter;
            }
            else
            {
                var low = visiter.visiter.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

                if (low == null)
                    return visiter;

                var checkResut = low.DndCheck(visibleCheckTarget);
                if (checkResut.Succeed)
                    visiter.visited = gameObject;

                return visiter;
            }
        }

        public bool TakeSkill(Skill skill)
        {
            OnTakeSkill(skill);
            skill.FuckMe(this);

            return Hp <= 0;
        }

        protected override void OnEnable()
        {
#if UNITY_EDITOR
            if (LayerMask.NameToLayer("Hero") == -1)
                GameFrameworkLog.Error("[DungeonMonsterBase] There is no layer named 'Hero' in the project.");
#endif
            m_HeroLayerMask = LayerMask.GetMask("Hero");
            m_SkillShooter = gameObject.GetOrAddComponent<SkillShooter>();
            m_BtHelper = GetComponent<DungeonMonsterBehaviourTreeHelper>();
            m_Animator = GetComponent<Animator>();
            m_Motor = GetComponent<DungeonEntityMotor>();
            m_Motor.InitMotor(moveSpeed, m_Animator.transform);
            m_BtHelper.Init(this, skill);
            Init();
        }

        private void Update()
        {
            UpdateBehaviourTreeHelper();
            UpdateAnimator();

            OnUpdate();
        }
        private void UpdateBehaviourTreeHelper()
        {
            UpdateVision();
            UpdateValues();
        }
        private void UpdateAnimator()
        {
            switch (m_BtHelper.state)
            {
                case MonsterAIState.Idle:
                    m_Animator.SetBool("isIdle", true);
                    m_Animator.SetBool("isMoving", false);
                    m_Animator.SetBool("isAttacking", false);
                    m_Animator.SetBool("isDead", false);
                    break;
                case MonsterAIState.Moving:
                    m_Animator.SetBool("isIdle", false);
                    m_Animator.SetBool("isMoving", true);
                    m_Animator.SetBool("isAttacking", false);
                    m_Animator.SetBool("isDead", false);
                    break;
                case MonsterAIState.Attacking:
                    m_Animator.SetBool("isIdle", false);
                    m_Animator.SetBool("isMoving", false);
                    m_Animator.SetBool("isAttacking", true);
                    m_Animator.SetBool("isDead", false);
                    break;
                case MonsterAIState.Dead:
                    m_Animator.SetBool("isIdle", false);
                    m_Animator.SetBool("isMoving", false);
                    m_Animator.SetBool("isAttacking", false);
                    m_Animator.SetBool("isDead", true);
                    break;
            }
        }
        private void UpdateVision()
        {
            m_BtHelper.targetsInVision.Clear();

            var results = new Collider2D[6];
            Physics2D.OverlapCircleNonAlloc(transform.position, visionRange, results, m_HeroLayerMask);

            foreach (var result in results)
            {
#if UNITY_EDITOR
                if (result?.GetComponent<HeroEntityBase>() == null && result != null)
                    GameFrameworkLog.Error($"[DungeonMonsterBase] Why the fuck there is a GO in Hero layer but not a HeroEntityBase?\n collider name: {result.name}");
#endif
                if (result == null)
                    break;
                if (!result.GetComponent<HeroEntityBase>().IsAlive())
                    continue;

                m_BtHelper.targetsInVision.Add(result.transform);
            }
        }
        private void UpdateValues()
        {
            m_BtHelper.hp = Hp;
            if (m_BtHelper.CurrentTarget != null)
                m_BtHelper.targetLastKnownPosition = m_BtHelper.CurrentTarget.position;

            if (skillColdingDownTime > 0)
                skillColdingDownTime -= Time.deltaTime;
        }
        private bool SkillIsCoolingDown()
        {
            return skillColdingDownTime > 0;
        }
        protected abstract void OnTakeSkill(Skill skill);
        protected abstract void OnUpdate();
        public virtual void Attack(Transform target)
        {
            if (!skill.IsInRange(transform.position, target.position) || !m_SkillShooter.CouldFire() || SkillIsCoolingDown())
                return;

            m_Motor.Stop();

            m_SkillShooter.Fire(skill, target.position, target.position - transform.position);
            m_BtHelper.isAttacking = true;
            skillColdingDownTime = skill.cooldownTimeInSec;
            SetSpriteDirection((target.position - transform.position).x > 0);

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(skill.preCastTimeInSec));
                await Task.Delay(TimeSpan.FromSeconds(skill.midCastTimeInSec));
                await Task.Delay(TimeSpan.FromSeconds(skill.postCastTimeInSec));

                m_BtHelper.isAttacking = false;
            });
        }
        protected virtual void Init()
        {
            DungeonGameEntry.DungeonGameEntry.DungeonEntityManager.RegisterDungeonEntity(this);
        }
        private void FlipSprite()
        {
            m_Animator.transform.localScale =
                new Vector3(m_Animator.transform.localScale.x * -1, m_Animator.transform.localScale.y, m_Animator.transform.localScale.z);
        }
        private void SetSpriteDirection(bool right)
        {
            m_Animator.transform.localScale = new Vector3(right ? 1 : -1, m_Animator.transform.localScale.y, m_Animator.transform.localScale.z);
        }

#if UNITY_EDITOR
        protected override void OnDestroy()
        {
            GameFrameworkLog.Warning("[Torch] Destroyed 清不要使用Destory而是对象池");
            DungeonGameEntry.DungeonGameEntry.DungeonEntityManager.UnregisterDungeonEntity(this);
        }
#endif
        protected virtual void OnDead()
        {
            Destroy(gameObject);
        }

        public void Stun(float duration)
        {
            m_Motor.Stun(duration);
            m_BtHelper.isStunned = true;

            DOVirtual.DelayedCall(duration, () => { m_BtHelper.isStunned = false; });
        }

        public bool IsAlive()
        {
            return Hp > 0;
        }

        public void OnKillSomebody(ICombatable killed)
        {
            GameFrameworkLog.Info($"[DungeonMonsterBase] {gameObject.name} killed {killed.GetGameObject().name}");
        }

        [Header("使用说明")]
        [ReadOnly, TextArea, LabelText("Animator可使用的变量如下:")] public string animatorParametersHint = "isIdle, isMoving, isAttacking, isDead";

        [Space]
        [Header("基本信息")]
        [SerializeField, LabelText("基本属性")] protected CombatorData basicInfo;
        [SerializeField, LabelText("状态条设置")] protected StatusBarSetting statusBarSetting;
        [Space]
        [SerializeField, LabelText("可见检定"), Tooltip("被看见是否需要过一个检定,不需要置空即可")]
        protected DndCheckTarget visibleCheckTarget;
        [Required, SerializeField, LabelText("使用技能")] protected SkillData skill;
        [SerializeField, LabelText("移动速度")] protected float moveSpeed = 5f;
        [SerializeField, LabelText("视野范围")] protected float visionRange = 10f;
        protected SkillShooter m_SkillShooter;
        protected Animator m_Animator;
        protected DungeonEntityMotor m_Motor;
        protected DungeonMonsterBehaviourTreeHelper m_BtHelper;
        private LayerMask m_HeroLayerMask;
        private float skillColdingDownTime;
    }
}
