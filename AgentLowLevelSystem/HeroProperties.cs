using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    [CreateAssetMenu(fileName = "新英雄属性", menuName = "Hero/英雄属性")]
    public class HeroProperties : ScriptableObject
    {
        [LabelText("六维属性")] public DndSkillData dndSkillData;
        [LabelText("战斗属性")] public CombatorData combatorData;
        [LabelText("血条设置")] public StatusBarSetting statusBarSetting;
        [LabelText("经验值"),Range(0,100)] public int currentExp;
    }
}
