using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework;
using UnityEngine;

namespace Dungeon.SkillSystem.SkillEffect
{
    [CreateAssetMenu(fileName = "New DamageOverTime Effect", menuName = "Skill System/Skill Effect/DamageOverTime")]
    public class SkillEffect_DamageOverTime : SkillEffectBase
    {
        public Damage damagePerSecond;
        public float duration;
        public override void Fuck(SkillCalculator calculator)
        {
            calculator.GiveEffect_DamageOverTime(damagePerSecond, duration);
        }
    }
}
