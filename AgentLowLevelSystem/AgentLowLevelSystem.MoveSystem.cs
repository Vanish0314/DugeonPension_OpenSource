using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using Dungeon.GridSystem;
using GameFramework;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem
    {
        [SerializeField] private bool enableAgentMoveDebug = true;
        private float m_MoveMaxSpeed = 5f;

        private Stack<Vector3> m_MoveWayPoints = new ();

        private void UpdateMoveSystem()
        {
            if (m_MoveWayPoints.Count > 0)
            {
                if(Vector3.Magnitude(m_MoveWayPoints.Peek() - transform.position) < 0.5f)
                    m_MoveWayPoints.Pop();

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
            GameFrameworkLog.Info("AgentLowLevelSystem.MoveTo()");

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
        }
    }

}