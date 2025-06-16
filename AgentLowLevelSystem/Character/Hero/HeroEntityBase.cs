using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity;
using Dungeon.GOAP;
using Dungeon.Vision2D;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Character
{
    [RequireComponent(typeof(AgentLowLevelSystem))]
    [RequireComponent(typeof(AgentHighLevelSystem))]
    [RequireComponent(typeof(GoapActionProvider))]
    [RequireComponent(typeof(AgentBehaviour))]
    public partial class HeroEntityBase : DungeonVisitorEntity
    {
        public string HeroName
        {
            get
            {
                if(m_LowLevelSystem!= null)
                    return m_LowLevelSystem.HeroName;

                m_LowLevelSystem = GetComponent<AgentLowLevelSystem>();
                return m_LowLevelSystem.HeroName;
            }
        }
        public void OnSpawn()
        {
            InitHighLevelSystem();

            InitLowLevelSystem();

            InitHeroBase();
            
            ValidateHeroBase();
        }

        void Update()
        {
            UpdateGOAP();
        }

        private void InitHighLevelSystem()
        {
            m_HighLevelSystem = GetComponent<AgentHighLevelSystem>();
            m_HighLevelSystem.OnSpawn(agentGoapType);
        }
        private void InitLowLevelSystem()
        {
            m_LowLevelSystem = GetComponent<AgentLowLevelSystem>();
            m_LowLevelSystem.OnSpawn();
        }

        private void InitHeroBase()
        {
            InitGOAP();

            InitViewer();
        }

        public override void OnSpawn(object data)
        {
            GameFrameworkLog.Warning("[HeroEntityBase.OnSpawn] Not well implemented");

            // 递归打开所有组件与子物体
            RecursivelyOpenComponents(transform);
        }

        private void RecursivelyOpenComponents(Transform transform)
        {
            transform.gameObject.SetActive(true);
            foreach (Transform child in transform)
            {
                RecursivelyOpenComponents(child);
            }
            foreach(var mono in transform.GetComponents<MonoBehaviour>())
            {
                mono.enabled = true;
            }
        }

        public Sprite GetSprite()
        {
            return GetComponentInChildren<SpriteRenderer>().sprite;
        }

        public override void Reset()
        {
            GameFrameworkLog.Warning("[HeroEntityBase.Reset] Not well implemented");
        }

        [LabelText("Goap的ai类型")] public AgentGoapType agentGoapType;
        private AgentHighLevelSystem m_HighLevelSystem;
        private AgentLowLevelSystem m_LowLevelSystem;
    }

    public partial class HeroEntityBase : DungeonVisitorEntity
    {
        private void ValidateHeroBase()
        {
            #if UNITY_EDITOR
            if(m_LowLevelSystem.Hp <= 0)
            {
                GameFrameworkLog.Warning("[HeroEntityBase.ValidateHeroBase] 英雄hp被设置为0,你确定吗?");
            }
            if(m_LowLevelSystem.Mp <= 0)
            {
                GameFrameworkLog.Warning("[HeroEntityBase.ValidateHeroBase] 英雄mp被设置为0,你确定吗?");
            }
            if(m_LowLevelSystem.AttackSpeed != 1)
            {
                GameFrameworkLog.Warning("[HeroEntityBase.ValidateHeroBase] 英雄攻击速度不是100%,你确定吗?");
            }
            #endif
        }
    }
}
