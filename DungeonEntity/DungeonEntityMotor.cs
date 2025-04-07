using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    [HideInInspector]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DungeonEntityMotor : MonoBehaviour
    {
        private Stack<Vector3> m_Path = new();
        private Rigidbody2D rb;
        private float speed;

        public void InitMotor(float speed)
        {
            rb = GetComponent<Rigidbody2D>();
            this.speed = speed;
        }
        public void Stun(float duration)
        {
            rb.velocity = Vector2.zero;
            var temp = m_Path;
            m_Path.Clear();

            DOVirtual.DelayedCall(duration, () =>
            {
                m_Path = temp;
            });
        }
        public void MoveTo(Vector3 targetPosInWorldCoord)
        {
            m_Path = DungeonGameEntry.DungeonGameEntry.GridSystem.FindPath(transform.position, targetPosInWorldCoord);
        }
        public void MoveTowards(Vector2 direction)
        {
            rb.velocity = direction.normalized * speed;
        }
#if UNITY_EDITOR
        void Start()
        {
            if (speed == 0)
            {
                GameFrameworkLog.Warning("[DungeonEntityMotor] DungeonEntityMotor speed is 0:" + gameObject.name);
            }
        }
#endif

        void Update()
        {
            if (m_Path.Count > 0)
            {
                if (m_Path.Count > 0)
                {
                    var dis = Vector3.Magnitude(m_Path.Peek() - transform.position);

                    if (dis < 0.1f)
                    {
                        m_Path.Pop();
                        return;
                    }
                    else if (dis > 1.5f)
                    {
                        ReCalculatePath();
                    }

                    var nextMove = m_Path.Peek();
                    var dir = (nextMove - transform.position).normalized;
                    var v = speed * dir;
                    rb.velocity = new Vector2(v.x, v.y);
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
            }
        }

        private void ReCalculatePath()
        {
            for (int i = 0; i < m_Path.Count - 1; i++)
                m_Path.Pop();

            var targetPosInWorldCoord = m_Path.Pop();

            MoveTo(targetPosInWorldCoord);
        }

    }
}
