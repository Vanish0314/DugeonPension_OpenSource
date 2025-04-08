using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Dungeon.SkillSystem.SkillEffect
{
    [CreateAssetMenu(fileName = "New Knockback Effect", menuName = "Skill System/Skill Effect/Knockback")]
    public class SkillEffect_Knockback : SkillEffectBase
    {
        public float KnockbackDistance;
        public override void Fuck(SkillCalculator calculator)
        {
            calculator.GiveEffect_KnockBack(KnockbackDistance);
        }

    }
}
