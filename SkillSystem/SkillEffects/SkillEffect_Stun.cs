using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.SkillSystem.SkillEffect
{
    [CreateAssetMenu(fileName = "New Stun Effect", menuName = "Skill System/Skill Effect/Stun")]
    public class SkillEffect_Stun : SkillEffectBase
    {
        [SerializeField,LabelText("僵直持续时间")] float duration;
        public override void Fuck(ICombatable target, SkillDeployMethod deployDesc)
        {
            target.Stun(duration);
        }
    }
}
