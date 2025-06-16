using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using Dungeon.SkillSystem.SkillEffect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "New GiveCharmismBuff Effect", menuName = "Skill System/Skill Effect/GiveCharmismBuff")]
    public class SkillEffect_GiveCharmismBuff : SkillEffectBase
    {
        [InfoBox("给一个持续时长为time的,修改魅力value的buff")]
        public int value;
        public float time;
        public override void Fuck(SkillCalculator calculator)
        {
            calculator.GiveBuff(new Buff_Charming(time, value));
        }
    }
}
