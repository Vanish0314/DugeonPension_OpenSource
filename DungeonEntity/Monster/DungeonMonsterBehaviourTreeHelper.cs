using System.Collections;
using System.Collections.Generic;
using Dungeon.SkillSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dungeon.DungeonEntity.Monster
{
    public enum MonsterAIState
    {
        Idle,
        Moving,
        Attacking,
        Dead
    }

    public class DungeonMonsterBehaviourTreeHelper : MonoBehaviour
    {
        [Header("使用说明")]
        [ReadOnly,TextArea] public string animatorParametersHint = "这个用于组件策划配置BehaviourTree 时方便的获取信息和调用方法\n 注意:物体必须有BehaviourTreeOwner组件\n RULES:\n 1.行为树不需要控制动画状态机,只需要设置state即可\nTIPS:\n 1.当currentTarget为空时说明没有看到目标";
        [ReadOnly]public MonsterAIState state;
        [ReadOnly]public bool isAttacking;
        [ReadOnly]public bool isStunned;
        [ReadOnly]public int hp;
        [ReadOnly]public Transform currentTarget;
        [ReadOnly]public List<Transform> targetsInVision;
        [ShowInInspector,ReadOnly]public Vector3? targetLastKnownPosition;

        public bool TargetIsInSkillRange()
        {
            if(currentTarget == null)
            {
                return false;
            }
            else
            {
                return m_Skill.IsInRange(transform.position,currentTarget.position);
            }
        }
        public void MoveTo(Vector3 targetPositionInWorldCoord)
        {
            m_Motor.MoveTo(targetPositionInWorldCoord);
        }
        public void MoveToCurrentTarget()
        {
            MoveTo(currentTarget.position);
        }
        public void Attack()
        {
            state = MonsterAIState.Attacking;
            m_Monster.Attack(currentTarget);
        }
        public void Init(DungeonMonsterBase monster, SkillData skill)
        {
            m_Monster = monster;
            m_Motor = GetComponent<DungeonEntityMotor>();
            m_Skill = skill;
        }
        private DungeonMonsterBase m_Monster;
        private DungeonEntityMotor m_Motor;
        private SkillData m_Skill;
    }
}
