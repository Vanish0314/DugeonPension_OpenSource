using System;
using System.Collections;
using GameFramework;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Dungeon.GameFeel
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class SpriteVisualEffectController : MonoBehaviour
    {
        public Action OnSkillVisualEffectStart;
        public Action OnSkillVisualEffectInterrupt;
        public Action OnSkillVisualEffectEnd;

        private bool isPlaying = false;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private static readonly int PlayTrigger = Animator.StringToHash("Play");

        void OnEnable()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            isPlaying = false;
            spriteRenderer.enabled = false;
        }

        void OnDisable()
        {
            Interrupt();
        }

        public void Play()
        {
            if (isPlaying) return;

            isPlaying = true;
            spriteRenderer.enabled = true;
            animator.SetTrigger(PlayTrigger);
            OnSkillVisualEffectStart?.Invoke();

            StartCoroutine(CheckAnimationEnd());
        }

        private IEnumerator CheckAnimationEnd()
        {
            yield return null;

            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float length = stateInfo.length;

            yield return new WaitForSeconds(length);

            OnEffectAnimationEnd();
        }


        public void Interrupt()
        {
            if (!isPlaying) return;

            isPlaying = false;
            spriteRenderer.enabled = false;
            OnSkillVisualEffectInterrupt?.Invoke();

            Destroy(gameObject);
        }

        public void OnEffectAnimationEnd()
        {
            if (!isPlaying) return;

            isPlaying = false;
            spriteRenderer.enabled = false;
            OnSkillVisualEffectEnd?.Invoke();

            Destroy(gameObject);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            animator = GetComponent<Animator>();
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                GameFrameworkLog.Error($"[SpriteVisualEffectController] {name} 没有配置Animator");
                return;
            }

            var controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null)
            {
                GameFrameworkLog.Error($"[SpriteVisualEffectController] {name} 动画控制器不是AnimatorController");
                return;
            }

            int clipCount = controller.animationClips.Length;
            if (clipCount != 1)
            {
                GameFrameworkLog.Error($"[SpriteVisualEffectController] {name} 动画片段数量不为1, 请检查动画配置");
            }
            else
            {
                GameFrameworkLog.Info($"[SpriteVisualEffectController] {name} 动画片段数量正确");
            }
        }
#endif
    }
}
