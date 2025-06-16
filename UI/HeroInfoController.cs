using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class HeroInfoController : MonoBehaviour
    {
        [SerializeField] private Button openButton;
        [SerializeField] private Button returnButton;
        [SerializeField] private Transform heroButtonsParent;
        [SerializeField] private GameObject heroButtonPrefab;
        
        private HeroInfoForm m_HeroInfoForm;
        private HeroInfoModel m_HeroInfoModel;
        
        private Dictionary<HeroEntityBase, GameObject> heroButtonMap = new Dictionary<HeroEntityBase, GameObject>();
        
        private void Start()
        {
            m_HeroInfoForm = GetComponent<HeroInfoForm>();
            m_HeroInfoModel = HeroInfoModel.Instance;

            // 初始化按钮事件
            openButton.onClick.AddListener(OpenHeroInfo);
            returnButton.onClick.AddListener(CloseHeroInfo);

            // 默认关闭详细信息面板
            m_HeroInfoForm.SetUIVisibility(false);
            
            // 初始更新按钮
            UpdateHeroButtons();
        }

        private void OpenHeroInfo()
        {
            m_HeroInfoForm.SetUIVisibility(true);
            UpdateHeroInfoDisplay();
        }

        private void CloseHeroInfo()
        {
            m_HeroInfoForm.SetUIVisibility(false);
        }

         private void UpdateHeroButtons()
        {
            ClearExistingButtons();
            
            // 为活跃英雄创建按钮
            foreach (var hero in m_HeroInfoModel.ActiveHeroes)
            {
                CreateHeroButton(hero, "Active");
            }
            
            // 为被捕获英雄创建按钮
            foreach (var hero in m_HeroInfoModel.CapturedHeroes)
            {
                CreateHeroButton(hero, "Captured");
            }
        }

        private void CreateHeroButton(HeroEntityBase hero, string status)
        {
            if (heroButtonPrefab == null || heroButtonsParent == null) return;
            
            var buttonObj = Instantiate(heroButtonPrefab, heroButtonsParent);
            var button = buttonObj.GetComponent<Button>();
            
            // 设置按钮图片
            var heroVisualData = m_HeroInfoModel.GetVisualData(hero.HeroName);
            buttonObj.GetComponent<Image>().sprite = heroVisualData.buttonImage;
            
            // 设置按钮文本
            var text = buttonObj.GetComponentInChildren<Text>();
            if (text != null) text.text = $"{hero.HeroName} ({status})";
            
            heroButtonMap[hero] = buttonObj;
            button.onClick.AddListener(() => OnHeroButtonClicked(hero));
            
            buttonObj.SetActive(true);
        }

        private void ClearExistingButtons()
        {
            foreach (var button in heroButtonMap.Values)
            {
                if (button != null) Destroy(button.gameObject);
            }
            heroButtonMap.Clear();
        }

        private void OnHeroButtonClicked(HeroEntityBase hero)
        {
            Debug.Log($"Hero button clicked: {hero.name}");
            // 更新详细信息显示
            UpdateHeroInfoDisplay(hero);
        }

        private void UpdateHeroInfoDisplay(HeroEntityBase hero = null)
        {
            // 如果有指定英雄则显示其信息，否则显示第一个英雄或默认信息
            HeroEntityBase heroToDisplay = hero ?? 
                                         (m_HeroInfoModel.ActiveHeroes.Count > 0 ? 
                                          m_HeroInfoModel.ActiveHeroes[0] : 
                                          null);
            
            m_HeroInfoForm.UpdateHeroInfo(heroToDisplay);
        }
    }
}
