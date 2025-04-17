using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MetropolisHeroMotor : MonoBehaviour
    {
        private bool reachTarget = false;
        private bool isMoving;
        private float distance;
        [SerializeField] private Vector3 targetPos;
        private Rigidbody2D m_Rigidbody;
        private float m_Speed;
        private float stoppingDistance = 0.1f; // 停止距离阈值

        public void InitMotor(float speed)
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
            this.m_Speed = speed;
        }

        public void MoveTo(Vector3 targetPos)
        {
            isMoving = true;
            reachTarget = false;
            this.targetPos = targetPos;
        }
        
        public void StopMoving()
        {
            reachTarget = true;
            isMoving = false;
            m_Rigidbody.velocity = Vector2.zero;
        }

        public bool ReachedTarget(Vector3 currentTargetPos)
        {
            return Vector2.Distance(transform.position, currentTargetPos) <= stoppingDistance;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_Rigidbody.gravityScale = 0;
            m_Rigidbody.freezeRotation = true; // 防止旋转
            
            var boxCollider = GetComponent<BoxCollider2D>();
            if(boxCollider != null)
            {
                boxCollider.isTrigger = false;
            }
            
            isMoving = false;
            targetPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (isMoving)
            {
                // 计算移动方向和距离
                Vector2 direction = (targetPos - transform.position).normalized;
                distance = Vector2.Distance(transform.position, targetPos);

                // 如果已经到达目标位置
                if (distance <= stoppingDistance)
                {
                    StopMoving();
                    return;
                }

                // 计算速度（可以添加平滑加速/减速效果）
                Vector2 velocity = direction * m_Speed;
                
                // 应用速度
                m_Rigidbody.velocity = velocity;

                // 让角色面向移动方向
                if (velocity.magnitude > 0.1f)
                {
                    if (velocity.x > 0.1f)
                    {
                        transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    else
                    {
                        transform.localScale = new Vector3(-1f, 1f, 1f);
                    }
                }
            }
        }
    }
}
