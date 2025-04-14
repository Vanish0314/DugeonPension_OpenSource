using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class HeroInfoController : MonoBehaviour
    {
        [SerializeField] private Button openButton;
        [SerializeField] private Button returnButton;
        [SerializeField] private Button[] menuButtons;
        
        private HeroInfoForm m_HeroInfoForm;
        private HeroInfoModel m_HeroInfoModel;

        private void Start()
        {
            m_HeroInfoForm = GetComponent<HeroInfoForm>();
            m_HeroInfoModel = HeroInfoModel.Instance;

            // 初始化按钮事件
            openButton.onClick.AddListener(OpenHeroInfo);
            returnButton.onClick.AddListener(CloseHeroInfo);
            
            foreach (var btn in menuButtons)
            {
                btn.onClick.AddListener(() => OnMenuButtonClick(btn));
            }

            // 默认关闭详细信息面板
            m_HeroInfoForm.SetUIVisibility(false);
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

        private void OnMenuButtonClick(Button clickedButton)
        {
            // 根据按钮name解析英雄类型
            string heroName = clickedButton.name.Replace("Btn", "");
            HeroType type = (HeroType)System.Enum.Parse(typeof(HeroType), heroName);
            
            m_HeroInfoModel.SwitchHeroType(type);
            UpdateHeroInfoDisplay();
        }

        private void UpdateHeroInfoDisplay()
        {
            var data = m_HeroInfoModel.GetCurrentHeroData();
            m_HeroInfoForm.UpdateHeroInfo(data);
        }
    }
}
