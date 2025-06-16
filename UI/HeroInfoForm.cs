using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using Dungeon.SkillSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public enum HeroType
    {
        Warrior,
        Mage
    }
    
    public class HeroInfoForm : UGuiForm
    {
        [SerializeField] private GameObject openButton;
        [SerializeField] private GameObject heroInfoPanel;
        [SerializeField] private Image heroImage;
        
        [Header("基础属性")]
        public Text levelText;
        public Text hpText;
        public Text atkText;
        public Text defText;
        public Text mpText;
        public Text moveSpeedText;
        public Text physicsText;
        public Text poisonText;
        public Text lightText;
        public Text fireText;
        public Text iceText;
        
        [Header("六维属性")] 
        public Text strengthText;
        public Text intelligenceText;
        public Text sensorText;
        public Text agilityText;
        public Text constitutionText;
        public Text charismaText;
        
        [Header("六维属性")] 
        [SerializeField] private Slider qufuduBar;
        
        [Header("技能显示")]
        [SerializeField] private Transform skillsParent; // 技能图标的父物体
        [SerializeField] private GameObject skillIconPrefab; // 技能图标预制体
        
        [Header("背包资源显示")]
        [SerializeField] private Transform resourcesParent; // 资源图标的父物体
        [SerializeField] private GameObject resourceItemPrefab; // 资源项预制体
        

        public void UpdateHeroInfo(HeroEntityBase hero)
        {
            var heroLowSys = hero.GetComponent<AgentLowLevelSystem>();
            
            // 更新基础属性
            levelText.text = $"LV.{heroLowSys.m_Properties.combatorData.currentLevel}";
            hpText.text = $"{hero.GetHp()}";
            mpText.text = $"{hero.GetMp()}";
            atkText.text = $"{heroLowSys.DndSkillData.StrengthModifyValue}";
            defText.text = $"{heroLowSys.DndSkillData.DexterityModifyValue}";
            moveSpeedText.text = $"{heroLowSys.m_MoveMaxSpeed}";
            physicsText.text =
                $"{GetResistanceLevelText(heroLowSys.m_Properties.combatorData.physicalResistance)}";
            poisonText.text =
                $"{GetResistanceLevelText(heroLowSys.m_Properties.combatorData.posionResistance)}";
            lightText.text =
                $"{GetResistanceLevelText(heroLowSys.m_Properties.combatorData.holyResistance)}";
            fireText.text =
                $"{GetResistanceLevelText(heroLowSys.m_Properties.combatorData.fireResistance)}";
            iceText.text =
                $"{GetResistanceLevelText(heroLowSys.m_Properties.combatorData.iceResistance)}";
            
            // 更新六维属性
            strengthText.text = $"力量：{heroLowSys.DndSkillData.Strength}";
            intelligenceText.text = $"智力：{heroLowSys.DndSkillData.Intelligence}";
            sensorText.text = $"感知：{heroLowSys.DndSkillData.Wisdom}";
            agilityText.text = $"敏捷：{heroLowSys.DndSkillData.Dexterity}";
            constitutionText.text = $"体质：{heroLowSys.DndSkillData.Constitution}";
            charismaText.text = $"魅力：{heroLowSys.DndSkillData.Charisma}";
            
            // 更新屈服度
            qufuduBar.value = heroLowSys.GetSubmissiveness() / 100f;

            // 更新技能
            UpdateHeroSkills(heroLowSys.CurrentOwnedSkills());
            
            // 更新背包资源
            UpdateHeroResources(heroLowSys.m_Backpack);

            // 更新立绘
            var heroVisualData = HeroInfoModel.Instance.GetVisualData(hero.HeroName);
            heroImage.sprite = heroVisualData.portrait;
        }

        public void SetUIVisibility(bool isOpen)
        {
            openButton.SetActive(!isOpen);
            heroInfoPanel.SetActive(isOpen);
        }

        private string GetResistanceLevelText(ResistanceLevel level)
        {
            switch (level)
            {
                case ResistanceLevel.Weak
                    : return "弱点";
                case ResistanceLevel.Normal
                    : return "普通";
                case ResistanceLevel.Strong
                    : return "抵抗";
                case ResistanceLevel.Immunity
                    : return "免疫";
                default:
                    return "无";
            }
        }
        
        private void UpdateHeroSkills(List<SkillData> skills)
        {
            // 清除现有技能图标
            foreach (Transform child in skillsParent)
            {
                Destroy(child.gameObject);
            }

            // 创建新的技能图标
            foreach (var skill in skills)
            {
                var skillIcon = Instantiate(skillIconPrefab, skillsParent);
                var image = skillIcon.GetComponent<Image>();
                var text = skillIcon.GetComponentInChildren<Text>();
            
                if (image != null && skill.skillIcon != null)
                {
                    image.sprite = skill.skillIcon;
                }
            
                if (text != null)
                {
                    text.text = skill.skillName;
                }
                
                skillIcon.gameObject.SetActive(true);
            }
        }

        public void UpdateHeroResources(HeroBackpack backpack)
        {
            // 清除现有资源显示
            foreach (Transform child in resourcesParent)
            {
                Destroy(child.gameObject);
            }

            // 显示金币
            CreateResourceItem("Gold", backpack.Gold);
        
            // 显示经验球
            CreateResourceItem("ExpBall", backpack.ExpOrb);

            // 显示特殊资源
            foreach (var resource in backpack.SpecialResources)
            {
                CreateResourceItem(resource.Key.ToString(), resource.Value);
            }

            // 显示种子
            foreach (var seed in backpack.CropSeeds)
            {
                CreateResourceItem(seed.Key.ToString(), seed.Value);
            }
        }

        private void CreateResourceItem(string resourceName, int amount)
        {
            var item = Instantiate(resourceItemPrefab, resourcesParent);
            var image = item.GetComponentInChildren<Image>();
            var text = item.GetComponentInChildren<Text>();
        
            // 获取资源图标
            var sprite = ResourceModel.Instance.GetResourceSprite(resourceName) ?? 
                         ResourceModel.Instance.GetCropSprite(resourceName);
        
            if (image != null && sprite != null)
            {
                image.sprite = sprite;
            }
        
            if (text != null)
            {
                text.text = "X" + amount;
            }
            
            item.gameObject.SetActive(true);
        }
    }
}
