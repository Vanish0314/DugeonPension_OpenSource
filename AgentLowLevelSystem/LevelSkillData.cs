using System.Collections;
using System.Collections.Generic;
using Dungeon.SkillSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "新勇者技能配置", menuName = "Hero/HeroSkillData")]
    public class LevelSkillData : ScriptableObject
    {
        [Header("注意,掌握技能需要与goap graph对应")]
        [LabelText("1级时能掌握的技能")] public List<SkillData> skillsInLevel1;
        [LabelText("5级时能额外掌握的技能")] public List<SkillData> skillsInLevel5;
        [LabelText("10级时能额外掌握的技能")] public List<SkillData> skillsInLevel10;
        [LabelText("15级时能额外掌握的技能")] public List<SkillData> skillsInLevel15;
        [LabelText("20级时能额外掌握的技能")] public List<SkillData> skillsInLevel20;
    }
}
