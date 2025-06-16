using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class CastleUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text percentageText;
        [SerializeField] private Slider stockSlider;
        [SerializeField] private float fillAmount;
        [SerializeField] private Button collectButton;
        [SerializeField] private Button treeButton;

        private Castle m_TargetBuilding;
    
        private void Awake()
        {
            m_TargetBuilding = transform.parent.GetComponent<Castle>();
            collectButton.onClick.AddListener(OnCollectClick);
            treeButton.onClick.AddListener(OnTreeClick);
        }
        
        private void Update()
        {
            if (m_TargetBuilding != null)
                fillAmount = (float)m_TargetBuilding.currentStock / m_TargetBuilding.maxStock;

            if (fillAmount >= 0.8)
            {
                if (!stockSlider.gameObject.activeInHierarchy)
                    ShowStockUI();
            }
        
            if(fillAmount >= 1)
            {
                ShowAllUI();
            }
        
            UpdateUI(m_TargetBuilding);
        }
        public void ShowAllUI()
        {
            stockSlider.gameObject.SetActive(true);
            collectButton.gameObject.SetActive(true);
            treeButton.gameObject.SetActive(true);
        }

        public void HideAllUI()
        {
            stockSlider.gameObject.SetActive(false);
            collectButton.gameObject.SetActive(false);
            treeButton.gameObject.SetActive(false);
        }

        public void ShowStockUI()
        {
            stockSlider.gameObject.SetActive(true);
        }

        private void UpdateUI(Castle building)
        {
            stockSlider.value = fillAmount;
            percentageText.text = (fillAmount * 100) + "%";
            collectButton.interactable = building.currentStock > 0;
        }

        private void OnCollectClick()
        {
            m_TargetBuilding.GatherMagicPower();
            UpdateUI(m_TargetBuilding); // 刷新UI
        }
        
        private void OnTreeClick()
        {
            GameEntry.UI.OpenUIForm(EnumUIForm.TechnologyTreeForm);
        }
    }
}
