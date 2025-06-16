using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Common;
using Dungeon.Evnents;
using GameFramework.Event;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dungeon
{
    public class GatherEffectHelper : MonoBehaviour
    {
        [SerializeField] private MonoPoolItem gatherEffect;
        [SerializeField] private Vector3 defaultScale = new Vector3(6, 6, 6);
        [SerializeField] private float spreadRadius = 200f; // 扩散半径
        [SerializeField] private float spreadDuration = 0.25f; // 扩散持续时间
        [SerializeField] private float flyDuration = 0.75f; // 飞行持续时间
        [SerializeField] private int maxSimultaneousEffects = 20; // 最大同时显示特效数量
        public Canvas mainCanvas; // 引用主Canvas
        private Camera mainCamera;
        
        private MonoPoolComponent gatherEffectPool;
        public static GatherEffectHelper Instance { get; private set; }
        public bool IsGathering {get; private set;} = false;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            transform.position = Vector3.zero;
            InitMonoPool();
        }

        private void OnEnable()
        { 
            Camera[] allCameras = FindObjectsOfType<Camera>(false);
            foreach (var activeCamera in allCameras)
            {
                if (activeCamera.isActiveAndEnabled)
                {
                    mainCamera = activeCamera;
                }
            }
        }

        private void InitMonoPool()
        {
            gatherEffectPool = gameObject.GetOrAddComponent<MonoPoolComponent>();
            gatherEffectPool.Init(
                "GatherEffectHelper",
                gatherEffect,
                transform,
                100
                );
        }

        public void OnGatherEffect(Sprite sprite, int amount, Vector3 startPos, Vector2 targetPos)
        {
            // 将世界坐标转换为屏幕坐标
            Vector3 screenStartPos = mainCamera.WorldToScreenPoint(startPos);
            
            // 限制同时播放的特效数量
            int effectsToSpawn = Mathf.Min(amount, maxSimultaneousEffects);
            StartCoroutine(SpawnEffectsWithDelay(sprite, effectsToSpawn, screenStartPos, targetPos));
        }

        private IEnumerator SpawnEffectsWithDelay(Sprite sprite, int count, Vector3 screenStartPos, Vector2 uiTargetScreenPos)
        {
            float delay = 0.05f;
        
            for (int i = 0; i < count; i += 3)
            {
                var effect = gatherEffectPool.GetItem(sprite);
            
                // 设置特效初始位置（屏幕空间）
                effect.transform.position = screenStartPos;
            
                StartCoroutine(PlayScreenSpaceAnimation(
                    effect.transform, 
                    screenStartPos, 
                    uiTargetScreenPos
                ));
            
                yield return new WaitForSeconds(delay);
            }
        }
        
        private IEnumerator PlayScreenSpaceAnimation(Transform effectTransform, Vector3 screenStartPos, Vector2 screenTargetPos)
        {
            IsGathering = true;
            
            // 设置初始缩放
            Vector3 startScale = defaultScale / mainCamera.orthographicSize;
            effectTransform.localScale = startScale;
            
            // 随机扩散位置（屏幕空间）
            Vector2 spreadPos = screenStartPos + 
                                new Vector3(
                                    Random.Range(-spreadRadius, spreadRadius),
                                    Random.Range(-spreadRadius, spreadRadius),
                                    0);
        
            // 扩散阶段
            float elapsedTime = 0f;
            while (elapsedTime < spreadDuration)
            {
                effectTransform.position = Vector3.Lerp(screenStartPos, spreadPos, elapsedTime / spreadDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            // 飞行到目标位置阶段
            elapsedTime = 0f;
            Vector3 flyStartPos = effectTransform.position;
            while (elapsedTime < flyDuration)
            {
                effectTransform.position = Vector3.Lerp(flyStartPos, screenTargetPos, elapsedTime / flyDuration);
            
                // 飞行过程中逐渐缩小
                float scale = Mathf.Lerp(startScale.x, 0.3f, elapsedTime / flyDuration);
                effectTransform.localScale = new Vector3(scale, scale, scale);
            
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            // 回收特效
            effectTransform.GetComponent<MonoPoolItem>().ReturnToPool();
            IsGathering = false;
        }
        
        private IEnumerator PlayEffectAnimation(Transform effectTransform, Vector2 uiStartPos, Vector2 uiTargetPos)
        {
            IsGathering = true;
            
            // 设置初始缩放
            Vector3 startScale = defaultScale / mainCamera.orthographicSize;
            effectTransform.localScale = startScale;
        
            // 随机扩散位置（在UI空间中）
            Vector2 spreadPos = uiStartPos + 
                                new Vector2(
                                    Random.Range(-spreadRadius, spreadRadius),
                                    Random.Range(-spreadRadius, spreadRadius));
        
            // 扩散阶段
            float elapsedTime = 0f;
            while (elapsedTime < spreadDuration)
            {
                effectTransform.localPosition = Vector2.Lerp(uiStartPos, spreadPos, elapsedTime / spreadDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            // 飞行到目标位置阶段
            elapsedTime = 0f;
            Vector2 flyStartPos = effectTransform.localPosition;
            while (elapsedTime < flyDuration)
            {
                effectTransform.localPosition = Vector2.Lerp(flyStartPos, uiTargetPos, elapsedTime / flyDuration);
            
                // 飞行过程中逐渐缩小
                float scale = Mathf.Lerp(startScale.x, 0.3f, elapsedTime / flyDuration);
                effectTransform.localScale = new Vector3(scale, scale, scale);
            
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            // 回收特效
            effectTransform.GetComponent<MonoPoolItem>().ReturnToPool();
            IsGathering = false;
        }
    }
}
