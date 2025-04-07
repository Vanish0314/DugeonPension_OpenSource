using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon;
using UnityEngine;

namespace Dungeon
{
    public class ResourceController : MonoBehaviour
    {
       private ResourceFrom m_ResourceFrom;
       private BuildManager m_BuildManager;
       private void Start()
       {
           m_ResourceFrom = GetComponent<ResourceFrom>();
           m_BuildManager = FindObjectOfType<BuildManager>();

           //-------------------------------------测试用
           ResourceModel.Instance.Gold = 200;
           ResourceModel.Instance.Stone = 200;
           ResourceModel.Instance.MagicPower = 200;
           ResourceModel.Instance.Material = 200;
           
           m_BuildManager.OnBuildCompleted += ReduceResources;
           ResourceModel.Instance.OnGoldChanged += UpdateGoldUI;
           ResourceModel.Instance.OnStoneChanged += UpdateStoneUI;
           ResourceModel.Instance.OnMagicPowerChanged += UpdateMagicPowerUI;
           ResourceModel.Instance.OnMaterialChanged += UpdateMaterialUI;
           
           //初始化
           RefreshAllUI();
       }

       private void ReduceResources(BuildingData buildingData)
       {
           if (buildingData.cost.gold > 0)
               ResourceModel.Instance.Gold = Mathf.Max(0, ResourceModel.Instance.Gold - buildingData.cost.gold);
           if (buildingData.cost.stone > 0)
               ResourceModel.Instance.Stone = Mathf.Max(0, ResourceModel.Instance.Stone - buildingData.cost.stone);
           if (buildingData.cost.magicPower > 0)
               ResourceModel.Instance.MagicPower = Mathf.Max(0, ResourceModel.Instance.MagicPower - buildingData.cost.magicPower);
           if (buildingData.cost.material > 0)
               ResourceModel.Instance.Material = Mathf.Max(0, ResourceModel.Instance.Material - buildingData.cost.material);
       }

       private void OnDestroy()
       {
           if (ResourceModel.Instance != null)
           {
               ResourceModel.Instance.OnGoldChanged -= UpdateGoldUI;
               ResourceModel.Instance.OnStoneChanged -= UpdateStoneUI;
               ResourceModel.Instance.OnMagicPowerChanged -= UpdateMagicPowerUI;
               ResourceModel.Instance.OnMaterialChanged -= UpdateMaterialUI;
               m_BuildManager.OnBuildCompleted -= ReduceResources;
           }
       }

       private void UpdateGoldUI()
       {
           m_ResourceFrom.UpdateGoldText(ResourceModel.Instance.Gold);
       }

       private void UpdateStoneUI()
       {
           m_ResourceFrom.UpdateStoneText(ResourceModel.Instance.Stone);
       }

       private void UpdateMagicPowerUI()
       {
           m_ResourceFrom.UpdateMagicPowerText(ResourceModel.Instance.MagicPower);
       }
       
       private void UpdateMaterialUI()
       {
           m_ResourceFrom.UpdateMaterialText(ResourceModel.Instance.Material);
       }
       
       private void RefreshAllUI()
       {
          UpdateGoldUI();
          UpdateStoneUI();
          UpdateMagicPowerUI();
          UpdateMaterialUI();
       }
    }
}
