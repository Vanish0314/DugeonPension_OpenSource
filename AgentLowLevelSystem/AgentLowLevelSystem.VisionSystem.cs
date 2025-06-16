using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon.Character
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem,ICombatable
    {
        private Viewer m_Viewer;
        private Vision m_Vision;

        public Viewer GetViewer() => m_Viewer;
        public Vision GetVision() => m_Vision;

        private void InitVisionSystem()
        {
            m_Viewer ??= GetComponent<Viewer>();
            m_Vision ??= GetComponentInChildren<Vision>();

            m_Vision.Init(this);
        }
        private void UpdateVisionSystem() 
        {
            
        }
    }
}
