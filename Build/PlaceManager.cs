using System;
using System.Collections.Generic;
using Dungeon.Common.MonoPool;
using Dungeon.GridSystem;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;


namespace Dungeon
{
    public partial class PlaceManager : MonoBehaviour
    {
        public InputReader inputReader;
        [SerializeField] private PreviewHelper previewHelper;

        // 字典存储对应 放置物 信息
        private Dictionary<BuildingType, BuildingData> m_BuildingDataDict = new Dictionary<BuildingType, BuildingData>();
        private Dictionary<TrapType, TrapData> m_TrapDataDict = new Dictionary<TrapType, TrapData>();
        private Dictionary<MonsterType, MonsterData> m_MonsterDataDict = new Dictionary<MonsterType, MonsterData>();


        // 字典对应 对象池 信息
        private Dictionary<BuildingData, MonoPoolComponent> m_BuildingMonoPoolComponentDict = new Dictionary<BuildingData, MonoPoolComponent>();
        private Dictionary<TrapData, MonoPoolComponent> m_TrapMonoPoolComponentDict = new Dictionary<TrapData, MonoPoolComponent>();
        private Dictionary<MonsterData, MonoPoolComponent> m_MonsterMonoPoolComponentDict = new Dictionary<MonsterData, MonoPoolComponent>();

        // 存储当前拿到的放置物信息
        private MonoPoolComponent m_SelectedBuildingMonoPoolComponent;
        private BuildingData m_SelectedBuildingData;
        private MonoPoolComponent m_SelectedTrapMonoPoolComponent;
        private TrapData m_SelectedTrapData;
        private MonoPoolComponent m_SelectedMonsterMonoPoolComponent;
        private MonsterData m_SelectedMonsterData;

        // 存储当前拿到的item
        private MonoPoolItem m_SelectedPoolItem;

        private Vector3 m_CurrentMouseWorldPos;

        private int m_Flag = 0;

        public static PlaceManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            transform.position = Vector3.zero;
        }

        private void OnEnable()
        {
            if (m_Flag == 0)
            {
                m_Flag = 1;
            }
            else if (m_Flag == 1)
            {
                Subscribe();
            }
        }

