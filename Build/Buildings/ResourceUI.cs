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

    private MetropolisBuildingBase targetBuilding;


    private void Awake()
    {
        targetBuilding = transform.parent.GetComponent<MetropolisBuildingBase>();
        collectButton.onClick.AddListener(OnCollectClick);
    }

    private void Update()
    {
        if (targetBuilding != null)
            fillAmount = (float)targetBuilding.currentStock / targetBuilding.maxStock;

        if (fillAmount >= 0.8)
        {
            ShowStockUI();
        }
        UpdateUI(targetBuilding);
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

    private void UpdateUI(MetropolisBuildingBase building)
    {
        stockSlider.value = fillAmount;
        percentageText.text = (fillAmount * 100) + "%";
        collectButton.interactable = building.currentStock > 0;
    }

    private void OnCollectClick()
    {
        targetBuilding.GatherResources();
        UpdateUI(targetBuilding); // 刷新UI
    }
}