using Dungeon;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text resourceTypeText;
    [SerializeField] private Text currentStockText;
    [SerializeField] private Text maxStockText;
    [SerializeField] private Button collectButton;

    private MetropolisBuildingBase targetBuilding;

    public void Setup(MetropolisBuildingBase building)
    {
        targetBuilding = building;
        collectButton.onClick.AddListener(OnCollectClick);
        UpdateUI(building);
    }

    private void UpdateUI(MetropolisBuildingBase building)
    {
        resourceTypeText.text = $"资源类型: {building.resourceType}";
        currentStockText.text = $"当前存量: {building.currentStock}";
        maxStockText.text = $"最大容量: {building.maxStock}";
        collectButton.interactable = building.currentStock > 0;
    }

    private void OnCollectClick()
    {
        targetBuilding.GatherResources();
        UpdateUI(targetBuilding); // 刷新UI
    }

    private void Update()
    {
        
    }
}