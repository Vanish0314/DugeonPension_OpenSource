using System.Collections;
using System.Collections.Generic;
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
        public Text hpText;
        public Text atkText;
        public Text defText;
        public Text mpText;
        public Text moveSpeedText;
        public Text attackSpeedText;
        public Text recoverMpSpeedText;
        public Text pressureText;
        public Text missText;
        
        [Header("六维属性")] 
        public Text strengthText;
        public Text intelligenceText;
        public Text sensorText;
        public Text agilityText;
        public Text constitutionText;
        public Text charismaText;
        
        [Header("技能")] 
        public Text skill1Text;
        public Text skill2Text;

        public void UpdateHeroInfo(HeroInfoData data)
        {
            // 更新基础属性
            hpText.text = $"血量: {data.baseAttribute.HP}";
            atkText.text = $"攻击力: {data.baseAttribute.ATK}";
            defText.text = $"防御力: {data.baseAttribute.DEF}";
            mpText.text = $"蓝量: {data.baseAttribute.MP}";
            moveSpeedText.text = $"移速: {data.baseAttribute.MoveSpeed}";
            attackSpeedText.text = $"攻速: {data.baseAttribute.AttackSpeed}/次";
            recoverMpSpeedText.text = $"回蓝效率：{data.baseAttribute.RecoverMPSpeed}";
            pressureText.text = $"压力: {data.baseAttribute.Pressure}";
            missText.text = $"闪避: {data.baseAttribute.Miss}%";

            // 更新六维属性
            strengthText.text = $"力量：{data.sixDimensionalAttribute.Strength}";
            intelligenceText.text = $"智力：{data.sixDimensionalAttribute.Intelligence}";
            sensorText.text = $"感知：{data.sixDimensionalAttribute.Sensor}";
            agilityText.text = $"敏捷：{data.sixDimensionalAttribute.Agility}";
            constitutionText.text = $"体质：{data.sixDimensionalAttribute.Constitution}";
            charismaText.text = $"魅力：{data.sixDimensionalAttribute.Charisma}";

            // 更新技能
            skill1Text.text = data.skill.Skill1;
            skill2Text.text = data.skill.Skill2;

            // 更新图片
            heroImage.sprite = data.heroSprite;
        }

        public void SetUIVisibility(bool isOpen)
        {
            openButton.SetActive(!isOpen);
            heroInfoPanel.SetActive(isOpen);
        }
        
    }
}
