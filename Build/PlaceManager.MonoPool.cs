using System.Collections;
using System.Collections.Generic;
using Dungeon.Common.MonoPool;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dungeon
{
    public partial class PlaceManager : MonoBehaviour
    {
        // 使用字典直接存储对应关系
        private Dictionary<MonoPoolComponent, MonoPoolItem> m_Pools = 
            new Dictionary<MonoPoolComponent, MonoPoolItem>();
        
        // BuildingPool
        [Header("Buildings")]
        [SerializeField] private MonoPoolComponent m_CastlePoolComponent;
        [SerializeField] private MonoPoolItem m_CastlePoolItem;
        [SerializeField] private MonoPoolComponent m_MonsterLairPoolComponent;
        [SerializeField] private MonoPoolItem m_MonsterLairPoolItem;
        [SerializeField] private MonoPoolComponent m_ControlCenterPoolComponent;
        [SerializeField] private MonoPoolItem m_ControlCenterPoolItem;
        [SerializeField] private MonoPoolComponent m_QuarryPoolComponent;
        [SerializeField] private MonoPoolItem m_QuarryPoolItem;
        [SerializeField] private MonoPoolComponent m_LoggingCampPoolComponent;
        [SerializeField] private MonoPoolItem m_LoggingCampPoolItem;
        [SerializeField] private MonoPoolComponent m_FarmlandPoolComponent;
        [SerializeField] private MonoPoolItem m_FarmlandPoolItem;
        [SerializeField] private MonoPoolComponent m_CanteenPoolComponent;
        [SerializeField] private MonoPoolItem m_CanteenPoolItem;
        [SerializeField] private MonoPoolComponent m_DormitoryPoolComponent;
        [SerializeField] private MonoPoolItem m_DormitoryPoolItem;
        
        // TrapPool
        [Header("Traps")]
        [SerializeField] private MonoPoolComponent m_SpikeTrapPoolComponent; 
        [SerializeField] private MonoPoolItem m_SpikeTrapPoolItem;
        
        // MonsterPool
        [Header("Monsters")]
        [SerializeField] private MonoPoolComponent m_SlimeMonsterPoolComponent;
        [SerializeField] private MonoPoolItem m_SlimeMonsterItem;


        private void InitMonoPool() // 有点烂了
        {
            //buildingpool
            m_CastlePoolComponent = GetOrCreateMonoPoolComponent("CastlePoolComponent");
            m_CastlePoolComponent.Init("Castle", m_CastlePoolItem, m_CastlePoolComponent.transform, 16);
            m_Pools.Add(m_CastlePoolComponent, m_CastlePoolItem);
            
            m_MonsterLairPoolComponent = GetOrCreateMonoPoolComponent("MonsterLairPoolComponent");
            m_MonsterLairPoolComponent.Init("Monster", m_MonsterLairPoolItem, m_MonsterLairPoolComponent.transform, 16);
            m_Pools.Add(m_MonsterLairPoolComponent, m_MonsterLairPoolItem);
            m_ControlCenterPoolComponent = GetOrCreateMonoPoolComponent("ControlCenterPoolComponent");
            m_ControlCenterPoolComponent.Init("ControlCenter", m_ControlCenterPoolItem, m_ControlCenterPoolComponent.transform, 16);
            m_Pools.Add(m_ControlCenterPoolComponent, m_ControlCenterPoolItem);
            m_QuarryPoolComponent = GetOrCreateMonoPoolComponent("QuarryPoolComponent");
            m_QuarryPoolComponent.Init("Quarry", m_QuarryPoolItem, m_QuarryPoolComponent.transform, 16);
            m_Pools.Add(m_QuarryPoolComponent, m_QuarryPoolItem);
            m_LoggingCampPoolComponent = GetOrCreateMonoPoolComponent("LoggingCampPoolComponent");
            m_LoggingCampPoolComponent.Init("Logging", m_LoggingCampPoolItem, m_LoggingCampPoolComponent.transform, 16);
            m_Pools.Add(m_LoggingCampPoolComponent, m_LoggingCampPoolItem);
            m_FarmlandPoolComponent = GetOrCreateMonoPoolComponent("FarmlandPoolComponent");
            m_FarmlandPoolComponent.Init("Farmland", m_FarmlandPoolItem, m_FarmlandPoolComponent.transform, 16);
            m_Pools.Add(m_FarmlandPoolComponent, m_FarmlandPoolItem);
            m_CanteenPoolComponent = GetOrCreateMonoPoolComponent("CanteenPoolComponent");
            m_CanteenPoolComponent.Init("Canteen", m_CanteenPoolItem, m_CanteenPoolComponent.transform, 16);
            m_Pools.Add(m_CanteenPoolComponent, m_CanteenPoolItem);
            m_DormitoryPoolComponent = GetOrCreateMonoPoolComponent("DormitoryPoolComponent");
            m_DormitoryPoolComponent.Init("Dormitory", m_DormitoryPoolItem, m_DormitoryPoolComponent.transform, 16);
            m_Pools.Add(m_DormitoryPoolComponent, m_DormitoryPoolItem);
            
            //trappool
            m_SpikeTrapPoolComponent = GetOrCreateMonoPoolComponent("SpikeTrapPoolComponent");
            m_SpikeTrapPoolComponent.Init("Spike",m_SpikeTrapPoolItem,m_SpikeTrapPoolComponent.transform, 16);
            m_Pools.Add(m_SpikeTrapPoolComponent, m_SpikeTrapPoolItem);
            
            //monsterpool
            m_SlimeMonsterPoolComponent = GetOrCreateMonoPoolComponent("SlimeMonsterPoolComponent");
            m_SlimeMonsterPoolComponent.Init("Slime",m_SlimeMonsterItem,m_SlimeMonsterPoolComponent.transform, 16);
            m_Pools.Add(m_SlimeMonsterPoolComponent, m_SlimeMonsterItem);
        }
        
        private MonoPoolComponent GetOrCreateMonoPoolComponent(string name)
        {
            var child = transform.Find(name);
            var obj = child != null ? child.gameObject : new GameObject(name);
            obj.transform.SetParent(transform);
            return obj.GetOrAddComponent<MonoPoolComponent>();
        }
    }
}
