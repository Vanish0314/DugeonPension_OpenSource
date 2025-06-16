using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class BubbleManager : MonoBehaviour
    {
        public static BubbleManager Instance;

        [SerializeField] private BubbleProfile[] profiles;
        private Dictionary<BubbleID, BubbleProfile> profileDict;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                LoadProfiles();
                InitializeProfileDictionary();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void LoadProfiles()
        {
            profiles = Resources.LoadAll<BubbleProfile>("BubbleProfiles");
        }
        
        private void InitializeProfileDictionary()
        {
            profileDict = new Dictionary<BubbleID, BubbleProfile>();
            foreach (var profile in profiles)
            {
                if (!profileDict.ContainsKey(profile.bubbleID))
                {
                    profileDict.Add(profile.bubbleID, profile);
                }
                else
                {
                    Debug.LogWarning($"重复的BubbleID: {profile.bubbleID}");
                }
            }
        }

        public void ShowBubble(Transform target, string content, BubbleID id)
        {
            // 检查是否已经有相同目标的气泡
            foreach(var existingBubble in FindObjectsOfType<BubbleAnimator>())
            {
                if (existingBubble.target == target && existingBubble.bubbleID == id)
                {
                    return; // 不再创建新气泡
                }
            }
            
            StartCoroutine(ProcessBubble(target, content, id));
        }
        
        private IEnumerator ProcessBubble(Transform target, string content, BubbleID id)
        {
            if (!profileDict.TryGetValue(id, out var profile))
            {
                Debug.LogError($"未找到BubbleID: {id} 的配置");
                yield break;
            }

            if (target == null)
            {
                Debug.LogError("目标Transform为空");
                yield break;
            }

            GameObject bubbleObj = Instantiate(profile.bubblePrefab);
            SetupBubblePosition(bubbleObj, target, profile); 
            InitializeBubbleContent(bubbleObj, content, profile);

            BubbleAnimator animator = bubbleObj.AddComponent<BubbleAnimator>();
            animator.target = target;
            animator.bubbleID = id;
            yield return StartCoroutine(ExecuteAnimation(animator, bubbleObj, target, profile)); 
            
            Destroy(bubbleObj);
        }
        
        private void SetupBubblePosition(GameObject obj, Transform target, BubbleProfile profile)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
    
            // 设置初始位置
            rt.position = target.position + (Vector3)profile.positionOffset;
    
            // World Space缩放调整
            rt.localScale = Vector3.one * 0.01f; 
        }

        private void InitializeBubbleContent(GameObject obj, string content, BubbleProfile profile)
        {
            GameFrameworkLog.Debug("InitializeBubbleContent");
            Text textComp = obj.GetComponentInChildren<Text>();
            if (textComp != null)
            {
                textComp.text = content;
                textComp.color = profile.textColor;
            }
        }

        private IEnumerator ExecuteAnimation(BubbleAnimator animator, GameObject obj, Transform target, BubbleProfile profile)
        {
            GameFrameworkLog.Debug("ExecuteAnimation");
            switch (profile.style)
            {
                case BubbleStyle.FloatingFade:
                    yield return StartCoroutine(animator.FloatingFade(obj, profile, target));
                    break;
                case BubbleStyle.FollowTarget:
                    yield return StartCoroutine(animator.FollowTarget(obj, target, profile));
                    break;
                default:
                    yield return new WaitForSeconds(profile.duration);
                    break;
            }
        }
    }
}
