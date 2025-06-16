using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.DungeonCalculator
{
    public class DungeonResultCalculator : MonoBehaviour
    {
        public int GetUpgradeExperienceNeed(int level)
        {
            if (upgradeExperienceNeedRule == null)
            {
                GameFrameworkLog.Error($"[DungeonResultCalculator] 地牢升级经验规则未设置");
                return 114514;
            }

            return upgradeExperienceNeedRule.GetExpForLevel(level);
        }
        public int GetDropExpForLevel(int level)
        {
            if (experienceGetByLevelRule == null)
            {
                GameFrameworkLog.Error($"[DungeonResultCalculator] 地牢等级经验获取规则未设置");
                return 114514;
            }

            return experienceGetByLevelRule.GetDropExpForLevel(level);
        }
        public int GetExpForRoom()
        {
            if (experienceGetByRoomRule == null)
            {
                GameFrameworkLog.Error($"[DungeonResultCalculator] 地牢房间经验获取规则未设置");
                return 114514;
            }

            return experienceGetByRoomRule.experiencePerRoom;
        }

        public DungeonExperienceGetByLevelRule ExperienceGetByLevelRule { get => experienceGetByLevelRule; }
        public DungeonUpgardeExperienceNeedRule UpgradeExperienceNeedRule { get => upgradeExperienceNeedRule; }
        public DungeonExperienceGetByRoomRule ExperienceGetByRoomRule { get => experienceGetByRoomRule; }

        void Start()
        {
            if (upgradeExperienceNeedRule == null)
            {
                GameFrameworkLog.Error($"[DungeonResultCalculator] 地牢升级经验规则未设置");
            }

            if (experienceGetByLevelRule == null)
            {
                GameFrameworkLog.Error($"[DungeonResultCalculator] 地牢等级经验获取规则未设置");
            }

            if (experienceGetByRoomRule == null)
            {
                GameFrameworkLog.Error($"[DungeonResultCalculator] 地牢房间经验获取规则未设置");
            }
        }

        [SerializeField] private DungeonUpgardeExperienceNeedRule upgradeExperienceNeedRule;
        [SerializeField] private DungeonExperienceGetByLevelRule experienceGetByLevelRule;
        [SerializeField] private DungeonExperienceGetByRoomRule experienceGetByRoomRule;
    }
    public class DungeonCalculationResult
    {

    }

    [CreateAssetMenu(fileName = "新升级规则", menuName = "DungeonRule/升级规则")]
    public class DungeonUpgardeExperienceNeedRule : ScriptableObject
    {
        [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
        [ValidateInput("@ValidateLevels()", "必须配置 2~20 级，每级唯一")]
        [LabelText("升级所需经验")]
        public List<LevelExpPair> LevelExpList = new();

        [Button("自动生成 2~20 级空数据"), GUIColor(0.3f, 0.8f, 1f)]
        private void GenerateDefaultLevels()
        {
            LevelExpList = new List<LevelExpPair>();
            for (int i = 2; i <= 20; i++)
            {
                LevelExpList.Add(new LevelExpPair { Level = i, RequiredExp = 100 });
            }
        }

        private bool ValidateLevels()
        {
            if (LevelExpList.Count != 19) return false;

            var expected = Enumerable.Range(2, 19);
            var actual = LevelExpList.Select(e => e.Level).OrderBy(l => l);
            return expected.SequenceEqual(actual);
        }

        [Serializable]
        public class LevelExpPair
        {
            [HorizontalGroup("Row", 50)]
            [LabelText("等级")]
            [Range(2, 20)]
            public int Level;

            [HorizontalGroup("Row")]
            [LabelText("所需经验")]
            [MinValue(1)]
            public int RequiredExp;
        }

        public int GetExpForLevel(int level)
        {
            var match = LevelExpList.Find(x => x.Level == level);
            return match != null ? match.RequiredExp : int.MaxValue;
        }
    }
    [CreateAssetMenu(fileName = "新等级经验获取规则", menuName = "DungeonRule/等级经验获取规则")]
    public class DungeonExperienceGetByLevelRule : ScriptableObject
    {
        [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
        [ValidateInput("@ValidateLevels()", "必须配置 1~20 级，每级唯一")]
        [LabelText("每等级经验掉落")]
        public List<LevelExpPair> LevelDropList = new();

        [Button("自动生成 1~20 级空数据"), GUIColor(0.4f, 1f, 0.5f)]
        private void GenerateDefaultLevels()
        {
            LevelDropList = new List<LevelExpPair>();
            for (int i = 1; i <= 20; i++)
            {
                LevelDropList.Add(new LevelExpPair { Level = i, DropExp = 50 });
            }
        }

        private bool ValidateLevels()
        {
            if (LevelDropList.Count != 20) return false;

            var expected = Enumerable.Range(1, 20);
            var actual = LevelDropList.Select(e => e.Level).OrderBy(l => l);
            return expected.SequenceEqual(actual);
        }

        [Serializable]
        public class LevelExpPair
        {
            [HorizontalGroup("Row", 50)]
            [LabelText("等级")]
            [Range(1, 20)]
            public int Level;

            [HorizontalGroup("Row")]
            [LabelText("掉落经验")]
            [MinValue(0)]
            public int DropExp;
        }

        public int GetDropExpForLevel(int level)
        {
            var match = LevelDropList.Find(x => x.Level == level);
            return match != null ? match.DropExp : 0;
        }
    }

    [CreateAssetMenu(fileName = "新地牢经验获取规则", menuName = "DungeonRule/地牢房间经验获取规则")]
    public class DungeonExperienceGetByRoomRule : ScriptableObject
    {
        [LabelText("每一个房间的经验")] public int experiencePerRoom;
    }

    [System.Serializable]
    public class DungeonHeroResourceRule
    {
        [SerializeField]
        public List<LevelResourcePair> LevelResourceList = new();

        [System.Serializable]
        public class LevelResourcePair
        {
            public int Level;
            public int Gold;
            public int ExpOrb;
        }

        public (int gold, int expOrb) GetResourcesForLevel(int level)
        {
            var match = LevelResourceList.Find(x => x.Level == level);
            return match != null ? (match.Gold, match.ExpOrb) : (0, 0);
        }
    }


    [System.Serializable]
    public class SpecialResourcePack
    {
        [System.Serializable]
        public class ResourceItem
        {
            public ResourceType Type;
            public int Amount;
        }

        [System.Serializable]
        public class CropSeedItem
        {
            public CropType CropType;
            public int Amount;
        }

        [System.Serializable]
        public class ResourceBundle
        {
            public string BundleName;
            public List<ResourceItem> Resources = new();
            public List<CropSeedItem> Seeds = new();
        }

        [SerializeField]
        public List<ResourceBundle> Bundles = new();
    }

}
