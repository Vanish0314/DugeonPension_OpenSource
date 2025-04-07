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
        public override void Fuck(ICombatable target,SkillDeployMethod deployDesc)
        {
            Task.Run(async () =>
            {
                GameFrameworkLog.Warning("[SKillEffect_DamageOverTime] 在多线程中调用了线程不安全的函数");
                while (duration > 0)
                {

                    target.Hp -= damagePerSecond.Claculate(target);
                    await Task.Delay(1000);
                    duration -= 1;
                }
            });
        }
    }
}
