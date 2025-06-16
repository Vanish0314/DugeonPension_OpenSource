using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CrashKonijn.Agent.Core;
using Dungeon.Character;
using Dungeon.DungeonCalculator;
using Dungeon.DungeonGameEntry;
using Dungeon.SkillSystem;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using static UnityGameFramework.Runtime.DebuggerComponent;

namespace Dungeon.Character
{
    [CreateAssetMenu(fileName = "新 勇者升级预设", menuName = "Hero/勇者升级预设")]
    public class HeroLevelPresetData : ScriptableObject
    {
        [Serializable]
        public class LevelPreset
        {
            [LabelText("等级")]
            [ValueDropdown("@HeroLevelPresetData.ValidLevels")]
            [ValidateInput("@HeroLevelPresetData.IsValidLevel(level)", "等级只能是 5, 10, 15, 20")]
            public int level;
            public DndSkillData dndSkillData;
            public CombatorData combatorData;
        }

        public static readonly int[] ValidLevels = { 5, 10, 15, 20 };
        public static bool IsValidLevel(int level) => ValidLevels.Contains(level);
        public List<LevelPreset> levelPresets;
        private Dictionary<int, LevelPreset> _presetMap;

        public void Init()
        {
            _presetMap = levelPresets.ToDictionary(p => p.level, p => p);
        }

        public LevelPreset GetPresetForLevel(int level)
        {
            if (_presetMap == null)
                Init();
            _presetMap.TryGetValue(level, out var preset);
            return preset;
        }

        [OnValidate]
        private void ValidatePresets()
        {
            levelPresets = levelPresets
                .Where(p => IsValidLevel(p.level))
                .GroupBy(p => p.level)
                .Select(g => g.First())
                .OrderBy(p => p.level)
                .ToList();

#if UNITY_EDITOR
            var configuredLevels = levelPresets.Select(p => p.level).ToHashSet();
            var missing = ValidLevels.Except(configuredLevels).ToList();
            if (missing.Count > 0)
            {
                Debug.LogWarning($"[勇者升级预设]缺失等级：{string.Join(",", missing)}", this);
            }
#endif
        }

        public List<int> GetPhaseLevels() => ValidLevels.ToList();
    }

