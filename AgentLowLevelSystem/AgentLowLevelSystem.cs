using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using GameFramework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityGameFramework.Runtime;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem
    {
        private Rigidbody2D m_AgentRigdbody;

        private void Start()
        {
            m_AgentRigdbody = GetComponent<Rigidbody2D>();

            InitSystem();
        }
        private void InitSystem()
        {
            
        }
        private void Update()
        {
            UpdateSystem();
        }

        private void UpdateSystem()
        {
            UpdateMoveSystem();
        }
        
    }
}
