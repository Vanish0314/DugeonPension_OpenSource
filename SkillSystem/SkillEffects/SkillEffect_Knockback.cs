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
        public override void Fuck(ICombatable target,SkillDeployMethod deployDesc)
        {
            var dir = deployDesc.SkillDirection;
            var dreamPos = deployDesc.SkillPosition + dir * KnockbackDistance;
            var wallPos = DungeonGameEntry.DungeonGameEntry.GridSystem.FindNearestWallInDirection(deployDesc.SkillPosition, dir);
            
            Vector2 finalPos = Vector2.zero;
            if(Vector2.Dot(dir, (wallPos - dreamPos).normalized) > 0)
                finalPos = wallPos;
            else
                finalPos = dreamPos;

            target.Stun(1f);
            DOTween.To((float t) =>
            {
                target.GetGameObject().transform.position = Vector3.Lerp(deployDesc.SkillPosition, finalPos, t);
            },0,1,1f);
        }

    }
}
