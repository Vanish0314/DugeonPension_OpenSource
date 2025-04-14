using System.Collections.Generic;
using GameFramework;
using GameFramework.ObjectPool;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class TrapObjectManager : MonoBehaviour
    {
        [SerializeField]
        private int m_InstancePoolCapacity = 16; // 每种陷阱类型的对象池初始容量

        private Dictionary<TrapType, IObjectPool<TrapObject>> m_TrapPools = new Dictionary<TrapType, IObjectPool<TrapObject>>();
        private Dictionary<GameObject, TrapData> m_ActiveTraps = new Dictionary<GameObject, TrapData>();

        private void Start()
        {
            // 订阅放置事件
            PlaceManager.Instance.TryPlaceTrapHere += OnTryPlaceTrap;
            PlaceManager.Instance.OnTrapPlaced += OnTrapPlaced;
        }

        private void OnDestroy()
        {
            if (PlaceManager.Instance != null)
            {
                PlaceManager.Instance.TryPlaceTrapHere -= OnTryPlaceTrap;
                PlaceManager.Instance.OnTrapPlaced -= OnTrapPlaced;
            }
        }

        private void OnTryPlaceTrap(Vector3 position, TrapData trapData)
        {
            GameFrameworkLog.Debug("OnTryPlaceTrap called");
            GameObject trapInstance = GetTrap(trapData);
            trapInstance.SetActive(true);
            trapInstance.transform.position = Vector3.zero;
            if (trapInstance != null)
            {
                m_ActiveTraps[trapInstance] = trapData;
                PlaceManager.Instance.TriggerOnTrapPlaced(trapData);
            }
        }

        private void OnTrapPlaced(TrapData trapData)
        {
            // 放置后逻辑
        }

        // 获取或创建指定类型的对象池
        private IObjectPool<TrapObject> GetOrCreatePool(TrapType trapType)
        {
            if (!m_TrapPools.TryGetValue(trapType, out var pool))
            {
                pool = GameEntry.ObjectPool.CreateSingleSpawnObjectPool<TrapObject>(
                    $"TrapPool_{trapType}",
                    m_InstancePoolCapacity,
                    expireTime: 60f,
                    priority: 0
                );
                m_TrapPools.Add(trapType, pool);
            }
            return pool;
        }

        public GameObject GetTrap(TrapData trapData)
        {
            if (trapData == null)
            {
                Debug.LogError("TrapData is null!");
                return null;
            }

            var pool = GetOrCreatePool(trapData.trapType);
            TrapObject trapObject = pool.Spawn();
            
            if (trapObject == null)
            {
                // 创建新实例
                GameObject newTrap = Instantiate(trapData.trapPrefab);
                trapObject = TrapObject.Create(newTrap);
                pool.Register(trapObject, true);
                return newTrap;
            }

            // 从池中获取对象
            GameObject trap = trapObject.Target as GameObject;
            if (trap != null)
            {
                trap.SetActive(true);
                return trap;
            }

            return null;
        }
        
        public void ClearUnusedTraps()
        {
            foreach (var pool in m_TrapPools.Values)
            {
                pool.ReleaseAllUnused();
            }
        }
    }
}