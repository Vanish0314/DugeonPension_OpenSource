using System;
using System.Collections.Generic;
using Dungeon.Common.MonoPool;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;
using static UnityGameFramework.Runtime.DebuggerComponent;

namespace Dungeon
{
    public class PlaceManager : MonoBehaviour
    {
        [SerializeField] public InputReader inputReader;
        
        // BuildingPool
        [SerializeField] private MonoPoolComponent m_CastleBuildingPoolComponent;
        [SerializeField] private MonoPoolItem m_CastleBuildingPoolItemPrefab;
        
        // TrapPool
        [SerializeField] private MonoPoolComponent m_SpikeTrapPoolComponent;
        [SerializeField] private MonoPoolItem m_SpikeTrapPoolItemPrefab;
        
        // MonsterPool
        [SerializeField] private MonoPoolComponent m_SlimeMonsterPoolComponent;
        [SerializeField] private MonoPoolItem m_SlimeMonsterItemPrefab;
        
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
        
        // 枚举方向
        public enum Direction { Up, Right, Down, Left }
        
        public static PlaceManager Instance { get; private set; }
        private void Awake()
        {
            m_CastleBuildingPoolComponent.Init("CastleBuildingPool", m_CastleBuildingPoolItemPrefab,null,16);
            GameFrameworkLog.Debug(m_CastleBuildingPoolComponent.GetItem(null));
            m_SpikeTrapPoolComponent.Init("SpikeTrapPool", m_SpikeTrapPoolItemPrefab,null,16);
            m_SlimeMonsterPoolComponent.Init("SlimeMonsterPool",m_SlimeMonsterItemPrefab,null,16);
            
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            inputReader = Resources.Load<InputReader>("InputReader");
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            LoadPlacementData();
            GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnSceneLoadedEventArgs.EventId,OnSceneLoaded);
            GameEntry.Event.Subscribe(OnTrapPlacedEventArgs.EventId, FinalizePlacement);
            GameEntry.Event.Subscribe(OnMonsterPlacedEventArgs.EventId, FinalizePlacement);
            GameEntry.Event.Subscribe(OnBuildingPlacedEventArgs.EventId, FinalizePlacement);
        }

        private void OnDisable()
        {
            GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnSceneLoadedEventArgs.EventId,OnSceneLoaded);
            GameEntry.Event.Unsubscribe(OnTrapPlacedEventArgs.EventId, FinalizePlacement);
            GameEntry.Event.Unsubscribe(OnMonsterPlacedEventArgs.EventId, FinalizePlacement);
            GameEntry.Event.Unsubscribe(OnBuildingPlacedEventArgs.EventId, FinalizePlacement);
        }
        private void OnDestroy()
        {
           
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
                //inputReader.OnTrapRotateEvent += HandleTrapRotation;
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
                return;

            var buildingItem = m_CastleBuildingPoolComponent.GetItem(null);

            GameEntry.Event.GetComponent<EventComponent>().Fire(this,
                TryPlaceBuildingEventArgs.Create(m_CurrentMouseWorldPos, buildingItem, m_SelectedBuildingData));
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
        
        private void TryPlaceTrap()
        {
            if (!CanAffordTrap())
                return;

            var trapItem = m_SpikeTrapPoolComponent.GetItem(null);

            GameEntry.Event.GetComponent<EventComponent>()
                .Fire(this, TryPlaceTrapEventArgs.Create(m_CurrentMouseWorldPos, trapItem, m_SelectedTrapData));
        }

        private bool CanAffordTrap()
        {
            return ResourceModel.Instance.Material >= m_SelectedTrapData.cost.material;
        }
        
        #endregion
        
        #region Monster Placement
        private void TryPlaceMonster()
        {
            if (!CanAffordMonster())
                return;
            
            var monsterItem = m_SlimeMonsterPoolComponent.GetItem(null);

            GameEntry.Event.GetComponent<EventComponent>().Fire(this,
                TryPlaceMonsterEventArgs.Create(m_CurrentMouseWorldPos, monsterItem, m_SelectedMonsterData));
        }
        
        private bool CanAffordMonster()
        {
            return ResourceModel.Instance.MagicPower >= m_SelectedMonsterData.cost.magicPower;
        }
        #endregion
        
        #region Common Logic
        
        // 放置结束
        private void FinalizePlacement(object sender, GameEventArgs gameEventArgs)
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
            
            //inputReader.OnTrapRotateEvent -= HandleTrapRotation;

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

