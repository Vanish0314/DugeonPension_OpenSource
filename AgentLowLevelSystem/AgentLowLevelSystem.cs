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
    [RequireComponent(typeof(BlackboardController))]
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem,ICombatable
    {
        private Rigidbody2D m_AgentRigdbody;
        private BoxCollider2D m_AgentCollider;
        private Animator m_AgentAnimator;
        private BlackboardController m_BlackboardController;
        private BlackBoardSystem.Blackboard m_blackboard => m_BlackboardController.GetBlackboard();

        public BlackboardController GetBlackboard() => m_BlackboardController;

        void OnValidate()
        {
            this.m_AgentRigdbody = GetComponent<Rigidbody2D>();
            this.m_BlackboardController = GetComponent<BlackboardController>();
        }

        private void Awake()
        {
            m_AgentRigdbody ??= GetComponent<Rigidbody2D>();
            m_BlackboardController ??= gameObject.GetOrAddComponent<BlackboardController>();

            gameObject.layer = LayerMask.NameToLayer("Hero");

            InitSystem();
        }
        private void InitSystem()
        {
            InitMoveSystem();
            InitVisionSystem();
            InitDNDSystem();
            InitText();
            InitSkillSystem();
            InitAnimator();
        }
        private void Update()
        {
            UpdateSystem();
            UpdateText();
        }

        private void UpdateSystem()
        {
            if(m_IsStunned)
                return;

            UpdateMoveSystem();
            UpdateVisionSystem();
        }
        
    }
}
