using System;
using System.Collections.Generic;
using System.Data.Common;
using Dungeon.Character;
using Dungeon.Evnents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class FightSettlementController : MonoBehaviour
    {
        [SerializeField] private Button mContinueButton;
        [SerializeField] private Transform mHeroStatusContainer; // 英雄状态UI的父物体
        [SerializeField] private GameObject mHeroStatusPrefab; // 英雄状态UI预制体
        [SerializeField] private Transform mResourceContainer; // 资源状态UI的父物体
        [SerializeField] private GameObject mResourceItemPrefab; // 资源项UI预制体
        
        [SerializeField] private float hpReducePerHero = -5f;
        
        // 用于记录玩家获得的总资源
        private Dictionary<ResourceType, int> mTotalResources = new Dictionary<ResourceType, int>();
        private Dictionary<CropType, int> mTotalSeeds = new Dictionary<CropType, int>();
        
        private void OnEnable()
        {
            mContinueButton.onClick.AddListener(OnContinueButtonClicked);
            RefreshUI();
        }

        private void OnContinueButtonClicked()
        {
            GameEntry.Event.Fire(this, OnDungeonCalculationFinishedEvent.Create());
        }

        private void RefreshUI()
        {
            // 清除现有内容
            ClearContainer(mHeroStatusContainer);
            ClearContainer(mResourceContainer);
            
            ShowHeroStatus();
            ShowTotalResources();
        }

        private void ClearContainer(Transform container)
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }
        
        private void ShowHeroStatus()
        {
            List<HeroEntityBase> progressingHero =
                DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentGameProgressingHeroTeam();
            List<HeroEntityBase> aliveHero =
                DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentBehavouringHeroTeam();
            List<HeroEntityBase> beCapturedHero =
                DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentBeCapturedHeroInTeam();
            
            // 计算死亡勇者
            List<HeroEntityBase> deadHero = new List<HeroEntityBase>();
            foreach (var hero in progressingHero)
            {
                if (!aliveHero.Contains(hero) && !beCapturedHero.Contains(hero))
                {
                    deadHero.Add(hero);
                }
            }
            
            List<HeroEntityBase> allHero = new List<HeroEntityBase>();
            foreach (var hero in progressingHero)
            {
                allHero.Add(hero);
            }
            foreach (var hero in beCapturedHero)
            {
                allHero.Add(hero);
            }

            var mainHero = DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentMainHero();
            // 确保主角排在第一位
            allHero.Sort((x, y) => 
            {
                if (x == mainHero && y != mainHero) return -1;  // x是主英雄，y不是，x排前面
                if (x != mainHero && y == mainHero) return 1;   // y是主英雄，x不是，y排前面
                return 0;  // 其他情况保持原顺序
            });
            
            // 为每个英雄创建UI元素
            foreach (var hero in allHero)
            {
                var heroStatusObj = Instantiate(mHeroStatusPrefab, mHeroStatusContainer);
                heroStatusObj.GetComponent<Image>().sprite = hero.GetSprite();
                
                // 设置状态
                if (deadHero.Contains(hero))
                {
                    SetStatus("阵亡", Color.red, heroStatusObj);
                    AddHeroResourcesToTotal(hero.GetComponent<AgentLowLevelSystem>().m_Backpack,1);
                }
                else if (beCapturedHero.Contains(hero))
                {
                    SetStatus("被俘", new Color(160,30,240,255), heroStatusObj);
                    AddHeroResourcesToTotal(hero.GetComponent<AgentLowLevelSystem>().m_Backpack,0.5f);
                }
                else
                {
                    SetStatus("存活", Color.white, heroStatusObj);
                    MetropolisHPModel.Instance.ModifyMetropolisHP(hpReducePerHero);
                }
                
                heroStatusObj.SetActive(true);
            }
        }
        
        public void SetStatus(string status, Color color, GameObject heroStatusObj)
        {
            heroStatusObj.GetComponentInChildren<TextMeshProUGUI>().text = status;
            heroStatusObj.GetComponent<Image>().color = color;
        }
        
        private void AddHeroResourcesToTotal(HeroBackpack backpack, float multiplier)
        {
            // 处理金币和经验球
            if (mTotalResources.ContainsKey(ResourceType.Gold))
            {
                mTotalResources[ResourceType.Gold] += Mathf.RoundToInt(backpack.Gold * multiplier);
            }
            else
            {
                mTotalResources[ResourceType.Gold] = Mathf.RoundToInt(backpack.Gold * multiplier);
            }

            if (mTotalResources.ContainsKey(ResourceType.ExpBall))
            {
                mTotalResources[ResourceType.ExpBall] += Mathf.RoundToInt(backpack.ExpOrb * multiplier);
            }
            else
            {
                mTotalResources[ResourceType.ExpBall] = Mathf.RoundToInt(backpack.ExpOrb * multiplier);
            }
            
            // 处理特殊资源
            foreach (var resource in backpack.SpecialResources)
            {
                if (mTotalResources.ContainsKey(resource.Key))
                {
                    mTotalResources[resource.Key] += Mathf.RoundToInt(resource.Value * multiplier);
                }
                else
                {
                    mTotalResources[resource.Key] = Mathf.RoundToInt(resource.Value * multiplier);
                }
            }
            
            // 处理作物种子
            foreach (var seed in backpack.CropSeeds)
            {
                if (mTotalSeeds.ContainsKey(seed.Key))
                {
                    mTotalSeeds[seed.Key] += Mathf.RoundToInt(seed.Value * multiplier);
                }
                else
                {
                    mTotalSeeds[seed.Key] = Mathf.RoundToInt(seed.Value * multiplier);
                }
            }
        }
        
        private void ShowTotalResources()
        {
            // 显示资源
            foreach (var resource in mTotalResources)
            {
                if (resource.Value > 0)
                    CreateResourceItem(resource.Key.ToString(), resource.Value, GetResourceColor(resource.Key));
            }
            
            // 显示作物种子
            foreach (var seed in mTotalSeeds)
            {
                if (seed.Value > 0)
                    CreateSeedItem(seed.Key.ToString(), seed.Value, Color.green);
            }
        }
        
        private Color GetResourceColor(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Gold: return new Color(1f, 0.84f, 0f);
                case ResourceType.ExpBall: return new Color(0.2f, 0.8f, 1f);
                case ResourceType.Stone: return Color.gray;
                case ResourceType.Wood: return new Color(0.55f, 0.27f, 0.07f);
                case ResourceType.MagicPower: return new Color(0.5f, 0f, 0.5f);
                case ResourceType.Material: return new Color(0.8f, 0.2f, 0.2f);
                default: return Color.white;
            }
        }
        private void CreateResourceItem(string resourceName, int amount, Color color)
        {
            var resourceItem = Instantiate(mResourceItemPrefab, mResourceContainer);
            var resourceImage = resourceItem.GetComponent<Image>();
            var amountText = resourceItem.GetComponentInChildren<TextMeshProUGUI>();
            
            resourceImage.sprite = ResourceModel.Instance.GetResourceSprite(resourceName);
            amountText.text = "X" + amount;
            amountText.color = color;
            
            resourceItem.SetActive(true);
        }

        private void CreateSeedItem(string seedName, int amount, Color color)
        {
            var resourceItem = Instantiate(mResourceItemPrefab, mResourceContainer);
            var resourceImage = resourceItem.GetComponent<Image>();
            var amountText = resourceItem.GetComponentInChildren<TextMeshProUGUI>();
            
            resourceImage.sprite = ResourceModel.Instance.GetCropSprite(seedName);
            amountText.text = "X" + amount;
            amountText.color = color;
            
            resourceItem.SetActive(true);
        }
    }
}