using Dungeon.Evnents;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class ResourceController : MonoBehaviour
    {
        [SerializeField] private RectTransform goldTransform;
        [SerializeField] private RectTransform stoneTransform;
        [SerializeField] private RectTransform magicPowerTransform;
        [SerializeField] private RectTransform materialTransform;
        [SerializeField] private RectTransform expBallTransform;
        [SerializeField] private RectTransform woodTransform;
        
       private ResourceFrom m_ResourceFrom;
       private PlaceManager m_PlaceManager;

       private void Awake()
       {
           m_ResourceFrom = GetComponent<ResourceFrom>();
           m_PlaceManager = PlaceManager.Instance;

           ResourceModel.Instance.SetRectTransform(ResourceType.Gold, goldTransform);
           ResourceModel.Instance.SetRectTransform(ResourceType.Stone, stoneTransform);
           ResourceModel.Instance.SetRectTransform(ResourceType.MagicPower, magicPowerTransform);
           ResourceModel.Instance.SetRectTransform(ResourceType.Material, materialTransform);
           ResourceModel.Instance.SetRectTransform(ResourceType.ExpBall, expBallTransform);
           ResourceModel.Instance.SetRectTransform(ResourceType.Wood, woodTransform);
       }
       
       private void OnEnable()
       {
           DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnBuildingPlacedEventArgs.EventId, ReduceResources);
           DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnTrapPlacedEventArgs.EventId, ReduceResources);
           DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnMonsterPlacedEventArgs.EventId, ReduceResources);
           
           ResourceModel.Instance.OnGoldChanged += UpdateGoldUI;
           ResourceModel.Instance.CannotAffordGold += OnCannotAffordGold;
           ResourceModel.Instance.OnStoneChanged += UpdateStoneUI;
           ResourceModel.Instance.CannotAffordStone += OnCannotAffordStone;
           ResourceModel.Instance.OnMagicPowerChanged += UpdateMagicPowerUI;
           ResourceModel.Instance.CannotAffordMagicPower += OnCannotAffordMagicPower;
           ResourceModel.Instance.OnMaterialChanged += UpdateMaterialUI;
           ResourceModel.Instance.CannotAffordMaterial += OnCannotAffordMaterial;
           ResourceModel.Instance.OnExpBallChanged += UpdateExpBallUI;
           ResourceModel.Instance.CannotAffordExpBall += OnCannotAffordExpBall;
           ResourceModel.Instance.OnWoodChanged += UpdateWoodUI;
           ResourceModel.Instance.CannotAffordWood += OnCannotAffordWood;
           ResourceModel.Instance.OnCursePowerChanged += UpdateCursePowerUI;
           ResourceModel.Instance.CannotAffordCursePower += OnCannotAffordCursePower;
           
           // 订阅
           DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnSwitchedToMetroplisProcedureEvent.EventId, SetBusinessUI);
           DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnSwitchedToDungeonPlacingProcedureEvent.EventId, SetPlaceArmyUI);
           DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnSwitchedToHeroExploringDungeonProcedureEvent.EventId, SetDungeonUI);
           
           //初始化
           RefreshAllUI();
       }

       private void OnDisable()
       {
           if (ResourceModel.Instance != null)
           {
               DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnBuildingPlacedEventArgs.EventId, ReduceResources);
               DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnTrapPlacedEventArgs.EventId, ReduceResources);
               DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnMonsterPlacedEventArgs.EventId, ReduceResources);

               ResourceModel.Instance.OnGoldChanged -= UpdateGoldUI;
               ResourceModel.Instance.CannotAffordGold -= OnCannotAffordGold;
               ResourceModel.Instance.OnStoneChanged -= UpdateStoneUI;
               ResourceModel.Instance.CannotAffordStone -= OnCannotAffordStone;
               ResourceModel.Instance.OnMagicPowerChanged -= UpdateMagicPowerUI;
               ResourceModel.Instance.CannotAffordMagicPower -= OnCannotAffordMagicPower;
               ResourceModel.Instance.OnMaterialChanged -= UpdateMaterialUI;
               ResourceModel.Instance.CannotAffordMaterial -= OnCannotAffordMaterial;
               ResourceModel.Instance.OnExpBallChanged -= UpdateExpBallUI;
               ResourceModel.Instance.CannotAffordExpBall -= OnCannotAffordExpBall;
               ResourceModel.Instance.OnWoodChanged -= UpdateWoodUI;
               ResourceModel.Instance.CannotAffordWood -= OnCannotAffordWood;
               ResourceModel.Instance.OnCursePowerChanged -= UpdateCursePowerUI;
               ResourceModel.Instance.CannotAffordCursePower -= OnCannotAffordCursePower;
               
               // 取消订阅
               DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnSwitchedToMetroplisProcedureEvent.EventId, SetBusinessUI);
               DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnSwitchedToDungeonPlacingProcedureEvent.EventId, SetPlaceArmyUI);
               DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnSwitchedToHeroExploringDungeonProcedureEvent.EventId, SetDungeonUI);
           }
       }
       
       private void ReduceResources(object sender, GameEventArgs gameEventArgs)
       {
           if (gameEventArgs is OnBuildingPlacedEventArgs buildingPlacedEventArgs)
           {
               BuildingData buildingData = buildingPlacedEventArgs.BuildingData;
               if (!ResourceModel.Instance.TryConsumeResources(buildingData.cost))
               {
                   GameFrameworkLog.Warning(string.Format("Failed to consume resources for building: {0}",
                       buildingData.name));
               }
           }
           else if (gameEventArgs is OnTrapPlacedEventArgs trapPlacedEventArgs)
           {
               TrapData trapData = trapPlacedEventArgs.TrapData;
               if (!ResourceModel.Instance.TryConsumeResources(trapData.cost))
               {
                   GameFrameworkLog.Warning(string.Format("Failed to consume resources for trap: {0}",
                       trapData.name));
               }
           }
           else if(gameEventArgs is OnMonsterPlacedEventArgs monsterPlacedEventArgs)
           {
               MonsterData monsterData = monsterPlacedEventArgs.MonsterData;
               if (!ResourceModel.Instance.TryConsumeResources(monsterData.cost))
               {
                   GameFrameworkLog.Warning(string.Format("Failed to consume resources for monster: {0}",
                       monsterData.name));
               }
           }
       }
       
       private void UpdateGoldUI(float value = 0)
       {
           if (value > 0)
           {
               m_ResourceFrom.UpdateGoldTextDelay(ResourceModel.Instance.Gold);
           }
           else
           {
               m_ResourceFrom.UpdateGoldText(ResourceModel.Instance.Gold);
           }
       }
       
       private void OnCannotAffordGold()
       {
           m_ResourceFrom.ShakeWarningGoldText();
       }

       private void UpdateStoneUI(float value = 0)
       {
           if (value > 0)
           {
               m_ResourceFrom.UpdateStoneTextDelay(ResourceModel.Instance.Stone);
           }
           else
           {
               m_ResourceFrom.UpdateStoneText(ResourceModel.Instance.Stone);
           }
       }

       private void OnCannotAffordStone()
       {
           m_ResourceFrom.ShakeWarningStoneText();
       }

       private void UpdateMagicPowerUI(float value = 0)
       {
           if (value > 0)
           {
               m_ResourceFrom.UpdateMagicPowerTextDelay(ResourceModel.Instance.MagicPower);
           }
           else
           {
               m_ResourceFrom.UpdateMagicPowerText(ResourceModel.Instance.MagicPower);
           }
       }

       private void OnCannotAffordMagicPower()
       {
           m_ResourceFrom.ShakeWarningMagicPowerText();
       }
       
       private void UpdateMaterialUI(float value = 0)
       {
           if (value > 0)
           {
               m_ResourceFrom.UpdateMaterialTextDelay(ResourceModel.Instance.Material);
           }
           else
           {
               m_ResourceFrom.UpdateMaterialText(ResourceModel.Instance.Material);
           }
       }

       private void OnCannotAffordMaterial()
       {
           m_ResourceFrom.ShakeWarningMaterialText();
       }
       
       private void UpdateExpBallUI(float value = 0)
       {
           if (value > 0)
           {
               m_ResourceFrom.UpdateExpBallTextDelay(ResourceModel.Instance.ExpBall);
           }
           else
           {
               m_ResourceFrom.UpdateExpBallText(ResourceModel.Instance.ExpBall);
           }
       }

       private void OnCannotAffordExpBall()
       {
           m_ResourceFrom.ShakeWarningExpBallText();
       }
       
       private void UpdateWoodUI(float value = 0)
       {
           if (value > 0)
           {
               m_ResourceFrom.UpdateWoodTextDelay(ResourceModel.Instance.Wood);
           }
           else
           {
               m_ResourceFrom.UpdateWoodText(ResourceModel.Instance.Wood);
           }
       }

       private void OnCannotAffordWood()
       {
           m_ResourceFrom.ShakeWarningWoodText();
       }

       private float countCursePower = 0f;
       private int lastIntegerValue = 0; // 记录上一次的整数值

       private void UpdateCursePowerUI(float value = 0)
       {
           countCursePower += value;
           int currentInteger = Mathf.FloorToInt(countCursePower);

           // 如果整数部分变化（例如从 1.9 → 2.0），触发逻辑
           if (currentInteger > lastIntegerValue || currentInteger < lastIntegerValue)
           {
               lastIntegerValue = currentInteger;
               m_ResourceFrom.UpdateCursePowerText(ResourceModel.Instance.CursePower);
           }
       }

       private void OnCannotAffordCursePower()
       {
           m_ResourceFrom.ShakeWarningCursePowerText(); 
       }
       
       private void SetBusinessUI(object sender, GameEventArgs e)
       {
           m_ResourceFrom.SetSomeUIActive(true);
           m_ResourceFrom.SetCursePowerUIActive(false);
       }
       
       private void SetPlaceArmyUI(object sender, GameEventArgs e)
       {
           m_ResourceFrom.SetSomeUIActive(false);
           m_ResourceFrom.SetCursePowerUIActive(false);
       }

       private void SetDungeonUI(object sender, GameEventArgs e)
       {
           m_ResourceFrom.SetSomeUIActive(false);
           m_ResourceFrom.SetCursePowerUIActive(true);
       }
       private void RefreshAllUI()
       {
          UpdateGoldUI();
          UpdateStoneUI();
          UpdateMagicPowerUI();
          UpdateMaterialUI();
          UpdateExpBallUI();
          UpdateWoodUI();
          UpdateCursePowerUI();
       }
    }
}
