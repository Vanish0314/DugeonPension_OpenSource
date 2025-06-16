using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonEntity;
using Dungeon.GOAP;
using Dungeon.Vision2D;

namespace Dungeon.Character
{
    public partial class HeroEntityBase : DungeonVisitorEntity
    {
        private void InitGOAP()
        {
            m_GoapAgent = GetComponent<AgentBehaviour>();
            m_GoapActionProvider = GetComponent<GoapActionProvider>();
            m_Goap = DungeonGameEntry.DungeonGameEntry.GOAP;

            m_GoapActionProvider.AgentType ??= m_Goap.GetAgentType(agentGoapType.ToString());

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
