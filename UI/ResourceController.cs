using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class ResourceController : MonoBehaviour
    {
       private ResourceFrom m_ResourceFrom;
       private PlaceManager m_PlaceManager;

       private void Awake()
       {
           m_ResourceFrom = GetComponent<ResourceFrom>();
           m_PlaceManager = PlaceManager.Instance;
           
           //-------------------------------------测试用
           ResourceModel.Instance.Gold = 200;
           ResourceModel.Instance.Stone = 200;
           ResourceModel.Instance.MagicPower = 200;
           ResourceModel.Instance.Material = 200;
       }
       
       private void OnEnable()
       {
           GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnBuildingPlacedEventArgs.EventId, ReduceResources);
           GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnTrapPlacedEventArgs.EventId, ReduceResources);
           GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnMonsterPlacedEventArgs.EventId, ReduceResources);
           
           ResourceModel.Instance.OnGoldChanged += UpdateGoldUI;
           ResourceModel.Instance.OnStoneChanged += UpdateStoneUI;
           ResourceModel.Instance.OnMagicPowerChanged += UpdateMagicPowerUI;
           ResourceModel.Instance.OnMaterialChanged += UpdateMaterialUI;
           
           // 订阅
           GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnBusinessStartEventArgs.EventId, SetBusinessUI);
           GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnPlaceArmyStartEventArgs.EventId, SetPlaceArmyUI);
           GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnFightStartEventArgs.EventId, SetPlaceArmyUI);
           
           //初始化
           RefreshAllUI();
       }

       private void OnDisable()
       {
           if (ResourceModel.Instance != null)
           {
               GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnBuildingPlacedEventArgs.EventId, ReduceResources);
               GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnTrapPlacedEventArgs.EventId, ReduceResources);
               GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnMonsterPlacedEventArgs.EventId, ReduceResources);

               ResourceModel.Instance.OnGoldChanged -= UpdateGoldUI;
               ResourceModel.Instance.OnStoneChanged -= UpdateStoneUI;
               ResourceModel.Instance.OnMagicPowerChanged -= UpdateMagicPowerUI;
               ResourceModel.Instance.OnMaterialChanged -= UpdateMaterialUI;
               
               // 取消订阅
               GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnBusinessStartEventArgs.EventId, SetBusinessUI);
               GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnPlaceArmyStartEventArgs.EventId, SetPlaceArmyUI);
               GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnFightStartEventArgs.EventId, SetPlaceArmyUI);
           }
       }

       private void OnDestroy()
       {
           
       }
       
       private void ReduceResources(object sender, GameEventArgs gameEventArgs)
       {
           if (gameEventArgs is OnBuildingPlacedEventArgs buildingPlacedEventArgs)
           {
               BuildingData buildingData = buildingPlacedEventArgs.BuildingData;
               if (buildingData.cost.gold > 0)
                   ResourceModel.Instance.Gold = Mathf.Max(0, ResourceModel.Instance.Gold - buildingData.cost.gold);
               if (buildingData.cost.stone > 0)
                   ResourceModel.Instance.Stone = Mathf.Max(0, ResourceModel.Instance.Stone - buildingData.cost.stone);
               if (buildingData.cost.magicPower > 0)
                   ResourceModel.Instance.MagicPower = Mathf.Max(0, ResourceModel.Instance.MagicPower - buildingData.cost.magicPower);
               if (buildingData.cost.material > 0)
                   ResourceModel.Instance.Material = Mathf.Max(0, ResourceModel.Instance.Material - buildingData.cost.material);
           }
           else if (gameEventArgs is OnTrapPlacedEventArgs trapPlacedEventArgs)
           {
               TrapData trapData = trapPlacedEventArgs.TrapData;
               ResourceModel.Instance.Material = Mathf.Max(0, ResourceModel.Instance.Material - trapData.cost.material);
           }
           else if(gameEventArgs is OnMonsterPlacedEventArgs monsterPlacedEventArgs)
           {
               MonsterData monsterData = monsterPlacedEventArgs.MonsterData;
               ResourceModel.Instance.MagicPower = Mathf.Max(0, ResourceModel.Instance.MagicPower - monsterData.cost.magicPower);
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

       private void SetBusinessUI(object sender, GameEventArgs e)
       {
           m_ResourceFrom.SetSomeUIActive(true);
       }
       
       private void SetPlaceArmyUI(object sender, GameEventArgs e)
       {
           m_ResourceFrom.SetSomeUIActive(false);
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
