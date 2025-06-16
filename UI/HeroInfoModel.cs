using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class HeroInfoModel : MonoBehaviour
    {
        public static HeroInfoModel Instance { get; private set; }

        public List<HeroEntityBase> ActiveHeroes { get; private set; } = new List<HeroEntityBase>();
        public List<HeroEntityBase> CapturedHeroes { get; private set; } = new List<HeroEntityBase>();

        [SerializeField] private List<HeroVisualData> heroVisualDatas = new List<HeroVisualData>();
        private Dictionary<string, HeroVisualData> heroVisualDict = new Dictionary<string, HeroVisualData>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeVisualData();
            }
        }

        private void InitializeVisualData()
        {
            heroVisualDict.Clear();
            foreach (var data in heroVisualDatas)
            {
                if (!string.IsNullOrEmpty(data.heroName))
                {
                    heroVisualDict[data.heroName] = data;
                }
            }
        }

        public HeroVisualData GetVisualData(string heroName)
        {
            return heroVisualDict.GetValueOrDefault(heroName);
        }
        
        private void Start()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnNewHeroTeamSpawnEvent.EventId, OnNewHeroTeamSpawn);
            UpdateHeroData();
        }

        private void OnDestroy()
        {
            if (DungeonGameEntry.DungeonGameEntry.Event != null)
            {
                DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnNewHeroTeamSpawnEvent.EventId, OnNewHeroTeamSpawn);
            }
        }

        private void OnNewHeroTeamSpawn(object sender, GameEventArgs e)
        {
            UpdateHeroData();
        }

        public void UpdateHeroData()
        {
            var guildSystem = DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem;
            ActiveHeroes = new List<HeroEntityBase>(guildSystem.GetCurrentGameProgressingHeroTeam());
            CapturedHeroes = new List<HeroEntityBase>(guildSystem.GetCurrentBeCapturedHeroInTeam());
        }
    }
    
    [System.Serializable]
    public class HeroVisualData
    {
        public string heroName; // 使用英雄名称作为键
        public Sprite portrait; // 英雄立绘
        public Sprite buttonImage; // 按钮图片
    }

}
