using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using Dungeon.AgentLowLevelSystem;
using GameFramework;
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
        public SkillCalculator(ICombatable attacker, ICombatable target, Skill skill)
        {
            this.attacker = attacker;
            this.target = target;
            this.deployDesc = skill.skillDeployMethod;
            this.skill = skill;
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
        public SkillCalculator GiveEffect_IncreaseSubmissiveness(int amount)
        {
            immediateEffects.Add(new ImmediateEffect(() =>{
                var low = target.GetGameObject().GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

                if(low!= null)
                {
                    low.ModifySubmissiveness(amount);
                }
                else
                {
                    GameFrameworkLog.Error("[Skill Calculator] AgentLowLevelSystem not found");
                }
            }));

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

            var phy = physicalChannel.Sum(damage => damage.Claculate(target));
            var fire = fireChannel.Sum(damage => damage.Claculate(target));
            var ice = iceChannel.Sum(damage => damage.Claculate(target));
            var holy = holyChannel.Sum(damage => damage.Claculate(target));
            var posion = posionChannel.Sum(damage => damage.Claculate(target));

            totalDamage += phy + fire + ice + holy + posion;

            target.Hp -= totalDamage;

            KillCheck();

            var sb = new StringBuilder();
            sb.AppendLine($"[Skill Calculator] 技能结算Log - 直接伤害")
              .AppendLine($"攻击者: {attacker.GetGameObject().name},使用了技能{skill.SkillName}")
              .AppendLine($"受击者: {target.GetGameObject().name}")
              .AppendLine($"造成总伤害: {totalDamage}")
              .AppendLine($"受击者剩余血量: {target.Hp}")
              .AppendLine($"具体伤害明细:")
              .AppendLine($"物理伤害: {phy}")
              .AppendLine($"火伤害: {fire}")
              .AppendLine($"冰伤害: {ice}")
              .AppendLine($"神圣伤害: {holy}")
              .AppendLine($"毒伤害: {posion}");
            GameFrameworkLog.Info(sb.ToString());

            CalculateEffects();
        }
        private void CalculateEffects()
        {
            foreach (var effect in immediateEffects)
            {
                effect.Effect();

#if UNITY_EDITOR
                var sb = new StringBuilder();
                sb.AppendLine($"[Skill Calculator] 技能结算Log - 立即效果")
                  .AppendLine($"攻击者: {attacker.GetGameObject().name},使用了技能{skill.SkillName}")
                  .AppendLine($"受击者: {target.GetGameObject().name}")
                  .AppendLine($"效果: {effect.Effect.Method.Name}");
                GameFrameworkLog.Info(sb.ToString());
#endif
            }
            KillCheck();

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

                        KillCheck();

#if UNITY_EDITOR
                        var sb = new StringBuilder();
                        sb.AppendLine($"[Skill Calculator] 技能结算Log - 持续效果")
                          .AppendLine($"攻击者: {attacker.GetGameObject().name},使用了技能{skill.SkillName}")
                          .AppendLine($"受击者: {target.GetGameObject().name}")
                          .AppendLine($"效果: {effect.Effect.Method.Name}")
                          .AppendLine($"剩余时间: {remainingDuration}");
                        GameFrameworkLog.Info(sb.ToString());
#endif
                    }
                });
            }

            foreach (var effect in timedEffects)
            {
                Task.Run(async () =>
                {
                    await Task.Delay((int)(effect.Delay * 1000)); // 转换为毫秒
                    effect.Effect();

                    KillCheck();

#if UNITY_EDITOR
                    var sb = new StringBuilder();
                    sb.AppendLine($"[Skill Calculator] 技能结算Log - 定时效果")
                      .AppendLine($"攻击者: {attacker.GetGameObject().name},使用了技能{skill.SkillName}")
                      .AppendLine($"受击者: {target.GetGameObject().name}")
                      .AppendLine($"效果: {effect.Effect.Method.Name}")
                      .AppendLine($"延迟: {effect.Delay}");
                    GameFrameworkLog.Info(sb.ToString());
#endif
                });
            }
        }
        private bool KillCheck()
        {
            if (!target.IsAlive())
            {
                attacker.OnKillSomebody(target);
                return true;
            }

            return false;
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
        private Skill skill;
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
