using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class BubbleAnimator : MonoBehaviour
    {
        public Transform target;
        public BubbleID bubbleID;
        public IEnumerator FloatingFade(GameObject obj, BubbleProfile profile, Transform target)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            CanvasGroup cg = obj.AddComponent<CanvasGroup>();
            
            // 初始位置基于目标或独立位置
            Vector3 basePosition = target != null ? target.position : rt.position;
            Vector3 startPos = basePosition + (Vector3)profile.positionOffset;

            float timer = 0f;
            while (timer < profile.duration)
            {
                float progress = timer / profile.duration;
                
                // 更新基础位置（如果跟随目标）
                if (target != null)
                {
                    basePosition = target.position;
                }
                
                // 计算新位置：基础位置 + 偏移 + 上浮
                Vector3 newPos = basePosition + 
                                 (Vector3)profile.positionOffset + 
                                 Vector3.up * profile.moveCurve.Evaluate(progress);
                
                rt.position = newPos;
                
                // 透明度控制
                cg.alpha = profile.alphaCurve.Evaluate(progress);
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // 动画结束后销毁对象
            Destroy(obj);
        }
        
        public IEnumerator FollowTarget(GameObject obj, Transform target, BubbleProfile profile)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            Vector3 offset = (Vector3)profile.positionOffset;

            float timer = 0f;
            while (timer < profile.duration)
            {
                if (target != null)
                {
                    // 直接更新世界坐标
                    rt.position = target.position + offset;
                }

                // 处理透明度
                CanvasGroup cg = obj.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = profile.alphaCurve.Evaluate(timer / profile.duration);
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
