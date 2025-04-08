using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dungeon.SkillSystem.SkillEffect
{
    public abstract class SkillEffectBase : ScriptableObject
    {
        public abstract void Fuck(SkillCalculator calculator);
    }

    public enum DndModifyChannel
    {
        Physical = 0,
        Fire = 1 << 0,
        Ice = 1 << 1,
        Holy = 1 << 2,
        Poison = 1 << 3,
    }

    [Serializable]
    /// <summary>
    /// 伤害
    /// </summary>
    public struct Damage
    {
        public DamageType type;
        public NDX value;

        public int Claculate(ICombatable target) => value.Claculate();
    }
    /// <summary>
    /// 调整值
    /// </summary>
    public struct DndModifier
    {
        public NDX value;
        public DndModifyChannel channel;
    }

    /// <summary>
    /// Buff
    /// </summary>
    public struct Buff
    {

    }
    public class SkillCalculator
    {
        public SkillCalculator(ICombatable attacker, ICombatable target, SkillDeployMethod deployDesc)
        {
            this.attacker = attacker;
            this.target = target;
            this.deployDesc = deployDesc;
        }

        /// <summary>
        /// 增加基础伤害
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public SkillCalculator AddBaseDamage(Damage damage)
        {
            switch (damage.type)
            {
                case DamageType.Physical:
                    physicalChannel.Add(damage);
                    break;
                case DamageType.Fire:
                    fireChannel.Add(damage);
                    break;
                case DamageType.Ice:
                    iceChannel.Add(damage);
                    break;
                case DamageType.Holy:
                    holyChannel.Add(damage);
                    break;
                case DamageType.Poison:
                    posionChannel.Add(damage);
                    break;
            }
            return this;
        }
        public SkillCalculator ApplyBuff(Buff buff)
        {
            //TODO: 增加buff效果
            return this;
        }
        public SkillCalculator GiveEffect_DamageOverTime(Damage damagePerSecond, float duration)
        {
            continuousEffects.Add(new ContinuousEffect(() =>
            {
                target.Hp -= damagePerSecond.Claculate(target);
            }, duration));
            return this;
        }
        public SkillCalculator GiveEffect_Heal(int amount)
        {
            immediateEffects.Add(new ImmediateEffect(() => target.Hp += amount));
            return this;
        }

        public SkillCalculator GiveEffect_HealOverTime(int amountPerSecond, float duration)
        {
            continuousEffects.Add(new ContinuousEffect(() =>
            {
                target.Hp += amountPerSecond;
            }, duration));
            return this;
        }
        public SkillCalculator GiveEffect_Stun(float duration)
        {
            immediateEffects.Add(new ImmediateEffect(() => target.Stun(duration)));
            return this;
        }
        public SkillCalculator GiveEffect_KnockBack(float distance)
        {
            var dir = deployDesc.SkillDirection;
            var dreamPos = deployDesc.SkillPosition + dir * distance;
            var wallPos = DungeonGameEntry.DungeonGameEntry.GridSystem.FindNearestWallInDirection(deployDesc.SkillPosition, dir);

            Vector2 finalPos = Vector2.zero;
            if (Vector2.Dot(dir, (wallPos - dreamPos).normalized) > 0)
                finalPos = wallPos;
            else
                finalPos = dreamPos;

            immediateEffects.Add(new ImmediateEffect(() =>
            {
                target.Stun(1f);
                DOTween.To((float t) =>
                {
                    target.GetGameObject().transform.position = Vector3.Lerp(deployDesc.SkillPosition, finalPos, t);
                }, 0, 1, 1f);
            }));
            return this;
        }
        public void Calculate()
        {
            int totalDamage = 0;

            totalDamage += physicalChannel.Sum(damage => damage.Claculate(target));
            totalDamage += fireChannel.Sum(damage => damage.Claculate(target));
            totalDamage += iceChannel.Sum(damage => damage.Claculate(target));
            totalDamage += holyChannel.Sum(damage => damage.Claculate(target));
            totalDamage += posionChannel.Sum(damage => damage.Claculate(target));

            target.Hp -= totalDamage;
        }
        private void CalculateEffects()
        {
            foreach (var effect in immediateEffects)
            {
                effect.Effect();
            }

            foreach (var effect in continuousEffects)
            {
                Task.Run(async () =>
                {
                    float remainingDuration = effect.Duration;
                    while (remainingDuration > 0)
                    {
                        effect.Effect();
                        await Task.Delay(1000); // 每秒触发一次
                        remainingDuration -= 1;
                    }
                });
            }

            foreach (var effect in timedEffects)
            {
                Task.Run(async () =>
                {
                    await Task.Delay((int)(effect.Delay * 1000)); // 转换为毫秒
                    effect.Effect();
                });
            }
        }

        private delegate void EffectHandler();

        private class ImmediateEffect
        {
            public EffectHandler Effect { get; }

            public ImmediateEffect(EffectHandler effect)
            {
                Effect = effect;
            }
        }

        private class ContinuousEffect
        {
            public EffectHandler Effect { get; }
            public float Duration { get; }

            public ContinuousEffect(EffectHandler effect, float duration)
            {
                Effect = effect;
                Duration = duration;
            }
        }

        private class TimedEffect
        {
            public EffectHandler Effect { get; }
            public float Delay { get; }

            public TimedEffect(EffectHandler effect, float delay)
            {
                Effect = effect;
                Delay = delay;
            }
        }
        private ICombatable attacker;
        private ICombatable target;
        private SkillDeployMethod deployDesc;
        private List<Damage> physicalChannel = new();
        private List<Damage> fireChannel = new();
        private List<Damage> iceChannel = new();
        private List<Damage> holyChannel = new();
        private List<Damage> posionChannel = new();
        private List<ImmediateEffect> immediateEffects = new();
        private List<ContinuousEffect> continuousEffects = new();
        private List<TimedEffect> timedEffects = new();
    }
}
