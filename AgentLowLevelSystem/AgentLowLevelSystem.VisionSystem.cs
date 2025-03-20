using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem
    {
        private Viewer m_Viewer;
        private Vision m_Vision;

        private void InitVisionSystem()
        {
            m_Viewer ??= GetComponent<Viewer>();
            m_Vision ??= GetComponentInChildren<Vision>();
        }
        private void UpdateVisionSystem() 
        {
            
        }
    }
}
