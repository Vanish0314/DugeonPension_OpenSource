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
            BumpGetExperienceBubble(exp);

            m_Properties.currentExp += exp;

            var calculator = DungeonGameEntry.DungeonGameEntry.DungeonResultCalculator;

            int currentLevel = m_Properties.combatorData.currentLevel;
            int currentExp = m_Properties.currentExp;

            while (currentLevel < 20)
            {
                int requiredExp = calculator.GetUpgradeExperienceNeed(currentLevel + 1);
                if (currentExp >= requiredExp)
                {
                    currentExp -= requiredExp;
                    currentLevel++;

                    BumpLevelUpBubble(currentLevel);
                }
                else
                {
                    break;
                }
            }

            UnlockNewSkills(m_Properties.combatorData.currentLevel, currentLevel);

            m_Properties.combatorData.currentLevel = currentLevel;
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

        [DungeonGridWindow("给勇者升级")]
        public static void Temp()
        {
            var hero = DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.currentBehavouringHeroTeam.FirstOrDefault<HeroEntityBase>();

            hero.GetComponent<AgentLowLevelSystem>().GetExperience(100);
        }

        [Header("属性相关")]
        [SerializeField] private HeroProperties m_Properties;
        [SerializeField] private LevelSkillData m_LevelSkillData;
        private List<SkillData> CurrentOwnedSkills()
        {
            var level = m_Properties.combatorData.currentLevel;
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
}
