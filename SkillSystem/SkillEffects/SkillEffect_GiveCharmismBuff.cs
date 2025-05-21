using System.Collections;
using System.Collections.Generic;
using Dungeon.AgentLowLevelSystem;
using Dungeon.SkillSystem.SkillEffect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon
{
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
