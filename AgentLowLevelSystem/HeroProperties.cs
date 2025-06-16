using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ParadoxNotion.Design;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Character
{
    [CreateAssetMenu(fileName = "新英雄属性", menuName = "Hero/英雄属性")]
    public class HeroProperties : ScriptableObject
    {
        [LabelText("勇者名字"),Tooltip("对话树的agent也是用这个名字")] public string heroName;
        [LabelText("屈服度"),UnityEngine.Range(0,100)] public int Submissiveness= 0;
        [LabelText("性格属性")] public CharacterTrait characterTrait;
        [LabelText("六维属性")] public DndSkillData dndSkillData;
        [LabelText("战斗属性")] public CombatorData combatorData;
        [LabelText("血条设置")] public StatusBarSetting statusBarSetting;
        [LabelText("经验值"),UnityEngine.Range(0,100)] public int currentExp;
    }

    [System.Serializable]
    public class CharacterTrait
    {
        /// <summary>
        /// 开拓
        /// </summary>
        [LabelText("开拓"),UnityEngine.Range(1,20)] public int Adventurous;

        /// <summary>
        /// 社交
        /// </summary>
        [LabelText("社交"),UnityEngine.Range(1,20)] public int Social;

        /// <summary>
        /// 同理
        /// </summary>
        [LabelText("同理"),UnityEngine.Range(1,20)] public int Sympathy;

        /// <summary>
        /// 强欲
        /// </summary>
        [LabelText("强欲"),UnityEngine.Range(1,20)] public int Desire;

        /// <summary>
        /// 激进
        /// </summary>
        [LabelText("激进"),UnityEngine.Range(1,20)] public int Aggressive;

        /// <summary>
        /// 稳定
        /// </summary>
        [LabelText("稳定"),UnityEngine.Range(1,20)] public int Stable;
    }
}
