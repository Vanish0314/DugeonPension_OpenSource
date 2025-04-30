using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Overload
{
    [CreateAssetMenu(fileName = "新魔王诅咒", menuName = "Overload/魔王诅咒")]
    public class OverloadCurse : ScriptableObject
    {
        [LabelText("诅咒名称")] public string curseName;
        [LabelText("诅咒Icon")] readonly public Sprite curseIcon;
        [LabelText("诅咒描述")] [TextArea] public string curseDescription;
        [LabelText("诅咒效果")] public CurseType curseType;
    }

}
