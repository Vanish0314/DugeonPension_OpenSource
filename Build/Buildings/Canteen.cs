using System.Collections;
using System.Collections.Generic;
using Dungeon.Common;
using ParadoxNotion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dungeon
{
    [System.Serializable]
    public class FoodData
    {
        public string foodName;
        public Sprite foodIcon;
        public MonoPoolItem foodPrefab;
        public float duration;
        public int satietyValue;
        public int mentalValue;
    }
    
    public class Canteen : MetropolisBuildingBase
    {
        [Header("输入设置")]
        [SerializeField] private InputReader m_inputReader;
        
        [Header("UI设置")] 
        [SerializeField] private CanteenUI canteenUI;
        [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0); // UI显示偏移
        
        [Header("烹饪相关")]
        private FoodData[] m_CookingSlots = new FoodData[4]; // 改为固定长度数组
        private Queue<FoodData> m_CookingQueue = new Queue<FoodData>();
        private Coroutine m_CookingCoroutine;
        
        [Header("食物生成相关")]
        private Dictionary<FoodData, MonoPoolComponent> m_FoodDataPool = new Dictionary<FoodData, MonoPoolComponent>();
        [SerializeField] private Transform spawnAreaCenter;
        [SerializeField] private Vector2 spawnArea = new Vector2(3, 3);

        [Header("食物配置")] 
        public List<FoodData> foodConfig = new();
        
        
        #region MonoPool

        private void InitMonoPool()
        {
            foreach (var config in foodConfig)
            {
                var foodMonoPool = GetOrCreateMonoPoolComponent(config.foodName + "Pool");
                foodMonoPool.Init(config.foodName, config.foodPrefab, transform, 16);
                m_FoodDataPool.Add(config, foodMonoPool);
            }
        }
        
        private MonoPoolComponent GetOrCreateMonoPoolComponent(string poolName)
        {
            var child = transform.Find(poolName);
            var obj = child != null ? child.gameObject : new GameObject(poolName);
            obj.transform.SetParent(transform);
            return obj.GetOrAddComponent<MonoPoolComponent>();
        }

        #endregion
        
        #region Override

        protected override void OnEnable()
        {
            base.OnEnable();
            InitMonoPool();
            m_inputReader.OnBuildingClickedEvent += OnBuildingClicked;
            m_inputReader.OnNoBuildingClickedEvent += HideUI;
        }

        
        protected override void OnDisable()
        {
            base.OnDisable();
            m_inputReader.OnBuildingClickedEvent -= OnBuildingClicked;
            m_inputReader.OnNoBuildingClickedEvent -= HideUI;
        }
        
        #endregion

        #region UI
        private void OnBuildingClicked(GameObject clickedBuilding)
        {
            if (hasWork)
                return;
            
            if (CurrentState == BuildingState.Completed)
            {
                if (clickedBuilding != gameObject)
                {
                    HideUI();
                    return;
                }

                ShowUI();
            }
        }

        // 显示资源UI
        private void ShowUI()
        {
            if (canteenUI != null)
            {
                if (canteenUI.m_Canteen == null)
                {
                    canteenUI.Initialize(this);
                }
                canteenUI.transform.position = transform.position + uiOffset;
                canteenUI.ShowCookUI();
            }
        }

        private void HideUI()
        {
            if (canteenUI != null)
            {
             canteenUI.HideCookUI();   
            }
        }

        #endregion

        #region 烹饪逻辑
        
        // 按索引添加食物
        public void AddToCookingPlan(FoodData data, int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= 4)
            {
                Debug.LogError($"无效槽位索引: {slotIndex}");
                return;
            }

            if (m_CookingSlots[slotIndex] != null)
            {
                Debug.Log($"槽位 {slotIndex} 已被占用");
                return;
            }

            m_CookingSlots[slotIndex] = data;
            canteenUI.UpdateSlot(slotIndex, data);
        }

        // 按索引移除食物
        public void RemoveFromCookingPlan(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= 4)
            {
                Debug.LogError($"无效槽位索引: {slotIndex}");
                return;
            }

            m_CookingSlots[slotIndex] = null;
            canteenUI.UpdateSlot(slotIndex, null);
        }
        
        // 获取有效烹饪计划
        private List<FoodData> GetValidCookingPlan()
        {
            var plan = new List<FoodData>();
            foreach (var slot in m_CookingSlots)
            {
                if (slot != null) plan.Add(slot);
            }
            return plan;
        }

        public void StartCooking()
        {
            if (m_CookingCoroutine != null) return;
            
            var validPlan = GetValidCookingPlan();
            if (validPlan.Count == 0) return;

            m_CookingQueue = new Queue<FoodData>(validPlan);
            hasWork = true;
            
            canteenUI.ShowProgressUI();
            
            m_CookingCoroutine = StartCoroutine(CookingProcess());
        }
        
        IEnumerator CookingProcess()
        {
            while (m_CookingQueue.Count > 0)
            {
                var food = m_CookingQueue.Dequeue();
                float timer = 0;
                
                // 制作过程
                while (timer < food.duration)
                {
                    timer += Time.deltaTime * currentEfficiency;
                    canteenUI.UpdateProgress(food,timer/food.duration);
                    yield return null;
                }

                GenerateFood(food);
            }

            // 完成所有制作
            hasWork = false;
            StopCooking();
        }

        public void StopCooking()
        {
            // 清空所有烹饪槽
            for (int i = 0; i < m_CookingSlots.Length; i++)
            {
                m_CookingSlots[i] = null;
                canteenUI.UpdateSlot(i, null); // 更新UI槽位显示
            }

            // 重置队列和状态
            m_CookingQueue.Clear();
            canteenUI.HideProgressUI();
            FireAllWorkers();
    
            if (m_CookingCoroutine != null)
            {
                StopCoroutine(m_CookingCoroutine);
                m_CookingCoroutine = null;
            }
        }

        void GenerateFood(FoodData data)
        {
            if (m_FoodDataPool.TryGetValue(data, out MonoPoolComponent monoPool))
            {
                var food = monoPool.GetItem(null) as Food;
                if (food != null)
                {
                    food.Initialize(data.satietyValue, data.mentalValue);
                
                    var spawnPos = spawnAreaCenter.position + new Vector3(
                        Random.Range(-spawnArea.x, spawnArea.x),
                        Random.Range(-spawnArea.y, spawnArea.y),
                        0
                    );
                
                    food.transform.position = spawnPos;
                }
            }
        }

        #endregion

        #region CompeleState

        public override void StartCompletedBehavior()
        {
            base.StartCompletedBehavior();
            hasWork = false;
        }

        public override void UpdateCompletedBehavior()
        {
            base.UpdateCompletedBehavior();

            if (workingHeroes.Count > 0)
            {
                StartCooking();
            }
        }

        public override void Reset()
        {
            base.Reset();
            StopAllCoroutines();
        }

        #endregion
        
    }
}