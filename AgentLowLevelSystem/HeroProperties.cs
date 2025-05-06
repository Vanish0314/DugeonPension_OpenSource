using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    [CreateAssetMenu(fileName = "新英雄属性", menuName = "Hero/英雄属性")]
    public class HeroProperties : ScriptableObject
    {
        [LabelText("勇者名字"),Tooltip("对话树的agent也是用这个名字")] public string heroName;
        [LabelText("屈服度"),Range(0,100)] public int Submissiveness= 0;
        [LabelText("六维属性")] public DndSkillData dndSkillData;
        [LabelText("战斗属性")] public CombatorData combatorData;
        [LabelText("血条设置")] public StatusBarSetting statusBarSetting;
        [LabelText("经验值"),Range(0,100)] public int currentExp;
    }
}
