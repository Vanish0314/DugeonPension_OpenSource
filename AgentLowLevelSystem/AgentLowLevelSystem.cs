using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using Dungeon.BlackBoardSystem;
using Dungeon.Vision2D;
using GameFramework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityGameFramework.Runtime;

namespace Dungeon.AgentLowLevelSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Viewer))]
    [RequireComponent(typeof(BlackBoardSystem.BlackboardController))]
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem
    {
        private Rigidbody2D m_AgentRigdbody;
        private BoxCollider2D m_AgentCollider;
        private Animator m_AgentAnimator;
        private BlackboardController m_BlackboardController = new ();
        private BlackBoardSystem.Blackboard m_blackboard => m_BlackboardController.GetBlackboard();

        public BlackboardController GetBlackboard() => m_BlackboardController;

        private void Awake()
        {
            m_AgentRigdbody ??= GetComponent<Rigidbody2D>();

            InitSystem();
        }
        private void InitSystem()
        {
            InitMoveSystem();
            InitVisionSystem();
            InitDNDSystem();
        }
        private void Update()
        {
            UpdateSystem();
        }

        private void UpdateSystem()
        {
            UpdateMoveSystem();
            UpdateVisionSystem();
        }
        
    }
}
