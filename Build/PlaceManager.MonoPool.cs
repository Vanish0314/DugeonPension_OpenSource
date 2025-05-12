using System.Collections;
using System.Collections.Generic;
using Dungeon.Common.MonoPool;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dungeon
{
    public partial class PlaceManager : MonoBehaviour
    {
        [System.Serializable]
        public struct BuildingConfig
        {
            public string poolName;
            public MonoPoolItem poolItem;
            public int initSize;
            
            public BuildingType buildingType;
        }

        [System.Serializable]
        public struct TrapConfig
        {
            public string poolName;
            public MonoPoolItem poolItem;
            public int initSize;
            
            public TrapType trapType;
        }

        [System.Serializable]
        public struct MonsterConfig
        {
            public string poolName;
            public MonoPoolItem poolItem;
            public int initSize;

            public MonsterType monsterType;
        }
        
        // 使用List存储所有池配置
        [Header("建筑池配置")]
        [SerializeField] private List<BuildingConfig> buildingPools = new List<BuildingConfig>
        {
            new BuildingConfig{ poolName = "Castle", initSize = 16 },
            new BuildingConfig{ poolName = "MonsterLair", initSize = 16 },
            new BuildingConfig{ poolName = "ControlCenter", initSize = 16 },
            new BuildingConfig{ poolName = "Quarry", initSize = 16 },
            new BuildingConfig{ poolName = "LoggingCamp", initSize = 16 },
            new BuildingConfig{ poolName = "Farmland", initSize = 16 },
            new BuildingConfig{ poolName = "Canteen", initSize = 16 },
            new BuildingConfig{ poolName = "Dormitory", initSize = 16 }
        };

        [Header("陷阱池配置")]
        [SerializeField] private List<TrapConfig> trapPools = new List<TrapConfig>
        {
            new TrapConfig{ poolName = "SpikeTrap", initSize = 16 }
        };

        [Header("怪物池配置")]
        [SerializeField] private List<MonsterConfig> monsterPools = new List<MonsterConfig>
        {
            new MonsterConfig{ poolName = "SlimeMonster", initSize = 16 }
        };

        // 合并后的存储结构
        private Dictionary<MonoPoolComponent, MonoPoolItem> m_Pools = 
            new Dictionary<MonoPoolComponent, MonoPoolItem>();

        private void InitMonoPool()
        {
            InitPoolCategory(buildingPools);
            InitPoolCategory(trapPools);
            InitPoolCategory(monsterPools);
        }

        private void InitPoolCategory(List<BuildingConfig> configs)
        {
            foreach (var config in configs)
            {
                var poolComponent = GetOrCreateMonoPoolComponent(config.poolName + "Pool");
                poolComponent.Init(
                    config.poolName,
                    config.poolItem,
                    poolComponent.transform,
                    config.initSize
                );
                m_Pools.Add(poolComponent, config.poolItem);
            }
        }

        private void InitPoolCategory(List<TrapConfig> configs)
        {
            foreach (var config in configs)
            {
                var poolComponent = GetOrCreateMonoPoolComponent(config.poolName + "Pool");
                poolComponent.Init(
                    config.poolName,
                    config.poolItem,
                    poolComponent.transform,
                    config.initSize
                );
                m_Pools.Add(poolComponent, config.poolItem);
            }
        }

        private void InitPoolCategory(List<MonsterConfig> configs)
        {
            foreach (var config in configs)
            {
                var poolComponent = GetOrCreateMonoPoolComponent(config.poolName + "Pool");
                poolComponent.Init(
                    config.poolName,
                    config.poolItem,
                    poolComponent.transform,
                    config.initSize
                );
                m_Pools.Add(poolComponent, config.poolItem);
            }
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
