using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonGameEntry;
using Dungeon.GOAP.Enums;
using Dungeon.GOAP.Goals;
using UnityEngine;

namespace Dungeon
{
    public class AgentHighLevelSystem : MonoBehaviour
    {

        private AgentBehaviour agent;
        private GoapActionProvider provider;
        private GoapBehaviour goap;

        private void Update()
        {

        }
        public void OnSpawn()
        {
            goap = DungeonGameEntry.DungeonGameEntry.GOAP;
            agent = GetComponent<AgentBehaviour>();
            provider = GetComponent<GoapActionProvider>();

            if (provider.AgentTypeBehaviour == null)
                provider.AgentType = goap.GetAgentType(AgentTypeIDs.Hero);

            provider.RequestGoal<EliminateThreatGoal, FinishDungeonGoal, LightDungeonRoomGoal, DesireFulfillmentGoal, AliveGoal>();
        }

        public void OnStunned()
        {
            provider.enabled = false;
        }
        public void OnStunnedEnd()
        {
            provider.enabled = true;
        }
    }
}
