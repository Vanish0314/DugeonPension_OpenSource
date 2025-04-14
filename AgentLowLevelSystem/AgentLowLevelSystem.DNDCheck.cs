using System;
using System.Collections.Generic;
using System.Linq;
using CrashKonijn.Agent.Core;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem,ICombatable
    {
        private void InitDNDSystem()
        {
            m_DndSystem = new DndSystem();
        }

        public DndCheckResult DndCheck(DndCheckTarget target)
        {
            var result = m_DndSystem.DNDCheck(this, target, out string checkResult);
            BumpDiceCheckBubbule(checkResult);
            return result;
        }

        public void AddDndModifier(DndAbility ability, string name, int value) => m_DndSystem.AddModifier(ability, name, value);

        private DndSystem m_DndSystem;
        // [SerializeField] private DndAbilityConfig m_DndAbility;
    }


#region DND System
    public class DndSystem
    {
        public static Dictionary<DndSkill, DndAbility> SkillToAbility = new()
        {
            { DndSkill.Acrobatics, DndAbility.Dexterity },
            { DndSkill.AnimalHandling, DndAbility.Wisdom },
            { DndSkill.Arcana, DndAbility.Intelligence },
            { DndSkill.Athletics, DndAbility.Strength },
            { DndSkill.Deception, DndAbility.Charisma },
            { DndSkill.History, DndAbility.Intelligence },
            { DndSkill.Insight, DndAbility.Wisdom },
            { DndSkill.Intimidation, DndAbility.Charisma },
            { DndSkill.Investigation, DndAbility.Intelligence },
            { DndSkill.Medicine, DndAbility.Wisdom },
            { DndSkill.Nature, DndAbility.Intelligence },
            { DndSkill.Perception, DndAbility.Wisdom },
            { DndSkill.Performance, DndAbility.Charisma },
            { DndSkill.Persuasion, DndAbility.Charisma },
            { DndSkill.Religion, DndAbility.Intelligence },
            { DndSkill.SleightOfHand, DndAbility.Dexterity },
            { DndSkill.Stealth, DndAbility.Dexterity },
            { DndSkill.Survival, DndAbility.Wisdom }
        };

        public DndCheckResult DNDCheck(AgentLowLevelSystem checker, DndCheckTarget target, out string checkResult)
        {
            int rollResult = UnityEngine.Random.Range(1, 21);
            GameFrameworkLog.Info($"[AgentLowLevelSystem] DND Check: {target.Name} (DC: {target.DifficultyClass}, Skill: {target.RequiredSkill}) rolled {rollResult}");

            SkillToAbility.TryGetValue(target.RequiredSkill, out DndAbility ability);
#if UNITY_EDITOR
            if (!SkillToAbility.TryGetValue(target.RequiredSkill, out DndAbility temp))
            {
                throw new System.Exception($"Unknown skill: {target.RequiredSkill}");
            }
#endif

            List<Dictionary<string, int>> modifiers = GetAbilityModifiers(ability);

            checkResult = $"[AgentLowLevelSystem] [DND Check Result] \n Checker: {checker.gameObject.name} \n DND Check: {target.Name} \n (DC: {target.DifficultyClass} \n Check Skill: {target.RequiredSkill}) \n modifiers: {modifiers.Select(dict => string.Join(", ", dict.Select(kv => $"{kv.Key}: {kv.Value}")))}";

            GameFrameworkLog.Info(checkResult);

            return new DndCheckResult(checker, target, rollResult, modifiers);
        }

        public void AddModifier(DndAbility ability, string name, int value, float duration = 0)
        {
            switch (ability)
            {
                case DndAbility.Strength:
                    StrengthModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                case DndAbility.Dexterity:
                    DexterityModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                case DndAbility.Constitution:
                    ConstitutionModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                case DndAbility.Intelligence:
                    IntelligenceModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                case DndAbility.Wisdom:
                    WisdomModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                case DndAbility.Charisma:
                    CharismaModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                default:
                    throw new Exception("Unknown ability");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="timesForUsage">可以被使用的次数</param>
        /// <exception cref="System.Exception"></exception>
        public void AddModifier(DndAbility ability, string name, int value, UInt32 timesForUsage)
        {
            switch (ability)
            {
                case DndAbility.Strength:
                    StrengthModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                case DndAbility.Dexterity:
                    DexterityModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                case DndAbility.Constitution:
                    ConstitutionModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                case DndAbility.Intelligence:
                    IntelligenceModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                case DndAbility.Wisdom:
                    WisdomModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                case DndAbility.Charisma:
                    CharismaModifiers.Add(new Dictionary<string, int>() { { name, value } });
                    break;
                default:
                    throw new System.Exception("Unknown ability");
            }
        }

        public List<Dictionary<string, int>> GetAbilityModifiers(DndAbility ability)
        {
            return ability switch
            {
                DndAbility.Strength => StrengthModifiers,
                DndAbility.Dexterity => DexterityModifiers,
                DndAbility.Constitution => ConstitutionModifiers,
                DndAbility.Intelligence => IntelligenceModifiers,
                DndAbility.Wisdom => WisdomModifiers,
                DndAbility.Charisma => CharismaModifiers,
                _ => new List<Dictionary<string, int>>() // 默认空列表
            };
        }
        private List<Dictionary<string, int>> StrengthModifiers = new();
        private List<Dictionary<string, int>> DexterityModifiers = new();
        private List<Dictionary<string, int>> ConstitutionModifiers = new();
        private List<Dictionary<string, int>> IntelligenceModifiers = new();
        private List<Dictionary<string, int>> WisdomModifiers = new();
        private List<Dictionary<string, int>> CharismaModifiers = new();
    }

    /// <summary>
    /// 能力
    /// ref: https://dnd5e.wikidot.com/skills
    /// </summary>
    public enum DndAbility
    {
        Strength, // 力量
        Dexterity, // 敏捷
        Constitution, // 体质
        Intelligence, // 智力
        Wisdom, // 知识
        Charisma // 魅力
    }

    /// <summary>
    /// 技能
    /// ref: https://dnd5e.wikidot.com
    /// </summary>
    public enum DndSkill
    {
        [LabelText("跳跃")] Acrobatics,  // 跳跃
        [LabelText("动物手段")] AnimalHandling, // 动物手段
        [LabelText("秘术")] Arcana, // 秘术
        [LabelText("运动")] Athletics, // 运动
        [LabelText("诈骗")] Deception, // 诈骗
        [LabelText("历史")] History, // 历史
        [LabelText("洞察")] Insight, // 洞察
        [LabelText("激怒")] Intimidation, // 激怒
        [LabelText("侦查")] Investigation, // 侦查
        [LabelText("医术")] Medicine, // 医术
        [LabelText("自然")] Nature, // 自然
        [LabelText("察觉")] Perception, // 察觉
        [LabelText("表演")] Performance, // 表演
        [LabelText("说服")] Persuasion, // 说服
        [LabelText("宗教")] Religion, // 宗教
        [LabelText("手艺")] SleightOfHand, // 手艺
        [LabelText("隐匿")] Stealth, // 隐匿
        [LabelText("生存")] Survival // 生存
    }

    public struct DndCheckResult
    {
        public readonly AgentLowLevelSystem Checker { get; }  // 进行检定的角色
        public readonly DndCheckTarget Target { get; }  // 目标，例如陷阱、法术
        public readonly int RollResult { get; }  // d20 投掷结果
        public readonly List<Dictionary<string, int>> Modifiers { get; }  // 所有影响检定值的修饰符（如技能、属性、道具等）
        public readonly int Total { get;}  // 总检定值 (RollResult + 修正值)
        public readonly bool Succeed { get => Total >= Target.DifficultyClass; }  // 是否成功
        public readonly bool IsCritical { get => Total >= 20; }  // 是否是会心成功（掷出 20）
        public readonly bool IsFailure { get => Total <= 1; }  // 是否是大失败（掷出 1）

        public DndCheckResult(AgentLowLevelSystem checker, DndCheckTarget target, int rollResult, List<Dictionary<string, int>> modifiers, int total)
        {
            Checker = checker;
            Target = target;
            RollResult = rollResult;
            Modifiers = modifiers;
            Total = total;
        }
        public DndCheckResult(AgentLowLevelSystem checker, DndCheckTarget target, int rollResult, List<Dictionary<string, int>> modifiers)
        {
            Checker = checker;
            Target = target;
            RollResult = rollResult;
            Modifiers = modifiers;
            Total = rollResult + modifiers.SelectMany(dict => dict.Values).Sum();
        }

        public override string ToString()
        {
            string status = IsCritical ? "Critical Success!" :
                            IsFailure ? "Critical Failure!" :
                            Succeed ? "Success!" : "Failure!";

            string modifiersText = Modifiers.Count > 0
                ? string.Join(", ", Modifiers.SelectMany(dict => dict.Select(kv => $"{kv.Key}: {kv.Value}")))
                : "None";

            return $"{Checker.gameObject.name} rolls {RollResult} + {Total - RollResult} = {Total} vs DC {Target.DifficultyClass} → {status}\n" +
                   $"Modifiers: {modifiersText}";
        }
    }

    public class ModifierBuilder
    {
        private List<Dictionary<string, int>> modifiers;

        public ModifierBuilder()
        {
            modifiers = new List<Dictionary<string, int>>();
        }

        public ModifierBuilder Add(Dictionary<string, int> source)
        {
            modifiers.Add(new Dictionary<string, int>(source));
            return this;
        }

        public ModifierBuilder Add(string name, int value)
        {
            if (modifiers.Count == 0)
            {
                modifiers.Add(new Dictionary<string, int>());
            }
            modifiers.Last()[name] = value;
            return this;
        }

        public int GetTotalBonus()
        {
            return modifiers.SelectMany(dict => dict.Values).Sum();
        }

        public List<Dictionary<string, int>> Build()
        {
            return modifiers;
        }

        public override string ToString()
        {
            return modifiers.Count > 0
                ? string.Join(", ", modifiers.SelectMany(dict => dict.Select(kv => $"{kv.Key}: {kv.Value}")))
                : "None";
        }
    }

#endregion

}
