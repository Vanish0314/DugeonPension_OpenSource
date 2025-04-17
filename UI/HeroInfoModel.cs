using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class HeroInfoModel : MonoBehaviour
    {
        public static HeroInfoModel Instance { get; private set; }

        private Dictionary<HeroType, HeroInfoData> heroDataDict;
        public HeroType CurrentHeroType { get; private set; } = HeroType.Warrior; // 默认战士
        
        // 资源加载路径配置
        private const string HERO_DATA_PATH = "HeroDatas";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeData();
            }
        }

        private void InitializeData()
        {
            heroDataDict = new Dictionary<HeroType, HeroInfoData>();
            
            // 动态加载所有HeroInfoData资源
            var loadedData = Resources.LoadAll<HeroInfoData>(HERO_DATA_PATH);
            
            if (loadedData == null || loadedData.Length == 0)
            {
                Debug.LogError($"未在Resources/{HERO_DATA_PATH}目录下找到任何英雄数据！");
                return;
            }

            foreach (var data in loadedData)
            {
                if (heroDataDict.ContainsKey(data.heroType))
                {
                    Debug.LogError($"英雄类型重复: {data.heroType}");
                    continue;
                }
                heroDataDict.Add(data.heroType, data);
            }
        }

        public HeroInfoData GetCurrentHeroData()
        {
            if (heroDataDict.TryGetValue(CurrentHeroType, out var data))
            {
                return data;
            }
            Debug.LogError($"未找到{CurrentHeroType}类型的英雄数据");
            return null;
        }

        public void SwitchHeroType(HeroType type)
        {
            if (!heroDataDict.ContainsKey(type))
            {
                Debug.LogError($"无效的英雄类型: {type}");
                return;
            }
            CurrentHeroType = type;
        }

        // 获取所有可用英雄类型（用于动态生成按钮）
        public List<HeroType> GetAvailableHeroTypes()
        {
            return new List<HeroType>(heroDataDict.Keys);
        }
    }
}
