using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dungeon.Common.MonoPool;
using Dungeon.DungeonEntity.Trap;

namespace Dungeon
{
    public class HPManager : MonoBehaviour
    {
        [SerializeField] private MonoPoolItem m_HPBarPrefab;
        [SerializeField] private int m_InitialPoolSize = 20;
        
        private MonoPoolComponent m_HPPool;
        private List<HPBar> m_ActiveHPBars = new List<HPBar>();
        
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
            //  回收所有当前血条
            ReturnAllHPBars();
            
            //  为所有活跃的战斗对象创建新血条
            CreateHPBarsForActiveCombatables();
        }

        private void ReturnAllHPBars()
        {
            foreach (var hpBar in m_ActiveHPBars)
            {
                hpBar.ReturnToPool();
            }
            m_ActiveHPBars.Clear();
        }

        private void CreateHPBarsForActiveCombatables()
        {
            // 获取场景中所有活跃且非陷阱的战斗对象
            var combatables = FindObjectsOfType<MonoBehaviour>(false)
                .OfType<ICombatable>()
                .Where(c => 
                {
                    var mono = c as MonoBehaviour;
                    return mono != null 
                           && mono.gameObject.activeInHierarchy 
                           && !(mono is DungeonTrapBase); // 排除 TrapBase 及其子类
                });
    
            foreach (var combatable in combatables)
            {
                CreateHPBar(combatable);
            }
        }

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
            m_ActiveHPBars.Add(hpBar);
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