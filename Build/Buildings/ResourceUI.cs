using System;
using Dungeon;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text percentageText;
    [SerializeField] private Slider stockSlider;
    [SerializeField] private float fillAmount;
    [SerializeField] private Button collectButton;

    private ProduceBuilding m_TargetBuilding;


    private void Awake()
    {
        m_TargetBuilding = transform.parent.GetComponent<ProduceBuilding>();
        collectButton.onClick.AddListener(OnCollectClick);
    }

    private void Update()
    {
        if (m_TargetBuilding != null)
            fillAmount = (float)m_TargetBuilding.currentStock / m_TargetBuilding.maxStock;

        if (fillAmount >= 0.8)
        {
            ShowStockUI();
        }
        UpdateUI(m_TargetBuilding);
    }
    public void ShowAllUI()
    {
        stockSlider.gameObject.SetActive(true);
        collectButton.gameObject.SetActive(true);
    }

    public void HideAllUI()
    {
        stockSlider.gameObject.SetActive(false);
        collectButton.gameObject.SetActive(false);
    }

    public void ShowStockUI()
    {
        stockSlider.gameObject.SetActive(true);
    }

    private void UpdateUI(ProduceBuilding building)
    {
        stockSlider.value = fillAmount;
        percentageText.text = (fillAmount * 100) + "%";
        collectButton.interactable = building.currentStock > 0;
    }

    private void OnCollectClick()
    {
        m_TargetBuilding.GatherResources();
        UpdateUI(m_TargetBuilding); // 刷新UI
    }
}