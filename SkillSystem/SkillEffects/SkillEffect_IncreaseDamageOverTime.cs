using System.Collections;
using System.Collections.Generic;
using Dungeon.SkillSystem;
using Dungeon.SkillSystem.SkillEffect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.SkillSystem
{
    [CreateAssetMenu(fileName = "New IncreaseDamageOverTime Effect", menuName = "Skill System/Skill Effect/IncreaseDamageOverTime")]
    public class SkillEffect_IncreaseDamageOverTime : SkillEffectBase
    {
        [InfoBox("暂未实现")]
        public NDX value;
        public override void Fuck(SkillCalculator calculator)
        {
            // calculator.GiveEffect_IncreaseDamageOverTime(value);
        }
    }
}
