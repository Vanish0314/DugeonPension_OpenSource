using System.Collections;
using System.Collections.Generic;
using Dungeon.SkillSystem.SkillEffect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.SkillSystem.SkillEffect
{
    [CreateAssetMenu(fileName = "New Increase Submissiveness Effect", menuName = "Skill System/Skill Effect/Increase Submissiveness")]
    public class SkillEffect_IncreaseSubmissiveness : SkillEffectBase
    {
        [LabelText("增加的屈服度")] public int count;
        public override void Fuck(SkillCalculator calculator)
        {
            calculator.GiveEffect_IncreaseSubmissiveness(count);
        }
    }
}
