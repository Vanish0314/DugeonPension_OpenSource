using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    // 气泡唯一标识
    public enum BubbleID
    {
        ExpBubble,
        DiceBubble,
        DialogueBubble,
    }

    // 气泡显示类型
    public enum BubbleStyle
    {
        FloatingFade, // 上浮淡出
        FollowTarget // 跟随目标移动
    }

    [CreateAssetMenu(fileName = "NewBubbleProfile", menuName = "Bubble System/Bubble Profile")]
    public class BubbleProfile : ScriptableObject
    {
        public BubbleID bubbleID;
        public BubbleStyle style;
        public GameObject bubblePrefab;
        public float duration = 2f;
        public Vector2 positionOffset;
        public Color textColor = Color.white;
        public AnimationCurve moveCurve; // 运动曲线
        public AnimationCurve alphaCurve; // 透明度曲线
    }
}
