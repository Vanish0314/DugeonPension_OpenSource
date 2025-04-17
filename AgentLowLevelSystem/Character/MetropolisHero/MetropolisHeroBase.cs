using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class MetropolisHeroBase : MonoBehaviour, IPointerClickHandler
    {
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
        public int EnergyLevel
        {
            get => basicInfo.EnergyLevel;
            set => basicInfo.EnergyLevel = value;
        }

        public int MaxEnergyLevel
        {
            get => basicInfo.MaxEnergyLevel;
            set => basicInfo.MaxEnergyLevel = value;
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
        private WorkplaceType m_CurrentWorkType;
        private MetropolisBuildingBase m_CurrentWorkBuilding;

        private void Start()
        {
            m_BTHelper = GetComponent<MetropolisHeroBehaviorTreeHelper>();
            m_Animator = GetComponent<Animator>();
            m_Motor = GetComponent<MetropolisHeroMotor>();
            m_Motor.InitMotor(moveSpeed);
            m_BTHelper.Init(this);
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
            m_BTHelper.energyLevel = EnergyLevel;
            m_BTHelper.tiredLevel = TiredLevel;

            m_BTHelper.talkTime = m_TalkingTime;
            m_BTHelper.canTalk = CanTalk();

            m_BTHelper.hasFoodAvailable = HasFoodAvailable();

            m_BTHelper.hasDormitoryAvailable = HasDormitoryAvailable();

            m_BTHelper.hasWorkAvailable = HasWorkplaceAvailable();
        }

        #region PublicAPI

        public virtual void Wander()
        {
            Vector3 randomPos = new Vector3();
            randomPos.x = UnityEngine.Random.Range(-10f, 10f);
            randomPos.y = UnityEngine.Random.Range(-10f, 10f);
            randomPos.z = 0;
            m_Motor.MoveTo(randomPos);
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

        public virtual void Build()
        {
            //建造设施
        }

        // 点击角色时触发
        public void OnPointerClick(PointerEventData eventData)
        {
            // 临时调试代码：打印当前悬停的UI对象
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("当前悬停的UI对象: " + EventSystem.current.currentSelectedGameObject?.name);
            }

            m_BTHelper.isCommandale = true;
            ToggleCommandUI();
        }

        // 显示/隐藏指令UI
        private void ToggleCommandUI()
        {
            commandUIPrefab.gameObject.SetActive(true);
            commandUIPrefab.GetComponent<CommandUIComponent>().Setup(this, uiOffsetY);
        }

        // 接收指令
        public void ReceiveCommand(string command)
        {
            Debug.Log($"接收到指令: {command}");
            m_BTHelper.hasCommand = true;
            m_Command = command;
            commandUIPrefab.gameObject.SetActive(false);
        }

        // 执行具体指令
        public void SetUpCommand()
        {
            switch (m_Command.ToLower())
            {
                case "work":
                    m_BTHelper.commandType = CommandType.Work;
                    break;
                case "eat":
                    m_BTHelper.commandType = CommandType.Eat;
                    break;
                case "sleep":
                    m_BTHelper.commandType = CommandType.Sleep;
                    break;
                default:
                    Debug.LogWarning($"未知指令: {m_Command}");
                    break;
            }

            m_BTHelper.hasCommand = false;
        }

        #endregion

        #region

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
            yield return new WaitForSeconds(2f);
            HungerLevel = Mathf.Clamp(HungerLevel + 10, 0, MaxHungerLevel);
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
            return FindNearestFoodPosition() != Vector3.zero;
        }

        // 返回最近食物的位置，如果没有则返回Vector3.zero
        public Vector3 FindNearestFoodPosition()
        {
            // 获取场景中所有食物对象
            GameObject[] allFoods = GameObject.FindGameObjectsWithTag("Food");

            // 如果没有食物则返回空
            if (allFoods.Length == 0) return Vector3.zero;

            // 使用LINQ找到最近的食物
            GameObject nearestFood = allFoods
                .OrderBy(food => Vector3.Distance(transform.position, food.transform.position))
                .FirstOrDefault();

            return nearestFood != null ? nearestFood.transform.position : Vector3.zero;
        }

        #endregion

        #region Sleep

        IEnumerator Sleeping()
        {
            m_IsSleeping = true;

            while (m_BTHelper.state == MetropolisHeroAIState.Sleeping)
            {
                yield return new WaitForSeconds(1f);
                TiredLevel = Mathf.Clamp(TiredLevel + 1, 0, MaxTiredLevel);
            }

            EndSleep();
        }

        private void EndSleep()
        {
            if (!m_IsSleeping) return;
            m_IsSleeping = false;
            StopCoroutine(m_SleepRoutine);
            m_SleepRoutine = null;
        }

        private const string DORMITORY_TAG = "Dormitory"; // 宿舍标签常量

        private bool HasDormitoryAvailable()
        {
            if (!m_IsSleeping) return false;
            return FindNearestDormitoryPosition() != Vector3.zero;
        }

        // 返回最近宿舍的位置，如果没有则返回Vector3.zero
        public Vector3 FindNearestDormitoryPosition()
        {
            // 获取场景中所有宿舍对象
            GameObject[] allDorms = GameObject.FindGameObjectsWithTag(DORMITORY_TAG);

            // 如果没有宿舍则返回空
            if (allDorms.Length == 0) return Vector3.zero;

            // 使用LINQ找到最近的宿舍
            GameObject nearestDorm = allDorms
                .OrderBy(dorm => Vector3.Distance(transform.position, dorm.transform.position))
                .FirstOrDefault();

            return nearestDorm != null ? nearestDorm.transform.position : Vector3.zero;
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


        private void EndWorking()
        {
            if (!m_IsWorking) return;
            m_IsWorking = false;
            StopCoroutine(m_WorkRoutine);
            m_WorkRoutine = null;
        }

        private const string WORKPLACE_TAG = "WorkPlace"; // 工作建筑标签

        // 判断是否有可用工作建筑
        public bool HasWorkplaceAvailable()
        {
            return FindNearestWorkplace() != null;
        }

        // 查找最近的工作建筑（返回GameObject）
        public GameObject FindNearestWorkplace()
        {
            MetropolisBuildingBase[] allWorkplaces = FindObjectsOfType<MetropolisBuildingBase>();
            if (allWorkplaces.Length == 0) return null;

            return allWorkplaces
                .OrderBy(building => Vector3.Distance(transform.position, building.transform.position))
                .FirstOrDefault()
                ?.gameObject;
        }

        // 获取最近工作建筑的位置
        public Vector3 FindNearestWorkplacePosition()
        {
            GameObject workplace = FindNearestWorkplace();
            m_CurrentWorkBuilding = workplace.GetComponent<MetropolisBuildingBase>();
            return workplace != null ? workplace.transform.position : Vector3.zero;
        }

        public Vector3 FindWorkplacePositionOfType()
        {
            return FindWorkplacePositionOfType(m_CurrentWorkType);
        }

        // 按类型查找工作建筑（优先返回有空位的）
        public Vector3 FindWorkplacePositionOfType(WorkplaceType targetType)
        {
            MetropolisBuildingBase[] allBuildings = FindObjectsOfType<MetropolisBuildingBase>();

            // 优先查找有空位的最近建筑
            MetropolisBuildingBase nearestAvailable = allBuildings
                .Where(b => b.workplaceType == targetType && b.CanAcceptWorker())
                .OrderBy(b => Vector3.Distance(transform.position, b.transform.position))
                .FirstOrDefault();

            m_CurrentWorkBuilding = nearestAvailable;
            
            if (nearestAvailable != null)
                return nearestAvailable.transform.position;

            // 其次返回最近同类型建筑（无论是否有空位）
            MetropolisBuildingBase nearestAny = allBuildings
                .Where(b => b.workplaceType == targetType)
                .OrderBy(b => Vector3.Distance(transform.position, b.transform.position))
                .FirstOrDefault();
            
            m_CurrentWorkBuilding = nearestAny;

            return nearestAny?.transform.position ?? Vector3.zero;
        }

        #endregion

        #endregion

        [SerializeField, LabelText("基本属性")] protected MetropolisHeroData basicInfo;
        [SerializeField, LabelText("移动速度")] protected float moveSpeed = 5f;
        [Header("指令设置")] [SerializeField] private GameObject commandUIPrefab; // 指令UI预制体
        [SerializeField] private float uiOffsetY = 1.5f; // UI显示偏移量
        protected Animator m_Animator;
        protected MetropolisHeroMotor m_Motor;
        protected MetropolisHeroBehaviorTreeHelper m_BTHelper;
    }
}
