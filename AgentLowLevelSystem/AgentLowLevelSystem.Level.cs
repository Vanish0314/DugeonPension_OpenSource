using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CrashKonijn.Agent.Core;
using Dungeon.Character.Hero;
using Dungeon.DungeonGameEntry;
using Dungeon.SkillSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using static UnityGameFramework.Runtime.DebuggerComponent;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem, ICombatable
    {
        public void GetExperience(int exp)
        {
            m_Properties.currentExp += exp;

            var calculator = DungeonGameEntry.DungeonGameEntry.DungeonResultCalculator;

            var upgradeRule = calculator.UpgradeExperienceNeedRule;
            int currentLevel = m_Properties.currentLevel;
            int currentExp = m_Properties.currentExp;

            while (currentLevel < 20)
            {
                int requiredExp = upgradeRule.GetExpForLevel(currentLevel + 1);
                if (currentExp >= requiredExp)
                {
                    currentExp -= requiredExp;
                    currentLevel++;
                }
                else
                {
                    break;
                }
            }

            UnlockNewSkills(m_Properties.currentLevel, currentLevel);

            m_Properties.currentLevel = currentLevel;
            m_Properties.currentExp = currentExp;
        }
        private void UnlockNewSkills(int fromLevel, int toLevel)
        {
            if (m_LevelSkillData == null) return;

            var levelSkillMap = new Dictionary<int, List<SkillData>>
            {
                { 5,  m_LevelSkillData.skillsInLevel5 },
                { 10, m_LevelSkillData.skillsInLevel10 },
                { 15, m_LevelSkillData.skillsInLevel15 },
                { 20, m_LevelSkillData.skillsInLevel20 }
            };

            levelSkillMap
                .Where(kv => fromLevel < kv.Key && toLevel >= kv.Key)
                .SelectMany(kv => kv.Value)
                .Where(skill => skill != null && !m_SkillDict.ContainsKey(skill.name))
                .ToList()
                .ForEach(skill => m_SkillDict[skill.name] = skill);
        }

        [DungeonGridWindow("升级")]
        public static void Temp()
        {
            var hero = DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.currentHeroTeam.FirstOrDefault<HeroEntityBase>();

            hero.GetComponent<AgentLowLevelSystem>().GetExperience(100);
        }

        [Header("属性相关")]
        [SerializeField] private HeroProperties m_Properties;
        [SerializeField] private LevelSkillData m_LevelSkillData;
        private List<SkillData> CurrentOwnedSkills()
        {
            var level = m_Properties.currentLevel;
            List<SkillData> ownedSkills = new();

            if (m_LevelSkillData == null)
                return ownedSkills;

            if (level >= 1)
                ownedSkills.AddRange(m_LevelSkillData.skillsInLevel1);
            if (level >= 5)
                ownedSkills.AddRange(m_LevelSkillData.skillsInLevel5);
            if (level >= 10)
                ownedSkills.AddRange(m_LevelSkillData.skillsInLevel10);
            if (level >= 15)
                ownedSkills.AddRange(m_LevelSkillData.skillsInLevel15);
            if (level >= 20)
                ownedSkills.AddRange(m_LevelSkillData.skillsInLevel20);

            return ownedSkills;
        }
        private Dictionary<string, SkillData> m_SkillDict = new();
    }

    public class LevelSkillData : ScriptableObject
    {
        [Header("注意,掌握技能需要与goap graph对应")]
        [LabelText("1级时能掌握的技能")] public List<SkillData> skillsInLevel1;
        [LabelText("5级时能额外掌握的技能")] public List<SkillData> skillsInLevel5;
        [LabelText("10级时能额外掌握的技能")] public List<SkillData> skillsInLevel10;
        [LabelText("15级时能额外掌握的技能")] public List<SkillData> skillsInLevel15;
        [LabelText("20级时能额外掌握的技能")] public List<SkillData> skillsInLevel20;
    }
}
