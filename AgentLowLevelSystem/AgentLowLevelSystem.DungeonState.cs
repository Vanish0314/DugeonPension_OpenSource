using CrashKonijn.Agent.Core;
using GameFramework;
using UnityEngine;

namespace Dungeon.Character
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem, ICombatable
    {
        private void OnDied()
        {
            GameFrameworkLog.Info($"[Hero] 悲报,勇者{HeroName}被曹氏啦TAT");

            SetAnimatorState(ANIMATOR_BOOL_DIE, 99999);
            m_Vision.gameObject.SetActive(false);
            this.GetComponent<HeroEntityBase>().OnDie();
        }

        public void RevivalMe()
        {
            SetAnimatorState(ANIMATOR_BOOL_IDLE, 1);
            m_Vision.gameObject.SetActive(true);
            
            this.GetComponent<HeroEntityBase>().OnRevival();
         }
    }
}
