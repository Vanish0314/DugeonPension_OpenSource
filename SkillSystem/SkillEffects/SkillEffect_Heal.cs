using System.Collections;
using System.Collections.Generic;
using Dungeon.SkillSystem.SkillEffect;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "New Heal Effect", menuName = "Skill System/Skill Effect/Heal")]
    public class SkillEffect_Heal : SkillEffectBase
    {
        public int HealAmount;
        public override void Fuck(SkillCalculator calculator)
        {
            calculator.GiveEffect_Heal(HealAmount);
        }

    }
}
