using System;
using System.Collections.Generic;
using Dungeon.DungeonGameEntry;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using static UnityGameFramework.Runtime.DebuggerComponent;

namespace Dungeon
{
    public class PlaceManager : MonoBehaviour
    {
        [SerializeField] public InputReader inputReader;

        //[SerializeField] private GridSystem.BuildGridSystem buildGridSystem;
        
        // 字典存储对应 放置物 信息
        private Dictionary<BuildingType, BuildingData> m_BuildingDataDict = new Dictionary<BuildingType, BuildingData>();
        private Dictionary<TrapType, TrapData> m_TrapDataDict = new Dictionary<TrapType, TrapData>();
        private Dictionary<MonsterType, MonsterData> m_MonsterDataDict = new Dictionary<MonsterType, MonsterData>();
        
        // 存储当前拿到的放置物信息
        private BuildingData m_SelectedBuildingData;
        private TrapData m_SelectedTrapData;
        private MonsterData m_SelectedMonsterData;
        
        // 预览物体
        private GameObject m_PreviewInstance;
        private float m_CurrentRotationAngle;
        private Vector3 m_CurrentMouseWorldPos;
        
        // 预览事件
        public event Action<Vector3,GameObject> SelectBuildingPreviewEvent; 

        // 建造完成事件
        public event Action<Vector3,BuildingData> TryPlaceBuildingHere; 
        public event Action<BuildingData> OnBuildingPlaced;
        public event Action<Vector3,TrapData> TryPlaceTrapHere; 
        public event Action<TrapData> OnTrapPlaced;
        public event Action<Vector3,MonsterData> TryPlaceMonsterHere; 
        public event Action<MonsterData> OnMonsterPlaced;
        
        // 枚举方向
        public enum Direction { Up, Right, Down, Left }
        
        // 建造资源缺乏事件 --------------------------------加不加参数再说
        public event Action CannotAfford;
        
        public static PlaceManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            inputReader = Resources.Load<InputReader>("InputReader");
            DungeonGameEntry.DungeonGameEntry.Event.GetComponent<EventComponent>().Subscribe(OnSceneLoadedEventArgs.EventId,OnSceneLoaded);
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            LoadPlacementData();
        }