    internal class OnValidateAttribute : Attribute
    {
    }


    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem, ICombatable
    {
        [Header("升级相关")]
        [ShowInInspector, LabelText("勇者升级预设")] private HeroLevelPresetData m_LevelPresetData;
        [ShowInInspector, ReadOnly, HideLabel]
        private string LevelInfo => $"当前等级: {m_Properties.combatorData.currentLevel}";
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

                    UpGradeStrengthenProperties(currentLevel);

                    // 升级改背包属性
                    m_Backpack.UpdateResource(resourceRule, currentLevel);

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
        private void UpGradeStrengthenProperties(int level)
        {
            var dnd = m_Properties.dndSkillData;
            var combat = m_Properties.combatorData;

            var nextPhase = GetNextPhaseLevel(level);
            var nextPreset = m_LevelPresetData?.GetPresetForLevel(nextPhase);

            if (level % 5 == 0)
            {
                var preset = m_LevelPresetData?.GetPresetForLevel(level);
                if (preset != null)
                {
                    ApplyPresetWithClamp(dnd, combat, preset.dndSkillData, preset.combatorData);
                }
            }
            else
            {
                List<Action> dndGrowList = new();

                if (nextPreset == null || dnd.Strength < nextPreset.dndSkillData.Strength) dndGrowList.Add(() => dnd.Strength++);
                if (nextPreset == null || dnd.Dexterity < nextPreset.dndSkillData.Dexterity) dndGrowList.Add(() => dnd.Dexterity++);
                if (nextPreset == null || dnd.Constitution < nextPreset.dndSkillData.Constitution) dndGrowList.Add(() => dnd.Constitution++);
                if (nextPreset == null || dnd.Intelligence < nextPreset.dndSkillData.Intelligence) dndGrowList.Add(() => dnd.Intelligence++);
                if (nextPreset == null || dnd.Wisdom < nextPreset.dndSkillData.Wisdom) dndGrowList.Add(() => dnd.Wisdom++);
                if (nextPreset == null || dnd.Charisma < nextPreset.dndSkillData.Charisma) dndGrowList.Add(() => dnd.Charisma++);

                int count = UnityEngine.Random.Range(1, 4);
                for (int i = 0; i < count && dndGrowList.Count > 0; i++)
                {
                    var index = UnityEngine.Random.Range(0, dndGrowList.Count);
                    dndGrowList[index].Invoke();
                    dndGrowList.RemoveAt(index); // 防止重复增长
                }

                if (nextPreset != null)
                {
                    if (combat.maxHp < nextPreset.combatorData.maxHp)
                        combat.maxHp += UnityEngine.Random.Range(1, 6);

                    if (combat.maxMp < nextPreset.combatorData.maxMp)
                        combat.maxMp += UnityEngine.Random.Range(1, 4);

                    if (combat.attackSpeed < nextPreset.combatorData.attackSpeed)
                        combat.attackSpeed += 0.1f;
                }
            }
        }

        private int GetNextPhaseLevel(int currentLevel)
        {
            if (currentLevel < 5) return 5;
            if (currentLevel < 10) return 10;
            if (currentLevel < 15) return 15;
            if (currentLevel < 20) return 20;
            return 20;
        }

        private void ApplyPresetWithClamp(DndSkillData currentDnd, CombatorData currentCombat, DndSkillData targetDnd, CombatorData targetCombat)
        {
            currentDnd.Strength = Mathf.Max(currentDnd.Strength, targetDnd.Strength);
            currentDnd.Dexterity = Mathf.Max(currentDnd.Dexterity, targetDnd.Dexterity);
            currentDnd.Constitution = Mathf.Max(currentDnd.Constitution, targetDnd.Constitution);
            currentDnd.Intelligence = Mathf.Max(currentDnd.Intelligence, targetDnd.Intelligence);
            currentDnd.Wisdom = Mathf.Max(currentDnd.Wisdom, targetDnd.Wisdom);
            currentDnd.Charisma = Mathf.Max(currentDnd.Charisma, targetDnd.Charisma);

            currentCombat.maxHp = Mathf.Max(currentCombat.maxHp, targetCombat.maxHp);
            currentCombat.maxMp = Mathf.Max(currentCombat.maxMp, targetCombat.maxMp);
            currentCombat.attackSpeed = Mathf.Max(currentCombat.attackSpeed, targetCombat.attackSpeed);
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

        public void InitializeBackpack()
        {
            if (resourceRule == null || specialResourcePack == null)
            {
                GameFrameworkLog.Error("[AgentLowLevelSystem]: 没设置背包相关");
                return;
            }
            m_Backpack.Initialize(resourceRule, 1, specialResourcePack);
        }

        [DungeonGridWindow("给勇者升级")]
        public static void Temp()
        {
            var hero = DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.currentBehavouringHeroTeam.FirstOrDefault<HeroEntityBase>();

            hero.GetComponent<AgentLowLevelSystem>().GetExperience(100);
        }

        [Header("属性相关")]
        public HeroProperties m_Properties;
        [SerializeField] private LevelSkillData m_LevelSkillData;
    
        public List<SkillData> CurrentOwnedSkills()
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

        [Header("背包相关")]
        [SerializeField] private DungeonHeroResourceRule resourceRule;
        [SerializeField] private SpecialResourcePack specialResourcePack;
        public HeroBackpack m_Backpack = new HeroBackpack();
    }

    public class HeroBackpack
    {
        // 核心资源
        public int Gold { get; private set; }
        public int ExpOrb { get; private set; }

        // 特殊资源
        public Dictionary<ResourceType, int> SpecialResources { get; } = new();
        public Dictionary<CropType, int> CropSeeds { get; } = new();

        public HeroBackpack(
            DungeonHeroResourceRule resourceRule = null,
            int heroLevel = 1,
            SpecialResourcePack resourcePack = null,
            int minBundles = 3,
            int maxBundles = 5)
        {
            if (resourceRule != null && resourcePack != null)
            {
                Initialize(resourceRule, heroLevel, resourcePack, minBundles, maxBundles);
            }
        }

        // 初始化背包
        public void Initialize(
            DungeonHeroResourceRule resourceRule,
            int heroLevel,
            SpecialResourcePack resourcePack,
            int minBundles = 3,
            int maxBundles = 5)
        {
            // 设置核心资源
            var (gold, expOrb) = resourceRule.GetResourcesForLevel(heroLevel);
            Gold = gold;
            ExpOrb = expOrb;

            // 随机选择资源包数量
            int bundleCount = UnityEngine.Random.Range(minBundles, maxBundles + 1);

            // 随机选择资源包并合并
            for (int i = 0; i < bundleCount; i++)
            {
                var selectedBundle = resourcePack.Bundles[UnityEngine.Random.Range(0, resourcePack.Bundles.Count)];
                AddBundleToBackpack(selectedBundle);
            }
        }

        private void AddBundleToBackpack(SpecialResourcePack.ResourceBundle bundle)
        {
            // 添加普通资源
            foreach (var resource in bundle.Resources)
            {
                if (SpecialResources.ContainsKey(resource.Type))
                {
                    SpecialResources[resource.Type] += resource.Amount;
                }
                else
                {
                    SpecialResources[resource.Type] = resource.Amount;
                }
            }

            // 添加作物种子
            foreach (var seed in bundle.Seeds)
            {
                if (CropSeeds.ContainsKey(seed.CropType))
                {
                    CropSeeds[seed.CropType] += seed.Amount;
                }
                else
                {
                    CropSeeds[seed.CropType] = seed.Amount;
                }
            }
        }

        public void UpdateResource(DungeonHeroResourceRule resourceRule,
            int heroLevel)
        {
            // 设置核心资源
            var (gold, expOrb) = resourceRule.GetResourcesForLevel(heroLevel);
            Gold = gold;
            ExpOrb = expOrb;
        }

        public void GatherResource()
        {
            ResourceModel.Instance.Gold += Gold;
            ResourceModel.Instance.ExpBall += ExpOrb;
            foreach (var resource in SpecialResources)
            {
                ResourceModel.Instance.GatherResource(resource.Key, resource.Value);
            }
            foreach (var seed in CropSeeds)
            {
                ResourceModel.Instance.AddSeed(seed.Key, seed.Value);
            }
        }

        public void GatherHalfResource()
        {
            ResourceModel.Instance.Gold += Gold / 2;
            ResourceModel.Instance.ExpBall += ExpOrb / 2;
            foreach (var resource in SpecialResources)
            {
                ResourceModel.Instance.GatherResource(resource.Key, resource.Value / 2);
            }
            foreach (var seed in CropSeeds)
            {
                ResourceModel.Instance.AddSeed(seed.Key, seed.Value / 2);
            }
        }

        public void CalculateAllResource()
        {
            
        }
    }
}
