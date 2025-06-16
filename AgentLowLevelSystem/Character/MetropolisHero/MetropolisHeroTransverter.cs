using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using Dungeon.Common;
using GameFramework;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Dungeon
{
    public class MetropolisHeroTransverter : MonoBehaviour
    {
        [Serializable]
        public class HeroConversionMapping
        {
            public string dungeonHeroName;
            public MonoPoolItem metropolisHeroPrefab;
            public MonoPoolComponent metropolisHeroPool;
        }
        
        [SerializeField] HeroConversionMapping[] mappings;
        Dictionary<string,MonoPoolComponent> transferPool = new Dictionary<string, MonoPoolComponent>();
        
        [SerializeField] private Vector3 defaultPosition;
        [SerializeField] private Vector3 randomVectorRange;
        
        public static MetropolisHeroTransverter Instance;
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
            InitMonoPoolComponent();
        }

        private void InitMonoPoolComponent()
        {
            foreach (var mapping in mappings)
            {
                mapping.metropolisHeroPool = GetOrCreateMonoPoolComponent(mapping.dungeonHeroName);
                mapping.metropolisHeroPool.Init(mapping.dungeonHeroName + "Pool", mapping.metropolisHeroPrefab,
                    mapping.metropolisHeroPool.transform, 8);
                
                transferPool.Add(mapping.dungeonHeroName, mapping.metropolisHeroPool);
            }
        }
        
        private MonoPoolComponent GetOrCreateMonoPoolComponent(string name)
        {
            var child = transform.Find(name);
            var obj = child != null ? child.gameObject : new GameObject(name);
            obj.transform.SetParent(transform);
            return obj.GetOrAddComponent<MonoPoolComponent>();
        }

        private MonoPoolItem TransferHeroOfName(string heroName)
        {
            if (transferPool.TryGetValue(heroName, out var pool))
            {
                return pool.GetItem(null);
            }
            return null;
        }
        
        public MetropolisHeroBase TransverseToMetropolisHero(HeroEntityBase dungeonHero)
        {
            // 从对象池获取城堡勇者实例
            var metropolisHeroObj = TransferHeroOfName(dungeonHero.HeroName);
            var metropolisHero = metropolisHeroObj.GetComponent<MetropolisHeroBase>();
            
            if (metropolisHero == null)
            {
                GameFrameworkLog.Error("转换失败: 获取的预制体没有MetropolisHeroBase组件");
                return null;
            }
          
            // 转换位置  
            float offsetX = UnityEngine.Random.Range(-randomVectorRange.x, randomVectorRange.x);
            float offsetY = UnityEngine.Random.Range(-randomVectorRange.y, randomVectorRange.y);
            Vector3 metropolisHeroPosition = defaultPosition + new Vector3(offsetX, offsetY, 0);
            metropolisHero.transform.position = metropolisHeroPosition;
            
            // 转换属性数据
            ConvertHeroAttributes(dungeonHero, metropolisHero);
            
            // 先禁用
            metropolisHeroObj.gameObject.SetActive(false);
            
            // 注册到管理器
            MetropolisHeroManager.Instance.RegisterHero(metropolisHero);
            
            return metropolisHero;
        }

        private void ConvertHeroAttributes(HeroEntityBase dungeonHero, MetropolisHeroBase metropolisHero)
        {
            
        }
    }
}