        private void OnDestroy()
        {
            GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnSceneLoadedEventArgs.EventId,OnSceneLoaded);
        }

        private void Update()
        {
            // UpdatePreview();
        }
        
        #region Data Loading
        private void LoadPlacementData()
        {
            LoadBuildings();
            LoadTraps();
            LoadMonsters();
        }

        private void LoadBuildings()
        {
            m_BuildingDataDict.Clear();
            var buildingDatas = Resources.LoadAll<BuildingData>("BuildingData");
            foreach (var data in buildingDatas)
            {
                m_BuildingDataDict.TryAdd(data.buildingType, data);
            }
        }

        private void LoadTraps()
        {
            m_TrapDataDict.Clear();
            var trapDatas = Resources.LoadAll<TrapData>("TrapData");
            foreach (var data in trapDatas)
            {
                m_TrapDataDict.TryAdd(data.trapType, data);
            }
        }

        private void LoadMonsters()
        {
            m_MonsterDataDict.Clear();
            var monsterDatas = Resources.LoadAll<MonsterData>("MonsterData");
            foreach (var data in monsterDatas)
            {
                m_MonsterDataDict.TryAdd(data.monsterType, data);
            }
        }
        #endregion
        
        #region Public API
        // 通过ID获取BuildingData
        public void SelectBuildingData(BuildingType type)
        {
            if (m_BuildingDataDict.TryGetValue(type, out var data))
            {
                StartPlacement(data);
                return;
            }
            GameFrameworkLog.Error($"BuildingData not found: {type}");
        }

        public void SelectTrapData(TrapType type)
        {
            if (m_TrapDataDict.TryGetValue(type, out var data))
            {
                StartPlacement(data);
                return;
            }
            GameFrameworkLog.Error($"TrapData not found: {type}");
        }

        public void SelectMonsterData(MonsterType monsterType)
        {
            if (m_MonsterDataDict.TryGetValue(monsterType, out var data))
            {
                StartPlacement(data);
                return;
            }
            GameFrameworkLog.Error($"MonsterData not found: {monsterType}");
        }
        #endregion
        
        #region Placement Logic
        private void StartPlacement(IPlaceableData data)
        {
            CancelPlacement();
            
            switch (data)
            {
                case BuildingData building:
                    m_SelectedBuildingData = building;
                    break;
                case TrapData trap:
                    m_SelectedTrapData = trap;
                    m_CurrentRotationAngle = 0f;
                    break;
                case MonsterData monster:
                    m_SelectedMonsterData = monster;
                    break;
            }
            
            SetupPreview(data);
            RegisterInputEvents();
        }

        private void RegisterInputEvents()
        {
            inputReader.SetPlaceActions();
            
            inputReader.OnPlacePositionEvent += OnMouseMoved;
            inputReader.OnTryPlaceEvent += TryPlace;
            inputReader.OnCancelPlaceEvent += CancelPlacement;
            
            // 仅当放置陷阱时订阅旋转事件
            if (m_SelectedTrapData != null)
            {
                inputReader.OnTrapRotateEvent += HandleTrapRotation;
            }
        }

        private void TryPlace()
        {
            if (m_SelectedBuildingData != null)
            {
                TryPlaceBuilding();
            }
            else if (m_SelectedTrapData != null)
            {
                TryPlaceTrap();
            }
            else if (m_SelectedMonsterData != null)
            {
                TryPlaceMonster();
            }
        }
        #endregion
        
        #region Building Placement
        private void TryPlaceBuilding()
        {
            if (m_SelectedBuildingData == null) return;
            
            if (!CanAffordBuilding())
            {
                CannotAfford?.Invoke();
                Debug.Log("资源不足，无法建造");
                return;
            }
            
            TryPlaceBuildingHere?.Invoke(m_CurrentMouseWorldPos, m_SelectedBuildingData);
            
            if (true)//----------------------------判断建造是否完成
            {
                InstantiateBuilding(m_CurrentMouseWorldPos);
                FinalizePlacement();
            }
        }
        
        private void InstantiateBuilding(Vector3 worldPos)
        {
            // 实例化建筑（精确居中）
            GameObject building = Instantiate(
                m_SelectedBuildingData.buildingPrefab,
                worldPos,
                Quaternion.identity
            );
                
            // 触发事件（传递BuildingData而非名字）
            OnBuildingPlaced?.Invoke(m_SelectedBuildingData);
        }
        
        // 检查资源是否足够
        private bool CanAffordBuilding()
        {
            return ResourceModel.Instance.Gold >= m_SelectedBuildingData.cost.gold &&
                   ResourceModel.Instance.Stone >= m_SelectedBuildingData.cost.stone &&
                   ResourceModel.Instance.MagicPower >= m_SelectedBuildingData.cost.magicPower &&
                   ResourceModel.Instance.Material >= m_SelectedBuildingData.cost.material;
        }
        #endregion

        #region Trap Placement
        
        [DungeonGridWindow("TryPlaceTrap")]
        private static void TryPlaceTrapTest()
        {
            PlaceManager.Instance.TryPlaceTrapHere?.Invoke(PlaceManager.Instance.m_CurrentMouseWorldPos, PlaceManager.Instance.m_SelectedTrapData);//--------------------------------------放置陷阱事件
        }
        private void TryPlaceTrap()
        {
            if (!CanAffordTrap())
            {
                CannotAfford?.Invoke();
                return;
            }
            
            TryPlaceTrapHere?.Invoke(m_CurrentMouseWorldPos, m_SelectedTrapData);//--------------------------------------放置陷阱事件
        }

        // 在 PlaceManager 类中添加
        public void TriggerOnTrapPlaced(TrapData trapData)
        {
            OnTrapPlaced?.Invoke(trapData);
            FinalizePlacement();
        }

        private bool CanAffordTrap()
        {
            return ResourceModel.Instance.Material >= m_SelectedTrapData.cost.material;
        }

        
        // 旋转相关
        private Direction GetCurrentDirection()
        {
            // 将角度转换为四方向
            return (Direction)(Mathf.RoundToInt(m_CurrentRotationAngle / 90) % 4);
        }
        
        // 可能有问题
        private void HandleTrapRotation(float angle)
        {
            if (angle != 0)
            {
                m_CurrentRotationAngle += angle > 0 ? 90 : -90;
                m_CurrentRotationAngle %= 360;
                UpdatePreviewRotation();
            }
        }

        private void UpdatePreviewRotation()
        {
            if (m_PreviewInstance != null)
            {
                m_PreviewInstance.transform.rotation = Quaternion.Euler(0, 0, m_CurrentRotationAngle);
            }
        }
        #endregion
        
        #region Monster Placement
        private void TryPlaceMonster()
        {
            if (!CanAffordMonster())
            {
                CannotAfford?.Invoke();
                return;
            }
            
            TryPlaceMonsterHere?.Invoke(m_CurrentMouseWorldPos, m_SelectedMonsterData);//--------------------------------
        }
        
        public void TriggerOnMonsterPlaced(MonsterData monsterData)
        {
            OnMonsterPlaced?.Invoke(monsterData);
            FinalizePlacement();
        }

        private bool CanAffordMonster()
        {
            return ResourceModel.Instance.MagicPower >= m_SelectedMonsterData.cost.magicPower;
        }
        #endregion
        
        #region Common Logic
        
        // 放置结束
        private void FinalizePlacement()
        {
            Destroy(m_PreviewInstance);
            CancelPlacement();
        }

        // 取消放置
        private void CancelPlacement()
        {
            inputReader.OnPlacePositionEvent -= OnMouseMoved;
            inputReader.OnTryPlaceEvent -= TryPlace;
            inputReader.OnCancelPlaceEvent -= CancelPlacement;
            
            inputReader.OnTrapRotateEvent -= HandleTrapRotation;

            inputReader.SetUIActions();
            
            m_SelectedBuildingData = null;
            m_SelectedTrapData = null;
            m_SelectedMonsterData = null;
            m_CurrentRotationAngle = 0f;
            
            if (m_PreviewInstance != null)
            {
                Destroy(m_PreviewInstance);
                m_PreviewInstance = null;
            }
        }
        
        private Vector2Int GetCurrentSize()
        {
            return (m_SelectedBuildingData?.size ?? 
                   m_SelectedTrapData?.size ?? 
                   m_SelectedMonsterData?.size) ?? Vector2Int.one;
        }
        
        private void OnMouseMoved(Vector2 mouseScreenPos)
        {
            // 将屏幕坐标转换为世界坐标
            if (Camera.main != null)
                m_CurrentMouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            m_CurrentMouseWorldPos.z = 0; // 确保z坐标为0
        }

        private void SetupPreview(IPlaceableData data)
        {
            m_PreviewInstance = Instantiate(data.GetPrefab(), new Vector3(-10000, -10000, 0), Quaternion.identity);
            SelectBuildingPreviewEvent?.Invoke(m_CurrentMouseWorldPos,m_PreviewInstance); 
            DisablePreviewComponents();
            // ApplyPreviewEffect();
        }

        private void DisablePreviewComponents()
        {
            // 获取所有Behaviour组件（包括MonoBehaviour和其他可禁用组件）
            Behaviour[] behaviours = m_PreviewInstance.GetComponents<Behaviour>();
    
            foreach (Behaviour behaviour in behaviours)
            {
                behaviour.enabled = false;
            }
        }

        private void OnSceneLoaded(object sender, GameEventArgs e)
        {
            CancelPlacement();
        }
        
        #endregion
        
        #region 暂留

        // private void DisablePreviewComponents()
        // {
        //     if (m_PreviewInstance.GetComponent<ProducingBuildingBase>())
        //         m_PreviewInstance.GetComponent<ProducingBuildingBase>().enabled = false;
        // }
        //
        // private void ApplyPreviewEffect()
        // {
        //     var previewRenderer = m_PreviewInstance.GetComponent<SpriteRenderer>();
        //     if (previewRenderer != null)
        //         previewRenderer.color *= new Color(1, 1, 1, 0.3f); // 半透明效果
        // }

        // private void UpdatePreview()
        // {
        //     if (m_PreviewInstance == null) return;
        //
        //     var gridPos = GetCurrentGridPosition();
        //     Vector3 targetPos = CalculateWorldPosition(gridPos, GetCurrentSize());
        //     m_PreviewInstance.transform.position = targetPos;
        // }
        
        // // 计算建筑左下角原点（网格坐标）--------------后续可能放到grid里
        // private Vector2Int GetBuildingOrigin(Vector2Int centerGridPos, Vector2Int buildingSize)
        // {
        //     int offsetX = (buildingSize.x - 1) / 2;
        //     int offsetY = (buildingSize.y - 1) / 2;
        //
        //     return new Vector2Int(
        //         centerGridPos.x - offsetX,
        //         centerGridPos.y - offsetY
        //     );
        // }
        //
        // // 获取当前鼠标位置网格坐标
        // private Vector2Int GetCurrentGridPosition()
        // {
        //     return buildGridSystem.WorldToGridPosition(m_CurrentMouseWorldPos);
        // }
        //
        // // 计算建筑中心偏移量（单位：格子数）
        // private Vector2 GetBuildingCenterOffset(Vector2Int buildingSize)
        // {
        //     // 奇数列：不需要水平偏移 (1,3,5...)
        //     // 偶数列：需要偏移0.5格 (2,4,6...)
        //     float offsetX = (buildingSize.x % 2 == 0) ? 0.5f : 0f;
        //     float offsetY = (buildingSize.y % 2 == 0) ? 0.5f : 0f;
        //
        //     return new Vector2(offsetX, offsetY);
        // }
        //
        // // 计算出建筑放置的世界坐标
        // private Vector3 CalculateWorldPosition(Vector2Int gridPos, Vector2Int size)
        // {
        //     Vector2 centerOffset = GetBuildingCenterOffset(size);
        //
        //     return buildGridSystem.GridToWorldPosition(gridPos) + 
        //            new Vector3(
        //                centerOffset.x,
        //                centerOffset.y,
        //                0
        //            );
        // }

        #endregion
    }
}

