using GameFramework;
using UnityEngine;
using UnityEngine.AI;

namespace Dungeon
{
    public class MetropolisHeroMotor : MonoBehaviour
    {
        private NavMeshAgent _agent;
        
        public bool _isMoving;
        [SerializeField] private Vector3 mTargetPos;
        [SerializeField] private Vector3 mTargetRot;
        [SerializeField] private float currentDistance;
        [SerializeField] private float stoppingDistance = 0.1f;
        private readonly float _defaultStoppingDistance = 0.1f;
        private float _currentStoppingDistance;

        public void InitMotor(float speed)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            _agent.speed = speed;
        }
        
        public void MoveTo(Vector3 targetPos, Collider2D targetCollider = null)
        {
            if (_agent == null)
            {
                GameFrameworkLog.Warning("Agent not initialized!");
                return;
            }

            _isMoving = true;
            _agent.isStopped = false;

            Vector3 finalTarget = CalculateFinalTargetPos(targetPos, targetCollider);
            
            mTargetPos = finalTarget;
            mTargetRot = targetPos;
            _agent.stoppingDistance = _currentStoppingDistance;
            _agent.SetDestination(finalTarget);
        }


        /// <summary>
        /// 根据目标位置和碰撞体计算最终可通行的目标位置（建筑边缘）
        /// </summary>
        private Vector3 CalculateFinalTargetPos(Vector3 targetPos, Collider2D targetCollider = null)
        {
            _currentStoppingDistance = _defaultStoppingDistance;
    
            if (targetCollider == null)
            {
                return targetPos;
            }

            Bounds bounds = targetCollider.bounds;
            Vector3 heroPos = transform.position;
            Vector3 center = bounds.center;

            float searchRadius = bounds.extents.magnitude + 0.2f;
            Vector3 closestPoint = center;
            float closestDistance = float.MaxValue;

            const int sampleCount = 16;
            for (int i = 0; i < sampleCount; i++)
            {
                float angle = (360f / sampleCount) * i * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * searchRadius;
                Vector3 samplePos = center + offset;

                if (NavMesh.SamplePosition(samplePos, out var hit, 0.5f, NavMesh.AllAreas))
                {
                    float dist = Vector3.Distance(heroPos, hit.position);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestPoint = hit.position;
                    }
                }
            }

            _currentStoppingDistance = 0.1f;
            return closestPoint;
        }
        
        private void StopMoving()
        {
            _isMoving = false;
            _agent.ResetPath();
        }

        public void PauseMoving()
        {
            _isMoving = false;
            _agent.isStopped = true;
        }

        public bool ReachedTarget(Vector3 currentTargetPos, Collider2D targetCollider = null)
        {
            if (_agent == null)
                return false;

            Vector3 finalTarget = CalculateFinalTargetPos(currentTargetPos, targetCollider);

            if (Vector2.Distance(transform.position, finalTarget) <= _agent.stoppingDistance + 0.01f) // 增加小量容差
            {
                StopMoving();
                return true;
            }

            return false;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            var boxCollider = GetComponent<BoxCollider2D>();
            if(boxCollider != null)
            {
                boxCollider.isTrigger = true;
            }
            
            _isMoving = false;
            mTargetPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isMoving)
            {
                currentDistance = Vector2.Distance(transform.position, mTargetPos);
                
                // 如果到达目标点
                if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
                {
                    // 移动停止，确保面朝目标点
                    Vector2 faceDir = mTargetRot - transform.position;
                    if (Mathf.Abs(faceDir.x) > 0.001f) // 避免方向为0时出现抖动
                    {
                        Vector3 stopScale = transform.localScale;
                        stopScale.x = faceDir.x > 0 ? 1 : -1;
                        transform.localScale = stopScale;
                    }
                    
                    StopMoving();
                }

                // 只有当移动距离足够大时才翻转朝向
                if (currentDistance > 0.1f) // 设置一个最小距离阈值
                {
                    // 翻转朝向
                    Vector3 velocity = _agent.desiredVelocity;
                    if (Mathf.Abs(velocity.x) > 0.1f) // 增加速度阈值
                    {
                        Vector3 scale = transform.localScale;
                        scale.x = Mathf.Lerp(scale.x, velocity.x > 0 ? 1 : -1, Time.deltaTime * 10f);
                        transform.localScale = scale;
                    }
                }
            }
            else
            {
                if (!Mathf.Approximately(transform.localScale.x, 1) && !Mathf.Approximately(transform.localScale.x, -1))
                {
                    transform.localScale = transform.localScale.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_agent != null && _agent.hasPath)
            {
                Gizmos.color = Color.green;
                var corners = _agent.path.corners;
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[i + 1]);
                }
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(mTargetPos, 0.2f);
        }
  
#endif
    }
}