        public void Initialize()
        {
            Subscribe();
            InitMonoPool();
            LoadPlacementData();
            inputReader = Resources.Load<InputReader>("InputReader");
            inputReader.Subscribe();
        }
        bool m_hasSubscribed = false;
        private void Subscribe()
        {
            if (!m_hasSubscribed)
            {
                if (DungeonGameEntry.DungeonGameEntry.Event == null)
                    return;

                DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnSceneLoadedEventArgs.EventId, OnSceneLoaded);
                DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnTrapPlacedEventArgs.EventId, FinalizePlacement);
                DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnMonsterPlacedEventArgs.EventId, FinalizePlacement);
                DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnBuildingPlacedEventArgs.EventId, FinalizePlacement);
                
                m_hasSubscribed = true;
            }
        }

        private void OnDisable()
        {
            if (m_hasSubscribed)
            {
                DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnSceneLoadedEventArgs.EventId, OnSceneLoaded);
                DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnTrapPlacedEventArgs.EventId, FinalizePlacement);
                DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnMonsterPlacedEventArgs.EventId, FinalizePlacement);
                DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnBuildingPlacedEventArgs.EventId, FinalizePlacement);
            }
        }
        private void OnDestroy()
        {

        }

        private void Update()
        {
            if (m_SelectedBuildingMonoPoolComponent != null)
            {
                // 调用BuildGridSystem的计算方法
                var (previewPos, isValid) = BuildGridSystem.Instance.CalculatePreviewPosition(
                    m_CurrentMouseWorldPos,
                    m_SelectedBuildingData
                );

                previewHelper.UpdatePreview(previewPos, isValid);
            }

            if (m_SelectedTrapMonoPoolComponent != null)
            {
                var previewPos = DungeonGameEntry.DungeonGameEntry.GridSystem.SnapToGridCenter(m_CurrentMouseWorldPos);
                var gridPos = DungeonGameEntry.DungeonGameEntry.GridSystem.WorldToGridPosition(
                    DungeonGameEntry.DungeonGameEntry.GridSystem.SnapToGridCorner(m_CurrentMouseWorldPos));
                var isValid = DungeonGameEntry.DungeonGameEntry.GridSystem.CouldPlaceTrap(gridPos);

                previewHelper.UpdatePreview(previewPos, isValid);
            }

            if (m_SelectedMonsterMonoPoolComponent != null)
            {
                var previewPos = DungeonGameEntry.DungeonGameEntry.GridSystem.SnapToGridCenter(m_CurrentMouseWorldPos);
                var gridPos = DungeonGameEntry.DungeonGameEntry.GridSystem.WorldToGridPosition(
                    DungeonGameEntry.DungeonGameEntry.GridSystem.SnapToGridCorner(m_CurrentMouseWorldPos));
                var isValid = DungeonGameEntry.DungeonGameEntry.GridSystem.CouldPlaceMonster(gridPos);

                previewHelper.UpdatePreview(previewPos, isValid);
            }
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
            m_BuildingMonoPoolComponentDict.Clear();
            var buildingDatas = Resources.LoadAll<BuildingData>("BuildingData");
            foreach (var data in buildingDatas)
            {
                m_BuildingDataDict.TryAdd(data.buildingType, data);

                switch (data.buildingType)
                {
                    case BuildingType.Castle:
                        m_BuildingMonoPoolComponentDict[data] = m_CastlePoolComponent;
                        break;
                    case BuildingType.MonsterLair:
                        m_BuildingMonoPoolComponentDict[data] = m_MonsterLairPoolComponent;
                        break;
                    case BuildingType.ControlCenter:
                        m_BuildingMonoPoolComponentDict[data] = m_ControlCenterPoolComponent;
                        break;
                    case BuildingType.Quarry:
                        m_BuildingMonoPoolComponentDict[data] = m_QuarryPoolComponent;
                        break;
                    case BuildingType.LoggingCamp:
                        m_BuildingMonoPoolComponentDict[data] = m_LoggingCampPoolComponent;
                        break;
                    case BuildingType.FarmLand:
                        m_BuildingMonoPoolComponentDict[data] = m_FarmlandPoolComponent;
                        break;
                    case BuildingType.Canteen:
                        m_BuildingMonoPoolComponentDict[data] = m_CanteenPoolComponent;
                        break;
                    case BuildingType.Dormitory:
                        m_BuildingMonoPoolComponentDict[data] = m_DormitoryPoolComponent;
                        break;
                    default:
                        Debug.LogError($"No pool mapped for {data.buildingType}");
                        break;
                }
            }
            Debug.Log(m_BuildingMonoPoolComponentDict.Count);
        }

        private void LoadTraps()
        {
            m_TrapDataDict.Clear();
            m_TrapMonoPoolComponentDict.Clear();
            var trapDatas = Resources.LoadAll<TrapData>("TrapData");
            foreach (var data in trapDatas)
            {
                m_TrapDataDict.TryAdd(data.trapType, data);

                switch (data.trapType)
                {
                    case TrapType.Spike:
                        m_TrapMonoPoolComponentDict[data] = m_SpikeTrapPoolComponent;
                        break;
                    default:
                        Debug.LogError($"No pool mapped for {data.trapType}");
                        break;
                }
            }
        }

        private void LoadMonsters()
        {
            m_MonsterDataDict.Clear();
            m_MonsterMonoPoolComponentDict.Clear();
            var monsterDatas = Resources.LoadAll<MonsterData>("MonsterData");
            foreach (var data in monsterDatas)
            {
                m_MonsterDataDict.TryAdd(data.monsterType, data);

                switch (data.monsterType)
                {
                    case MonsterType.Slime:
                        m_MonsterMonoPoolComponentDict[data] = m_SlimeMonsterPoolComponent;
                        break;
                    default:
                        Debug.LogError($"No pool mapped for {data.monsterType}");
                        break;
                }
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
                    if (m_BuildingMonoPoolComponentDict.TryGetValue(m_SelectedBuildingData,
                            out MonoPoolComponent buildingMonoPoolComponent))
                    {
                        m_SelectedBuildingMonoPoolComponent = buildingMonoPoolComponent;
                    }
                    previewHelper.Initialize(GetPreviewSprite(m_SelectedBuildingMonoPoolComponent));// 初始化预览
                    break;
                case TrapData trap:
                    m_SelectedTrapData = trap;
                    if (m_TrapMonoPoolComponentDict.TryGetValue(m_SelectedTrapData,
                            out MonoPoolComponent trapMonoPoolComponent))
                    {
                        m_SelectedTrapMonoPoolComponent = trapMonoPoolComponent;
                    }
                    previewHelper.Initialize(GetPreviewSprite(m_SelectedTrapMonoPoolComponent));
                    break;
                case MonsterData monster:
                    m_SelectedMonsterData = monster;
                    if (m_MonsterMonoPoolComponentDict.TryGetValue(m_SelectedMonsterData,
                            out MonoPoolComponent monsterMonoPoolComponent))
                    {
                        m_SelectedMonsterMonoPoolComponent = monsterMonoPoolComponent;
                    }
                    previewHelper.Initialize(GetPreviewSprite(m_SelectedMonsterMonoPoolComponent));
                    break;
            }

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

        // 放置结束
        private void FinalizePlacement(object sender, GameEventArgs gameEventArgs)
        {
            CancelPlacement();
        }

        // 取消放置
        private void CancelPlacement()
        {
            inputReader.OnPlacePositionEvent -= OnMouseMoved;
            inputReader.OnTryPlaceEvent -= TryPlace;
            inputReader.OnCancelPlaceEvent -= CancelPlacement;

            inputReader.SetUIActions();

            previewHelper.HidePreview();
            m_SelectedPoolItem = null;
            m_SelectedBuildingData = null;
            m_SelectedBuildingMonoPoolComponent = null;
            m_SelectedTrapData = null;
            m_SelectedTrapMonoPoolComponent = null;
            m_SelectedMonsterData = null;
            m_SelectedMonsterMonoPoolComponent = null;
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

            if (m_SelectedBuildingMonoPoolComponent != null)
            {
                m_SelectedPoolItem = m_SelectedBuildingMonoPoolComponent.GetItem(null);
            }

            DungeonGameEntry.DungeonGameEntry.Event.FireNow(this,
                TryPlaceBuildingEventArgs.Create(m_CurrentMouseWorldPos, m_SelectedPoolItem, m_SelectedBuildingData));
        }

        // 检查资源是否足够
        private bool CanAffordBuilding()
        {
            return ResourceModel.Instance.HasEnoughResources(m_SelectedBuildingData.cost);
        }
        #endregion

        #region Trap Placement

        private void TryPlaceTrap()
        {
            if (m_SelectedTrapData == null) return;
            
            if (!CanAffordTrap())
                return;

            if (m_SpikeTrapPoolComponent != null)
            {
                m_SelectedPoolItem = m_SpikeTrapPoolComponent.GetItem(null);
            }

            DungeonGameEntry.DungeonGameEntry.Event.Fire(this,
                TryPlaceTrapEventArgs.Create(m_CurrentMouseWorldPos, m_SelectedPoolItem, m_SelectedTrapData));
        }

        private bool CanAffordTrap()
        {
            return ResourceModel.Instance.HasEnoughResources(m_SelectedTrapData.cost);
        }

        #endregion

        #region Monster Placement
        private void TryPlaceMonster()
        {
            if (m_SelectedMonsterData == null) return;
            
            if (!CanAffordMonster())
                return;

            if (m_SelectedMonsterMonoPoolComponent != null)
            {
                m_SelectedPoolItem = m_SelectedMonsterMonoPoolComponent.GetItem(null);
            }

            DungeonGameEntry.DungeonGameEntry.Event.Fire(this,
                TryPlaceMonsterEventArgs.Create(m_CurrentMouseWorldPos, m_SelectedPoolItem, m_SelectedMonsterData));
        }

        private bool CanAffordMonster()
        {
            return ResourceModel.Instance.HasEnoughResources(m_SelectedMonsterData.cost);
        }
        #endregion

        #region Common Logic

        private Vector2Int GetCurrentSize()
        {
            return (m_SelectedBuildingData?.size ??
                   m_SelectedTrapData?.size ??
                   m_SelectedMonsterData?.size) ?? Vector2Int.one;
        }

        private void OnMouseMoved(Vector2 mouseScreenPos)
        {
            Camera[] allCameras = FindObjectsOfType<Camera>(false);
            foreach (var activeCamera in allCameras)
            {
                if (activeCamera.isActiveAndEnabled)
                {
                    // 将屏幕坐标转换为世界坐标
                    m_CurrentMouseWorldPos = activeCamera.ScreenToWorldPoint(mouseScreenPos);
                }
            }
            m_CurrentMouseWorldPos.z = 0; // 确保z坐标为0
        }

        private Sprite GetPreviewSprite(MonoPoolComponent selectedPoolComponent)
        {
            if (m_Pools.TryGetValue(selectedPoolComponent, out MonoPoolItem selectedPoolItem))
            {
                return selectedPoolItem.GetComponent<SpriteRenderer>().sprite;
            }
            return null;
        }

        private void OnSceneLoaded(object sender, GameEventArgs e)
        {
            CancelPlacement();
        }

        #endregion
    }
}

