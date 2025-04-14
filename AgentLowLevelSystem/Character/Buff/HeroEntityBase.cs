using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Dungeon.AgentLowLevelSystem;
using Dungeon.Character.Hero;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon.Character.Hero
{
    [RequireComponent(typeof(AgentLowLevelSystem.AgentLowLevelSystem))]
    [RequireComponent(typeof(AgentHighLevelSystem))]
    [RequireComponent(typeof(GoapActionProvider))]
    [RequireComponent(typeof(AgentBehaviour))]
    public partial class HeroEntityBase : DungeonVisitorEntity
    {
        void Start()
        {
            InitHero();
        }

        void Update()
        {
            UpdateGOAP();
        }

        private void InitHero()
        {
            InitICharacter();

            InitGOAP();

            InitViewer();
        }

        public override void Init(object data)
        {
            
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}
