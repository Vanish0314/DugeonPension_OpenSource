using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CrashKonijn.Agent.Core;
using Dungeon.GridSystem;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem
    {
        [SerializeField] private bool enableAgentMoveDebug = true;
        private float m_MoveMaxSpeed = 5f;

        private Stack<Vector3> m_MoveWayPoints = new ();

        private void InitMoveSystem(){}
        private void UpdateMoveSystem()
        {
            if (m_MoveWayPoints.Count > 0)
            {
                if(Vector3.Magnitude(m_MoveWayPoints.Peek() - transform.position) < 0.2f)
                {
                    m_MoveWayPoints.Pop();
                    return;
                }

                var nextMove = m_MoveWayPoints.Peek();
                var dir = (nextMove - transform.position).normalized;
                var v = m_MoveMaxSpeed * dir;
                m_AgentRigdbody.velocity = new Vector2(v.x, v.y);
            }
            else
            {
                m_AgentRigdbody.velocity = Vector2.zero;
            }
        }

        public void MoveTo(Vector3 targetPositionWorldCoord)
        {
            GameFrameworkLog.Info("[AgentLowLevelSystem] AgentLowLevelSystem.MoveTo()");

            m_MoveWayPoints = GridSystem.GridSystem.Instance.FindPath(transform.position,targetPositionWorldCoord);
        }

        public void TargetChanged(ITarget target, bool inRange)
        {
            if (inRange)
            {
                m_MoveWayPoints.Clear();
                return;
            }

            MoveTo(target.Position);
        }

        public void TargetInRange(ITarget target)
        {
            m_MoveWayPoints.Clear();
        }

        public void TargetLost()
        {
            m_MoveWayPoints.Clear();
        }

        public void TargetNotInRange(ITarget target)
        {
            MoveTo(target.Position);
        }
#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if(!enableAgentMoveDebug)
                return;

            if(m_MoveWayPoints.Count == 0)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, m_MoveWayPoints.Peek());

            Gizmos.color = Color.gray;
            var lastPos = transform.position;
            foreach(var wayPoint in m_MoveWayPoints)
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