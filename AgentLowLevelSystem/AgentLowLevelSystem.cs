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
    [RequireComponent(typeof(Viewer))]
    [RequireComponent(typeof(BlackboardController))]
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem, ICombatable
    {
        public string Name{get => m_Properties.heroName;}

        public BlackboardController GetBlackboard() => m_BlackboardController;
        public void OnSpawn()
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
            InitSkillSystem();
            InitAnimator();
        }
        private void Update()
        {
            UpdateSystem();
        }

        private void FixedUpdate()
        {
            FixedUpdateSystem();
        }

        private void UpdateSystem()
        {
            if (m_IsStunned)
                return;

            UpdateVisionSystem();
        }

        private void FixedUpdateSystem()
        {
            if (m_IsStunned || Hp <= 0)
                return;

            FixedUpdateMoveSystem();
        }

        private Rigidbody2D m_AgentRigdbody;
        private BoxCollider2D m_AgentCollider;
        private Animator m_AgentAnimator;
        private BlackboardController m_BlackboardController;
        private Blackboard blackboard => m_BlackboardController.GetBlackboard();

#if UNITY_EDITOR
        void OnValidate()
        {
            this.m_AgentRigdbody = GetComponent<Rigidbody2D>();
            this.m_BlackboardController = GetComponent<BlackboardController>();
        }
#endif
    }
}
