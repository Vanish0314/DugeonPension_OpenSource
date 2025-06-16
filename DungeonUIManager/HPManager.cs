using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dungeon.Common;
using Dungeon.DungeonEntity;

namespace Dungeon
{
    public class HPManager : MonoBehaviour
    {
        [SerializeField] private MonoPoolItem m_HPBarPrefab;
        [SerializeField] private int m_InitialPoolSize = 20;
        
        private MonoPoolComponent m_HPPool;
        private Dictionary<ICombatable, HPBar> m_ActiveHPBars = new Dictionary<ICombatable, HPBar>();
        
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
            m_HPPool = gameObject.GetOrAddComponent<MonoPoolComponent>();
            m_HPPool.Init("HPBarPool", m_HPBarPrefab, transform, m_InitialPoolSize);
        }

        private void Update()
        {
            // 获取当前所有需要显示血条的战斗单位
            var activeCombatables = FindObjectsOfType<MonoBehaviour>(false)
                .OfType<ICombatable>()
                .Where(c => 
                {
                    var mono = c as MonoBehaviour;
                    return mono != null 
                           && mono.gameObject.activeInHierarchy 
                           && !(mono is DungeonTrapBase);
                }).ToList();

            // 清理已失效的血条（战斗单位已死亡或禁用）
            CleanUpInactiveHPBars(activeCombatables);

            // 为活跃战斗单位创建或更新血条
            foreach (var combatable in activeCombatables)
            {
                if (m_ActiveHPBars.TryGetValue(combatable, out var existingBar))
                {
                    // 已有血条，只需更新位置
                    UpdateHPBarPosition(combatable, existingBar);
                }
                else
                {
                    // 新战斗单位，创建血条
                    CreateHPBar(combatable);
                }
            }
        }
        
        // 清理无效血条（战斗单位已不存在或禁用）
        private void CleanUpInactiveHPBars(List<ICombatable> activeCombatables)
        {
            var toRemove = new List<ICombatable>();
            foreach (var kvp in m_ActiveHPBars)
            {
                if (!activeCombatables.Contains(kvp.Key))
                {
                    kvp.Value.ReturnToPool();
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var key in toRemove)
            {
                m_ActiveHPBars.Remove(key);
            }
        }

        // 创建新血条并绑定到战斗单位
        private void CreateHPBar(ICombatable combatable)
        {
            var hpBar = m_HPPool.GetItem(combatable) as HPBar;
            if (hpBar == null)
            {
                Debug.LogError("获取的血条不是HPBar类型!");
                return;
            }
            
            hpBar.Initialize(combatable);
            UpdateHPBarPosition(combatable, hpBar);
            m_ActiveHPBars.Add(combatable, hpBar);
        }
        
        private void UpdateHPBarPosition(ICombatable combatable, HPBar hpBar)
        {
            if (combatable is MonoBehaviour monoBehaviour)
            {
                Vector3 worldPosition = monoBehaviour.transform.position + combatable.StatusBarSetting.offset;
                hpBar.transform.position = worldPosition;
            }
        }
    }
}