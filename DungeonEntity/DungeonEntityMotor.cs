using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    [HideInInspector]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DungeonEntityMotor : MonoBehaviour
    {
        private Stack<Vector3> m_Path = new();
        private Rigidbody2D rb;
        private Transform spriteTransform;
        private float speed;

        public void InitMotor(float speed, Transform spriteTransform)
        {
            rb = GetComponent<Rigidbody2D>();
            this.speed = speed;
            this.spriteTransform = spriteTransform;
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
            m_Path.Clear();
            m_Path = DungeonGameEntry.DungeonGameEntry.GridSystem.FindPath(transform.position, targetPosInWorldCoord);
            m_Path.Pop(); // remove the first point, which is the current position
        }
        public void MoveTowards(Vector2 direction)
        {
            rb.velocity = direction.normalized * speed;
        }
        void Start()
        {
#if UNITY_EDITOR
            if (speed == 0)
            {
                GameFrameworkLog.Warning("[DungeonEntityMotor] DungeonEntityMotor speed is 0:" + gameObject.name);
            }
#endif       
            gameObject.layer = LayerMask.NameToLayer("Monster");
            var rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            var box = GetComponent<BoxCollider2D>();
            box.isTrigger = false;
        }

        void Update()
        {
            if (m_Path.Count > 0)
            {
                var dis = Vector3.Magnitude(m_Path.Peek() - transform.position);

                if (dis < 0.1f)
                {
                    m_Path.Pop();

                    if (m_Path.Count == 0)
                    {
                        rb.velocity = Vector2.zero;
                    }

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

                FlipSpriteTowards(dir.x > 0.1); // prevent flip when moving diagonally
            }
        }

        private void ReCalculatePath()
        {
            for (int i = 0; i < m_Path.Count - 1; i++)
                m_Path.Pop();

            var targetPosInWorldCoord = m_Path.Pop();

            MoveTo(targetPosInWorldCoord);
        }

        private void FlipSpriteTowards(bool right)
        {
            spriteTransform.localScale = new Vector3(right ? 1 : -1, 1, 1);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (m_Path.Count > 0)
            {
                Gizmos.color = Color.red;
                var copy = new Stack<Vector3>(m_Path);

                while (copy.Count > 0)
                {
                    var pos = copy.Pop();
                    Gizmos.DrawSphere(pos, 0.2f);
                    if (copy.Count > 0)
                    {
                        var nextPos = copy.Peek();
                        Gizmos.DrawLine(pos, nextPos);
                    }
                }
            }
        }
#endif
    }
}
