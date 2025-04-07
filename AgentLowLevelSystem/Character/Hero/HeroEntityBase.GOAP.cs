using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using Dungeon.DungeonGameEntry;
using Dungeon.GOAP.Enums;
using Dungeon.Vision2D;

namespace Dungeon.Character.Hero
{
    public partial class HeroEntityBase : DungeonVisitorEntity
    {
        private void InitGOAP()
        {
            m_GoapAgent = this.GetComponent<AgentBehaviour>();
            m_GoapActionProvider = this.GetComponent<GoapActionProvider>();
            m_Goap = DungeonGameEntry.DungeonGameEntry.GOAP;

            m_GoapActionProvider.AgentType ??= m_Goap.GetAgentType(AgentTypeIDs.Hero);

            // m_GoapAgent.Events.OnTa
        }

        private void UpdateGOAP()
        {

        }

        private AgentBehaviour m_GoapAgent;
        private GoapActionProvider m_GoapActionProvider;
        private GoapBehaviour m_Goap;
    }
}
