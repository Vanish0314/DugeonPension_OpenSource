using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Dungeon.AgentLowLevelSystem;
using Dungeon.Character.Hero;
using Dungeon.Vision2D;
using GameFramework;
using UnityEngine;

namespace Dungeon.Character.Hero
{
    [RequireComponent(typeof(AgentLowLevelSystem.AgentLowLevelSystem))]
    [RequireComponent(typeof(AgentHighLevelSystem))]
    [RequireComponent(typeof(GoapActionProvider))]
    [RequireComponent(typeof(AgentBehaviour))]
    public partial class HeroEntityBase : DungeonVisitorEntity
    {
        public void OnSpawn()
        {
            InitHighLevelSystem();

            InitLowLevelSystem();

            InitHeroBase();
        }

        void Update()
        {
            UpdateGOAP();
        }

        private void InitHighLevelSystem()
        {
            m_HighLevelSystem = GetComponent<AgentHighLevelSystem>();
            m_HighLevelSystem.OnSpawn();
        }
        private void InitLowLevelSystem()
        {
            m_LowLevelSystem = GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();
            m_LowLevelSystem.OnSpawn();
        }

        private void InitHeroBase()
        {
            InitGOAP();

            InitViewer();
        }

        public override void OnSpawn(object data)
        {
            GameFrameworkLog.Warning("[HeroEntityBase.OnSpawn] Not implemented");
        }

        public override void Reset()
        {
            GameFrameworkLog.Warning("[HeroEntityBase.Reset] Not implemented");
        }

        private AgentHighLevelSystem m_HighLevelSystem;
        private AgentLowLevelSystem.AgentLowLevelSystem m_LowLevelSystem;
    }
}
