using System.Linq;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP;
using UnityEngine;

namespace Dungeon
{
    public class AgentHighLevelSystem : MonoBehaviour
    {
        private GoapActionProvider provider;
        private GoapBehaviour goap;

        private void Update()
        {

        }
        public void OnSpawn(AgentGoapType agentGoapType)
        {
            goap = DungeonGameEntry.DungeonGameEntry.GOAP;
            provider = GetComponent<GoapActionProvider>();

            if (provider.AgentTypeBehaviour == null)
            {
                provider.AgentType = goap.GetAgentType(agentGoapType.ToString());

                var allGoals = provider.AgentType.GetGoals();
                var goalTypes = allGoals.Select(g => g.GetType()).ToArray();
                provider.RequestGoal(goalTypes, true);
            }
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
