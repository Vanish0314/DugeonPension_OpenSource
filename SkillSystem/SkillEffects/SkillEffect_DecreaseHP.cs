using System.Collections;
using System.Collections.Generic;
using Dungeon.SkillSystem.SkillEffect;
using Sirenix.Reflection.Editor;
using UnityEngine;

namespace Dungeon.SkillSystem.SkillEffect
{
    [CreateAssetMenu(fileName = "New Decrease HP Effect", menuName = "Skill System/Skill Effect/Decrease HP")]
    public class SkillEffect_DecreaseHP : SkillEffectBase
    {
        public Damage damage;
        public override void Fuck(SkillCalculator calculator)
        {
            calculator.AddBaseDamage(damage);
        }
    }
}
