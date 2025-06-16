using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework;
using UnityEngine;

namespace Dungeon.GameFeel
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShaderEffectHelper : MonoBehaviour
    {
        void Start()
        {
            mSpriteRenderer = GetComponent<SpriteRenderer>();
            mMaterial = mSpriteRenderer.material;

#if UNITY_EDITOR
            if (!mMaterial.shader.name.StartsWith("AllIn1SpriteShader"))
            {
                GameFrameworkLog.Error($"[ShaderEffectHelper] 物体材质设置错误. 请将材质设置为AllIn1SpriteShader.但更推荐增加 AddAllIn1Shader 组件\n物体名称: {gameObject.name}\n材质名称: {mMaterial.shader.name}");
            }
#endif
        }
        void OnEnable()
        {
            mOwner = InverseTraverseHierarchyFind<ICombatable>(transform);
            if (mOwner != null)
            {
                mOwner.CombatEvents.OnBeAttacked += OnBeAttacked;
            }
        }

        void OnDisable()
        {
            if (mOwner != null)
            {
                mOwner.CombatEvents.OnBeAttacked -= OnBeAttacked;
            }
        }


        public void OnBeAttacked(Skill skill)
        {
            mHitEffectTween?.Kill();

            mMaterial.EnableKeyword("HITEFFECT_ON");

            mHitEffectTween = DOTween.To(
                () => mMaterial.GetFloat("_HitEffectBlend"),
                value => mMaterial.SetFloat("_HitEffectBlend", value),
                1f, skill.skillData.midCastTimeInSec / 1.8f
            )
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                DisableHitEffect();
            });
        }

        private void DisableHitEffect()
        {
            mMaterial.SetFloat("_HitEffectBlend", 0f);
            mMaterial.DisableKeyword("HITEFFECT_ON");
        }
        private T InverseTraverseHierarchyFind<T>(Transform current) where T : class
        {
            while (current != null)
            {
                var component = current.GetComponent<T>();
                if (component != null)
                    return component;
                current = current.parent;
            }
            return null;
        }

        private ICombatable mOwner;
        private Material mMaterial;
        private SpriteRenderer mSpriteRenderer;
        private Tween mHitEffectTween;
    }
}
