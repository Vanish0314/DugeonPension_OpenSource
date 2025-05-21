using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeon.Common.MonoPool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Dungeon
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(MetropolisHeroBehaviorTreeHelper))]
    [RequireComponent(typeof(MetropolisHeroMotor), typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public class MetropolisHeroBase : MonoPoolItem
    {
        public string HeroName;
        
        #region BaseVariables

        // 堕落等级
        public int CorruptLevel
        {
            get => basicInfo.CorruptLevel;
            set => basicInfo.CorruptLevel = value;
        }

        public int MaxCorruptLevel
        {
            get => basicInfo.MaxCorruptLevel;
            set => basicInfo.MaxCorruptLevel = value;
        }

        // 饱食度
        public int HungerLevel
        {
            get => basicInfo.HungerLevel;
            set => basicInfo.HungerLevel = value;
        }

        public int MaxHungerLevel
        {
            get => basicInfo.MaxHungerLevel;
            set => basicInfo.MaxHungerLevel = value;
        }

        // 精神力
        public int MentalLevel
        {
            get => basicInfo.MentalLevel;
            set => basicInfo.MentalLevel = value;
        }

        public int MaxMentalLevel
        {
            get => basicInfo.MaxMentalLevel;
            set => basicInfo.MaxMentalLevel = value;
        }

        // 疲劳度
        public int TiredLevel
        {
            get => basicInfo.TiredLevel;
            set => basicInfo.TiredLevel = value;
        }

        public int MaxTiredLevel
        {
            get => basicInfo.MaxTiredLevel;
            set => basicInfo.MaxTiredLevel = value;
        }

        // 工作效率
        public int Efficiency
        {
            get => basicInfo.Efficiency;
            set => basicInfo.Efficiency = value;
        }

        public int MaxEfficiency
        {
            get => basicInfo.MaxEfficiency;
            set => basicInfo.MaxEfficiency = value;
        }

        public MetropolisHeroData BasicInfo
        {
            get => basicInfo;
            set => basicInfo = value;
        }

        #endregion

        private Coroutine m_CurrentTalkRoutine;
        private bool m_IsTalking = false;
        private float m_TalkingTime = 0f;
        private Coroutine m_EatingRoutine;
        private bool m_IsEating = false;
        private Coroutine m_SleepRoutine;
        private bool m_IsSleeping = false;
        private Coroutine m_WorkRoutine;
        private bool m_IsWorking = false;
        private string m_Command;

        private Vector3 m_CurrentWorkPosition;
        private WorkplaceType m_TargetWorkType;
        private MetropolisBuildingBase m_CurrentWorkBuilding;

        protected override void Start()
        {
            m_BTHelper = GetComponent<MetropolisHeroBehaviorTreeHelper>();
            m_Animator = GetComponent<Animator>();
            m_Motor = GetComponent<MetropolisHeroMotor>();
            m_Motor.InitMotor(moveSpeed);
            m_BTHelper.Init(this);
            m_TargetWorkType = WorkplaceType.None;
        }

        protected override void OnEnable()
        {
            m_InputReader.OnHeroClickedEvent += OnHeroClicked;
            m_InputReader.OnNoHeroClickedEvent += HideCommandUI;
        }

        protected override void OnDisable()
        {
            m_InputReader.OnHeroClickedEvent -= OnHeroClicked;
            m_InputReader.OnNoHeroClickedEvent -= HideCommandUI;
        }

        private void Update()
        {
            UpdateBehaviourTreeHelper();
            UpdateAnimator();
        }

        private void UpdateBehaviourTreeHelper()
        {
            UpdateValues();
        }

        private void UpdateAnimator()
        {
            switch (m_BTHelper.state)
            {
                case MetropolisHeroAIState.Idle:
                    m_Animator.SetBool("isIdle", true);
                    m_Animator.SetBool("isMoving", false);
                    m_Animator.SetBool("isSleeping", false);
                    m_Animator.SetBool("isEating", false);
                    m_Animator.SetBool("isInteracting", false);
                    m_Animator.SetBool("isAttacking", false);
                    break;
                case MetropolisHeroAIState.Moving:
                    m_Animator.SetBool("isIdle", false);
                    m_Animator.SetBool("isMoving", true);
                    m_Animator.SetBool("isSleeping", false);
                    m_Animator.SetBool("isEating", false);
                    m_Animator.SetBool("isInteracting", false);
                    m_Animator.SetBool("isAttacking", false);
                    break;
                case MetropolisHeroAIState.Sleeping:
                    m_Animator.SetBool("isIdle", false);
                    m_Animator.SetBool("isMoving", false);
                    m_Animator.SetBool("isSleeping", true);
                    m_Animator.SetBool("isEating", false);
                    m_Animator.SetBool("isInteracting", false);
                    m_Animator.SetBool("isAttacking", false);
                    break;
                case MetropolisHeroAIState.Eating:
                    m_Animator.SetBool("isIdle", false);
                    m_Animator.SetBool("isMoving", false);
                    m_Animator.SetBool("isSleeping", false);
                    m_Animator.SetBool("isEating", true);
                    m_Animator.SetBool("isInteracting", false);
                    m_Animator.SetBool("isAttacking", false);
                    break;
                case MetropolisHeroAIState.Talking:
                    m_Animator.SetBool("isIdle", true);
                    m_Animator.SetBool("isMoving", false);
                    m_Animator.SetBool("isSleeping", false);
                    m_Animator.SetBool("isEating", false);
                    m_Animator.SetBool("isInteracting", false);
                    m_Animator.SetBool("isAttacking", false);
                    break;
                case MetropolisHeroAIState.Working:
                    m_Animator.SetBool("isIdle", false);
                    m_Animator.SetBool("isMoving", false);
                    m_Animator.SetBool("isSleeping", false);
                    m_Animator.SetBool("isEating", false);
                    m_Animator.SetBool("isInteracting", true);
                    m_Animator.SetBool("isAttacking", false);
                    break;
                case MetropolisHeroAIState.Revolting:
                    m_Animator.SetBool("isIdle", false);
                    m_Animator.SetBool("isMoving", false);
                    m_Animator.SetBool("isSleeping", false);
                    m_Animator.SetBool("isEating", false);
                    m_Animator.SetBool("isInteracting", false);
                    m_Animator.SetBool("isAttacking", true);
                    break;
                case MetropolisHeroAIState.Building:
                    m_Animator.SetBool("isIdle", false);
                    m_Animator.SetBool("isMoving", false);
                    m_Animator.SetBool("isSleeping", false);
                    m_Animator.SetBool("isEating", false);
                    m_Animator.SetBool("isInteracting", true);
                    m_Animator.SetBool("isAttacking", false);
                    break;
                case MetropolisHeroAIState.Command:
                    m_Animator.SetBool("isIdle", true);
                    m_Animator.SetBool("isMoving", false);
                    m_Animator.SetBool("isSleeping", false);
                    m_Animator.SetBool("isEating", false);
                    m_Animator.SetBool("isInteracting", false);
                    m_Animator.SetBool("isAttacking", false);
                    break;
            }
        }

        private void UpdateValues()
        {
            m_BTHelper.hungerLevel = HungerLevel;
            m_BTHelper.energyLevel = MentalLevel;
            m_BTHelper.tiredLevel = TiredLevel;

            m_BTHelper.talkTime = m_TalkingTime;
            m_BTHelper.canTalk = CanTalk();

            m_BTHelper.hasFoodAvailable = HasFoodAvailable();

            m_BTHelper.hasWorkAvailable = HasWorkplaceAvailable();
        }

        #region PublicAPI

        public virtual void Wander()
        {
            if(m_IsWandering) return;
            
            // 如果已有协程在运行则先停止
            if (m_WanderCoroutine != null)
            {
                StopCoroutine(m_WanderCoroutine);
            }
            m_WanderCoroutine = StartCoroutine(WanderRoutine());
        }

        public virtual void Eat()
        {
            if (m_IsEating) return;

            if (m_EatingRoutine != null)
            {
                StopCoroutine(m_EatingRoutine);
            }

            m_EatingRoutine = StartCoroutine(EatCoroutine());
        }

        public virtual void Sleep()
        {
            if (m_IsSleeping) return;

            if (m_SleepRoutine != null)
            {
                StopCoroutine(m_SleepRoutine);
            }

            m_SleepRoutine = StartCoroutine(Sleeping());
        }

        public virtual void Talk()
        {
            // 如果已经在闲聊则返回
            if (m_IsTalking) return;

            // 启动计时器协程
            if (m_CurrentTalkRoutine != null)
            {
                StopCoroutine(m_CurrentTalkRoutine);
            }

            m_CurrentTalkRoutine = StartCoroutine(TalkTimer());
        }

        public virtual void Work()
        {
            if(m_CurrentWorkBuilding == null)
                return;
            
            if (m_IsWorking) return;

            if (m_WorkRoutine != null)
            {
                StopCoroutine(m_WorkRoutine);
            }

            m_CurrentWorkBuilding.ForceAssignWorker(this);
            m_WorkRoutine = StartCoroutine(Working());
        }

        public virtual void Revolt()
        {
            //破坏设施
        }

        // 执行具体指令
        public void SetUpCommand()
        {
            switch (m_Command.ToLower())
            {
                case "quarry":
                    m_BTHelper.commandType = MetropolisHeroAIState.Working;
                    m_TargetWorkType = WorkplaceType.Quarry;
                    break;
                case "loggingcamp":
                    m_BTHelper.commandType = MetropolisHeroAIState.Working;
                    m_TargetWorkType = WorkplaceType.LoggingCamp;
                    break;
                case "farm":
                    m_BTHelper.commandType = MetropolisHeroAIState.Working;
                    m_TargetWorkType = WorkplaceType.Farm;
                    break;
                case "eat":
                    m_BTHelper.commandType = MetropolisHeroAIState.Eating;
                    break;
                case "sleep":
                    m_BTHelper.commandType = MetropolisHeroAIState.Sleeping;
                    break;
                default:
                    Debug.LogWarning($"未知指令: {m_Command}");
                    break;
            }

            m_BTHelper.hasCommand = false;
        }

        #endregion

        #region Command

        // 点击角色时触发
        public void OnHeroClicked(GameObject clickedHero)
        {
            if (clickedHero != this.gameObject)
                return;
            m_BTHelper.isCommandale = true;
            ForceEndCurrentState();
            ToggleCommandUI();
        }

        // 显示/隐藏指令UI
        private void ToggleCommandUI()
        {
            commandUIPrefab.gameObject.SetActive(true);
            commandUIPrefab.GetComponent<CommandUIComponent>().Setup(this, uiOffsetY);
        }

        private void HideCommandUI()
        {
            m_BTHelper.isCommandale = false;
            commandUIPrefab.gameObject.SetActive(false);
        }

        // 接收指令
        public void ReceiveCommand(string command)
        {
            Debug.Log($"接收到指令: {command}");
            m_BTHelper.isCommandale = false;
            m_BTHelper.hasCommand = true;
            m_Command = command;
            commandUIPrefab.gameObject.SetActive(false);
        }
        
        private void ForceEndCurrentState()
        {
            switch (m_BTHelper.state)
            {
                case MetropolisHeroAIState.Eating:
                    EndEat();
                    break;
                case MetropolisHeroAIState.Sleeping:
                    EndSleep();
                    break;
                case MetropolisHeroAIState.Working:
                    EndWorking();
                    break;
                case MetropolisHeroAIState.Talking:
                    EndTalking();
                    break;
            }
        }

        #endregion

        #region Actions

        #region Wander

        // 新增字段
        [Header("闲逛设置")]
        [SerializeField] private float wanderInterval = 5f;    // 生成新位置的间隔时间
        [SerializeField] private float wanderRadius = 10f;    // 移动半径范围
        [SerializeField] private float positionCheckDistance = 0.5f; // 位置有效性检测距离
        [SerializeField] private LayerMask obstacleLayers;    // 障碍物层级

        private Coroutine m_WanderCoroutine;
        private Vector3 m_CurrentWanderTarget;
        private bool m_IsWandering;
        
        private IEnumerator WanderRoutine()
        {
            m_IsWandering = true;
            
            while (m_BTHelper.state == MetropolisHeroAIState.Idle)
            {
                float startTime = Time.time;
                
                // 生成新位置直到找到有效位置
                Vector3 newPos;
                do 
                {
                    newPos = GenerateRandomPosition();
                } 
                while (!IsPositionValid(newPos));

                m_CurrentWanderTarget = newPos;
                m_Motor.MoveTo(m_CurrentWanderTarget);

                // 等待指定间隔或提前到达
                yield return new WaitUntil(() => 
                    Vector3.Distance(transform.position, m_CurrentWanderTarget) < positionCheckDistance || 
                    Time.time >= startTime + wanderInterval
                );
                
                yield return new WaitForSeconds(UnityEngine.Random.Range(0, 1));
            }
            
            StopWandering();
        }
        
        public void StopWandering()
        {
            if(!m_IsWandering)
                return;
            
            m_IsWandering = false;
            
            if (m_WanderCoroutine != null)
            {
                StopCoroutine(m_WanderCoroutine);
                m_WanderCoroutine = null;
            }
        }


        // 生成随机位置
        private Vector3 GenerateRandomPosition()
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * wanderRadius;
            return transform.position + new Vector3(randomCircle.x, randomCircle.y, 0);
        }

        // 位置有效性检测
        private bool IsPositionValid(Vector3 position)
        {
            // 检测目标点是否可达
            RaycastHit2D hit = Physics2D.Linecast(
                transform.position, 
                position, 
                obstacleLayers
            );
            return hit.collider == null;
        }

            #endregion

        #region Talk

        IEnumerator TalkTimer()
        {
            m_IsTalking = true;

            float timer = 0;
            while (timer < 10f)
            {
                // 如果状态被强制改变则提前退出
                if (m_BTHelper.state != MetropolisHeroAIState.Talking)
                {
                    EndTalking();
                    break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            // 结束闲聊
            EndTalking();
        }

        private void EndTalking()
        {
            if (!m_IsTalking) return;

            // 重置状态
            m_IsTalking = false;

            StopCoroutine(m_CurrentTalkRoutine);
            // 清理协程引用
            m_CurrentTalkRoutine = null;
        }

        private bool CanTalk()
        {
            if (m_IsTalking) return false;

            return HasOtherCharactersInRange();
        }

        [Header("检测设置")] [SerializeField] private float detectRadius = 5f; // 检测半径
        [SerializeField] private LayerMask characterLayer; // 角色所在层级

        // 检测范围内是否存在其他角色
        public bool HasOtherCharactersInRange()
        {
            return FindNearestCharacter() != null;
        }

        // 获取范围内最近的游戏角色
        // 获取范围内最近的 2D 角色
        public GameObject FindNearestCharacter()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
                transform.position,
                detectRadius,
                characterLayer
            );

            GameObject nearestCharacter = null;
            float minDistance = Mathf.Infinity;

            foreach (var collider in hitColliders)
            {
                // 排除自己
                if (collider.gameObject == gameObject) continue;

                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestCharacter = collider.gameObject;
                }
            }

            return nearestCharacter;
        }

        #endregion

        #region Eat

        IEnumerator EatCoroutine()
        {
            m_IsEating = true;
            var food = FindNearestAvailableFood();
            yield return new WaitForSeconds(2f);
            HungerLevel = Mathf.Clamp(HungerLevel - food.SatietyValue, 0, MaxHungerLevel);
            MentalLevel = Mathf.Clamp(MentalLevel - food.MentalValue, 0, MaxMentalLevel);
            EndEat();
        }

        private void EndEat()
        {
            if (!m_IsEating) return;
            m_IsEating = false;
            StopCoroutine(m_EatingRoutine);
            m_EatingRoutine = null;
        }

        private bool HasFoodAvailable()
        {
            return FindNearestAvailableFood() != null;
        }
        
        public Vector3 FindNearestFoodPosition()
        {
            // 获取所有有效食物对象
            Food nearestFood = FindNearestAvailableFood();
            return nearestFood != null ? nearestFood.transform.position : Vector3.zero;
        }

        private Food FindNearestAvailableFood()
        {
            return FindObjectsOfType<Food>()
                .OrderBy(food => Vector3.Distance(transform.position, food.transform.position))
                .FirstOrDefault();
        }

        #endregion

        #region Sleep

        // 新增字段
        [Header("睡觉设置")]
        private Dormitory m_CurrentDormitory;
        private Dormitory m_TargetDormitory;
        [SerializeField] private int bedEfficiency = 5; // 床上每分钟降低疲劳值
        [SerializeField] private int groundEfficiency = 3; // 地上每分钟降低疲劳值
        
        IEnumerator Sleeping()
        {
            m_IsSleeping = true;

            m_CurrentDormitory = m_TargetDormitory;
            bool inDormitory = m_CurrentDormitory != null;

            // 正式入住登记
            if(inDormitory) 
            {
                m_CurrentDormitory.CheckIn(this);
            }

            int efficiency = inDormitory ? bedEfficiency : groundEfficiency;
    
            // 持续降低疲劳度
            while (TiredLevel > 0 && m_BTHelper.state == MetropolisHeroAIState.Sleeping)
            {
                TiredLevel = Mathf.Clamp(TiredLevel - efficiency, 0, MaxTiredLevel);
                yield return new WaitForSeconds(1);
            }

            EndSleep();
        }

        private void EndSleep()
        {
            if (!m_IsSleeping) return;

            // 起床气判定
            if (TiredLevel > 50 && m_BTHelper.state != MetropolisHeroAIState.Sleeping)
            {
                basicInfo.CorruptLevel = Mathf.Max(0, basicInfo.CorruptLevel - 5);
                Debug.Log($"{name} 被强制唤醒，支配度-5");
            }

            // 退房处理
            if (m_CurrentDormitory != null)
            {
                m_CurrentDormitory.CheckOut(this);
                m_CurrentDormitory = null;
            }

            m_IsSleeping = false;
            if (m_SleepRoutine != null) StopCoroutine(m_SleepRoutine);
            m_SleepRoutine = null;
        }
        
        public (Vector3, Collider2D) TryFindNearestDormitory()
        {
            Dormitory nearestDorm = FindObjectsOfType<Dormitory>()
                .Where(d => d.CanAcceptResident())
                .OrderBy(d => Vector3.Distance(transform.position, d.transform.position))
                .FirstOrDefault();

            if (nearestDorm != null)
            {
                m_TargetDormitory = nearestDorm;
                Collider2D dormCollider = nearestDorm.GetComponent<Collider2D>();
                return (nearestDorm.transform.position, dormCollider);
            }
            
            m_TargetDormitory = null;
            return (transform.position, null);
        }
        
        #endregion

        #region Work

        IEnumerator Working()
        {
            m_IsWorking = true;

            while (m_BTHelper.state == MetropolisHeroAIState.Working)
            {
                yield return new WaitForSeconds(1f);
                TiredLevel = Mathf.Clamp(TiredLevel + 1, 0, MaxTiredLevel);
            }

            EndWorking();
        }

        public void EndWorking()
        {
            if (!m_IsWorking) return;
    
            m_IsWorking = false;
    
            if (m_CurrentWorkBuilding != null)
            {
                m_CurrentWorkBuilding.WorkerLeave(this); // 新增建筑解绑
                m_CurrentWorkBuilding = null;
            }
    
            if (m_WorkRoutine != null)
                StopCoroutine(m_WorkRoutine);
    
            m_WorkRoutine = null;
            m_BTHelper.workComplete = true;
        }
        
        // 判断是否有可用工作建筑
        public bool HasWorkplaceAvailable()
        {
            return FindHighestPriorityWorkplace().Item2 != null;
        }

        public bool RegisterWorkPlace()
        {
            if (m_TargetWorkType != WorkplaceType.None)
            {
                var building = FindNearestAvailableBuildingOfType(m_TargetWorkType);
                if (building != null)
                {
                    m_CurrentWorkBuilding = building;
                    return true;
                }
            }
            
            // 按优先级排序类型
            var sortedTypes = workplacePriorities
                .OrderBy(p => p.priority)
                .Select(p => p.type)
                .ToList();

            foreach (var type in sortedTypes)
            {
                var building = FindNearestAvailableBuildingOfType(type);
                if (building != null)
                {
                    m_CurrentWorkBuilding = building;
                    return true;
                }
            }
            
            return false;
        }
        
        // 返回（位置, 建筑）元组，支持优先级和强制分配
        public (Vector3, Collider2D) FindHighestPriorityWorkplace()
        {
            if (m_TargetWorkType != WorkplaceType.None)
            {
                return FindNearestAvailableWorkplaceOfType();
            }
            
            // 按优先级排序类型
            var sortedTypes = workplacePriorities
                .OrderBy(p => p.priority)
                .Select(p => p.type)
                .ToList();

            foreach (var type in sortedTypes)
            {
                var building = FindNearestAvailableBuildingOfType(type);
                if (building != null)
                {
                    return (building.transform.position, building.GetComponent<Collider2D>());
                }
            }
            
            return (Vector3.zero, null);
        }

        public (Vector3, Collider2D) FindNearestAvailableWorkplaceOfType()
        {
            var building = FindNearestAvailableBuildingOfType(m_TargetWorkType);
            if (building != null)
            {
                return (building.transform.position, building.GetComponent<Collider2D>());
            }
            return (Vector3.zero, null);
        }

        private MetropolisBuildingBase FindNearestAvailableBuildingOfType(WorkplaceType targetType)
        {
            return FindObjectsOfType<MetropolisBuildingBase>()
                .Where(b => b.workplaceType == targetType && b.CanAcceptWorker() && b.hasWork)
                .OrderBy(b => Vector3.Distance(transform.position, b.transform.position))
                .FirstOrDefault();
        }

        #endregion

        #endregion
        
        #region Override

        public override void OnSpawn(object data)
        {
            
        }

        public override void Reset()
        {
            
        }

        #endregion


        [SerializeField] private InputReader m_InputReader;
        [SerializeField, LabelText("基本属性")] protected MetropolisHeroData basicInfo;
        [SerializeField, LabelText("移动速度")] protected float moveSpeed = 5f;
        [Header("指令设置")] [SerializeField] private GameObject commandUIPrefab; // 指令UI预制体
        [SerializeField] private float uiOffsetY = 1.5f; // UI显示偏移量
        [Header("工作设置")]
        [SerializeField] 
        private List<WorkplacePriority> workplacePriorities = new List<WorkplacePriority>
        {
            new WorkplacePriority{ type = WorkplaceType.Construction, priority = 0},
            new WorkplacePriority{ type = WorkplaceType.Quarry, priority = 1 },
            new WorkplacePriority{ type = WorkplaceType.Farm, priority = 2 },
            new WorkplacePriority{ type = WorkplaceType.LoggingCamp, priority = 3 }
        };
        protected Animator m_Animator;
        protected MetropolisHeroMotor m_Motor;
        protected MetropolisHeroBehaviorTreeHelper m_BTHelper;
    }
    
    [Serializable]
    public class WorkplacePriority
    {
        public WorkplaceType type;
        public int priority; // 数值越小优先级越高
    }
}
