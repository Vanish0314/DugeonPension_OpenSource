using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.DUngeonCalculator
{
    public class DungeonResultCalculator : MonoBehaviour
    {
        public DungeonExperienceGetByLevelRule ExperienceGetByLevelRule { get => experienceGetByLevelRule; }
        public DungeonUpgardeExperienceNeedRule UpgradeExperienceNeedRule { get => upgradeExperienceNeedRule; }
        private DungeonUpgardeExperienceNeedRule upgradeExperienceNeedRule;
        private DungeonExperienceGetByLevelRule experienceGetByLevelRule;
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
                LevelExpList.Add(new LevelExpPair { Level = i, RequiredExp = 100 }); // 默认经验可自定义
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
    [CreateAssetMenu(fileName = "新经验获取规则", menuName = "DungeonRule/经验获取规则")]
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

}
