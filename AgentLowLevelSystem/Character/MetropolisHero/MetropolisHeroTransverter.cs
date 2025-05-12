using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Hero;
using Dungeon.Common.MonoPool;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dungeon
{
    public class MetropolisHeroTransverter : MonoBehaviour
    {
        private MonoPoolComponent m_MonoPoolComponent;
        [SerializeField] private MonoPoolItem metropolisHeroTemplate;
        
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
            m_MonoPoolComponent = gameObject.GetOrAddComponent<MonoPoolComponent>();
            m_MonoPoolComponent.Init("metropolisHero", metropolisHeroTemplate, transform, 16);
        }
        
        public void TransverseMetropolisHero(HeroEntityBase hero)
        {
            switch (hero.HeroName)
            {
                case "MetropolisHero":
                    m_MonoPoolComponent.GetItem(null);
                    break;
                default:
                    var metropolisHero = m_MonoPoolComponent.GetItem(null);
                    metropolisHero.transform.position = Vector3.zero;
                    break;
            }
        }
    }
}
