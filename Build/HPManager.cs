using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dungeon.Common.MonoPool;

namespace Dungeon
{
    public class HPManager : MonoBehaviour
    {
        [SerializeField] private MonoPoolItem m_HPBarPrefab; // 血条预制体
        [SerializeField] private int m_InitialPoolSize = 20;
        
        private MonoPoolComponent m_HPPool;
        private Dictionary<ICombatable, HPBar> m_CombatableHPBars = new Dictionary<ICombatable, HPBar>();
        
        public static HPManager Instance { get; private set; }
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
        
        public void Initialize()
        {
            // 初始化血条对象池
            m_HPPool = gameObject.GetOrAddComponent<MonoPoolComponent>();
            m_HPPool.Init("HPBarPool", m_HPBarPrefab, transform, m_InitialPoolSize);
            
            // 注册现有战斗对象
            RegisterExistingCombatables();
        }
        private void RegisterExistingCombatables()
        {
            var combatables = FindObjectsOfType<MonoBehaviour>(false).OfType<ICombatable>();
            foreach (var combatable in combatables)
            {
                RegisterCombatable(combatable);
            }
        }

        private void Update()
        {
            RegisterExistingCombatables();
            
            // 更新所有血条位置
            foreach (var kvp in m_CombatableHPBars)
            {
                UpdateHPBarPosition(kvp.Key, kvp.Value);
            }
        }

        public void RegisterCombatable(ICombatable combatable)
        {
            if (m_CombatableHPBars.ContainsKey(combatable)) return;
            
            // 从对象池获取血条
            var hpBar = m_HPPool.GetItem(combatable) as HPBar;
            if (hpBar == null)
            {
                Debug.LogError("获取的血条不是HPBar类型!");
                return;
            }
            
            hpBar.Initialize(combatable);
            m_CombatableHPBars.Add(combatable, hpBar);
            UpdateHPBarPosition(combatable, hpBar);
        }

        public void UnregisterCombatable(ICombatable combatable)
        {
            if (m_CombatableHPBars.TryGetValue(combatable, out var hpBar))
            {
                hpBar.ReturnToPool();
                m_CombatableHPBars.Remove(combatable);
            }
        }

        private void UpdateHPBarPosition(ICombatable combatable, HPBar hpBar)
        {
            if (combatable is MonoBehaviour monoBehaviour)
            {
                Vector3 worldPosition = monoBehaviour.transform.position + Vector3.up * 2.5f;
                hpBar.transform.position = worldPosition;
            }
        }
    }
}