using System.Collections.Generic;
using System.Linq;
using CrashKonijn.Agent.Core;
using GameFramework;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem, ICombatable
    {
        [SerializeField] private bool enableAgentMoveDebug = true;
        [SerializeField,LabelText("移动速度")] private float m_MoveMaxSpeed = 5f;

        private Stack<Vector3> m_MoveWayPoints = new();

        private void InitMoveSystem() { }
        private void FixedUpdateMoveSystem()
        {
            if(SkillTween != null && SkillTween.IsActive())
            {
                m_MoveWayPoints.Clear();
                m_AgentRigdbody.velocity = Vector2.zero;
                return;
            }

            if (m_MoveWayPoints.Count > 0)
            {
                var dis = Vector3.Magnitude(m_MoveWayPoints.Peek() - transform.position);

                if (dis < 0.1f)
                {
                    m_MoveWayPoints.Pop();
                    return;
                }
                else if (dis > 1.5f)
                {
                    ReCalculatePath();
                }

                var nextMove = m_MoveWayPoints.Peek();
                var dir = (nextMove - transform.position).normalized;
                var v = m_MoveMaxSpeed * dir;
                m_AgentRigdbody.velocity = new Vector2(v.x, v.y);

                SetAnimatorState(ANIMATOR_BOOL_MOVING,1);

                SetSpriteDirection(dir.x > -0.1); // prevent sprite flip when moving diagonally
            }
            else
            {
                m_AgentRigdbody.velocity = Vector2.zero;
            }
        }

        public void MoveTo(Vector3 targetPositionWorldCoord)
        {
            m_MoveWayPoints.Clear();
            m_MoveWayPoints = DungeonGameEntry.DungeonGameEntry.GridSystem.FindPath(transform.position, targetPositionWorldCoord);

            m_MoveWayPoints.Pop();
        }

        public void TargetChanged(ITarget target, bool inRange)
        {
            if (inRange)
            {
                m_MoveWayPoints.Clear();
                SetAnimatorState(ANIMATOR_BOOL_IDLE,99999f);

                return;
            }

            MoveTo(target.Position);
            SetAnimatorState(ANIMATOR_BOOL_MOVING,99999f);

        }

        public void TargetPositionChanged(ITarget target)
        {
            if(Vector3.Distance(transform.position, target.Position) < 0.3f)
            {
                return;
            }

            MoveTo(target.Position);
        }

        public void TargetInRange(ITarget target)
        {
            m_MoveWayPoints.Clear();
            SetAnimatorState(ANIMATOR_BOOL_IDLE,999999f);
        }

        public void TargetLost()
        {

            m_MoveWayPoints.Clear();
            SetAnimatorState(ANIMATOR_BOOL_IDLE,999999f);
        }

        public void TargetNotInRange(ITarget target)
        {
            MoveTo(target.Position);
            SetAnimatorState(ANIMATOR_BOOL_MOVING,999999f);
        }

        private void ReCalculatePath()
        {
            for (int i = 0; i < m_MoveWayPoints.Count - 1; i++)
                m_MoveWayPoints.Pop();

            var targetPosInWorldCoord = m_MoveWayPoints.Pop();

            MoveTo(targetPosInWorldCoord);
        }
        private void FlipSprite()
        {
            m_AgentAnimator.transform.localScale = 
                new Vector3(m_AgentAnimator.transform.localScale.x * -1, m_AgentAnimator.transform.localScale.y, m_AgentAnimator.transform.localScale.z);
        }
        private void SetSpriteDirection(bool right)
        {
            m_AgentAnimator.transform.localScale = new Vector3(right? 1 : -1, m_AgentAnimator.transform.localScale.y, m_AgentAnimator.transform.localScale.z);
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!enableAgentMoveDebug)
                return;

            if (m_MoveWayPoints.Count == 0)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, m_MoveWayPoints.Peek());

            Gizmos.color = Color.gray;
            var lastPos = transform.position;
            foreach (var wayPoint in m_MoveWayPoints)
            {
                Gizmos.DrawLine(lastPos, wayPoint);
                lastPos = wayPoint;
            }

            Gizmos.color = Color.green;
            var labelStyle = new GUIStyle();
            var goal = m_MoveWayPoints.Last();
            Gizmos.DrawCube(goal, Vector3.one * 1f);
            Handles.Label(goal, $"({goal.x},{goal.y})", labelStyle);
        }
#endif
    }

}