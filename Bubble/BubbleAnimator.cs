using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class BubbleAnimator : MonoBehaviour
    {
        public IEnumerator FloatingFade(GameObject obj, BubbleProfile profile)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            CanvasGroup cg = obj.AddComponent<CanvasGroup>();
            Vector3 startPos = rt.position; // 使用世界坐标

            float timer = 0f;
            while (timer < profile.duration)
            {
                float progress = timer / profile.duration;
                
                // 在世界空间移动
                Vector3 newPos = startPos + Vector3.up * (profile.moveCurve.Evaluate(progress) * 2.5f);
                rt.position = newPos;
                
                // 透明度控制
                cg.alpha = profile.alphaCurve.Evaluate(progress);
                
                timer += Time.deltaTime;
                yield return null;
            }
        }

        // 修改8：接收显式target参数
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
