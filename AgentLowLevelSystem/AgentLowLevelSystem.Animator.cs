using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem, ICombatable
    {
        private const string ANIMATOR_BOOL_IDLE = "Idle";
        private const string ANIMATOR_BOOL_MOVING = "Moving";
        private const string ANIMATOR_BOOL_ATTACKING = "Attacking";
        private const string ANIMATOR_BOOL_STUN = "Stunned";
        private const string ANIMATOR_TRIGGER_INTERACT = "Interacting";
        private const string ANIMATOR_TRIGGER_Die = "Died";

        private void InitAnimator()
        {
            var transf = transform.Find("Sprite");
            if(transf == null)
            {
                var go = new GameObject("Sprite");
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                m_AgentAnimator = go.AddComponent<Animator>();
            }
            else
            {m_AgentAnimator = transf.gameObject.GetOrAddComponent<Animator>();}


            m_AgentAnimator.SetBool(ANIMATOR_BOOL_IDLE, true);
            m_AgentAnimator.SetBool(ANIMATOR_BOOL_MOVING, false);
            m_AgentAnimator.SetBool(ANIMATOR_BOOL_ATTACKING, false);
            m_AgentAnimator.SetBool(ANIMATOR_BOOL_STUN, false);
        }
        private void SetAnimatorState(string stateName)
        {
            switch (stateName)
            {
                case ANIMATOR_BOOL_IDLE:
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_IDLE, true);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_MOVING, false);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_ATTACKING, false);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_STUN, false);
                    break;
                case ANIMATOR_BOOL_MOVING:
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_IDLE, false);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_MOVING, true);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_ATTACKING, false);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_STUN, false);
                    break;
                case ANIMATOR_BOOL_ATTACKING:
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_IDLE, false);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_MOVING, false);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_ATTACKING, true);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_STUN, false);
                    break;
                case ANIMATOR_BOOL_STUN:
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_IDLE, false);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_MOVING, false);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_ATTACKING, false);
                    m_AgentAnimator.SetBool(ANIMATOR_BOOL_STUN, true);
                    break;
            }
        }
        private void SetAnimatorState(string stateName, float duration)
        {
            AddToTween((float t) =>
            {
                SetAnimatorState(stateName);
            }, 0, 1, duration,
            () =>
            {
                SetAnimatorState(ANIMATOR_BOOL_IDLE);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setter"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        /// <param name="onKill"></param>
        /// <param name="onComplete"></param>
        private void AddToTween(DOSetter<float> setter, float startValue, float endValue, float duration, TweenCallback onKill = null, TweenCallback onComplete = null)
        {
            var tween = DOTween.To(setter, 0, 1, duration);

            if (onKill != null)
                tween.onKill += onKill;

            if (onComplete == null)
            {
                tween.onComplete += () =>
                {
                    WipTweens.Remove(tween);
                };
            }
            else
            {
                tween.onComplete += onComplete;
            }

            tween.onKill += ()=>
            {
                WipTweens.Remove(tween);
            };
            tween.onComplete += () =>
            {
                WipTweens.Remove(tween);
            };

            WipTweens.Add(tween);
        }
        private void SetAnimatorTrigger(string triggerName)
        {
#if UNITY_EDITOR
            CheckIfAnimatorHasParameter(triggerName);
#endif
            m_AgentAnimator.SetTrigger(triggerName);
        }
#if UNITY_EDITOR
        [Obsolete]
        private bool CheckIfAnimatorHasParameter(string parameterName)
        {
            bool flag = false;
            foreach (var param in m_AgentAnimator.parameters)
            {
                if (param.name == parameterName)
                {
                    flag = true;
                }
            }
            Debug.Assert(flag, "[AgentLowLevelSystem] Animator Parameter not found: " + parameterName + "\nMake sure the Animator has a parameter with the name and set it as a trigger parameter.");

            return flag;
        }
#endif
    }
}
