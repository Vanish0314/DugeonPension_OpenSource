using System.Collections;
using System.Collections.Generic;
using Dungeon.DungeonEntity;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public class UpgradeBlueprintManager : MonoBehaviour
    {
        [Header("当前选中蓝图")] public UpgradeBlueprintData currentBlueprintData;
        
        public static UpgradeBlueprintManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            transform.position = Vector3.zero;
        }
        
        private int _firstFlag;
        private void OnEnable()
        {
            if (_firstFlag == 0)
            {
                _firstFlag++;
            }
            else
            {
                Subscribe();
            }
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Initialize()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            
        }

        private void Unsubscribe()
        {
            
        }
        
        public void UnlockUpgradeBlueprint()
        {
            if (currentBlueprintData == null)
                return;

            bool hasResource = TechnologyTreeManager.Instance.currentTechnologyPoint >=
                               currentBlueprintData.unlockPointCost &&
                               ResourceModel.Instance.ExpBall >= currentBlueprintData.unlockExpCost;
            
            if (!currentBlueprintData.isUnlocked && hasResource)
            {
                TechnologyTreeManager.Instance.currentTechnologyPoint -= currentBlueprintData.unlockPointCost;
                ResourceModel.Instance.ExpBall -= currentBlueprintData.unlockExpCost;
                
                currentBlueprintData.isUnlocked = true;
                currentBlueprintData.blueprintLevel = 1;

                if (!currentBlueprintData.isMonster)
                {
                    currentBlueprintData.blueprintLevel = 3;
                }

                DungeonGameEntry.DungeonGameEntry.Event.Fire(this,
                    OnUpgradeBlueprintUnlockEventArgs.Create(currentBlueprintData.blueprintID,
                        currentBlueprintData.isMonster));
            }
        }
        
        public void UpgradeUpgradeBlueprint()
        {
            if (currentBlueprintData == null)
                return;

            bool hasResource = false;
            
            switch (currentBlueprintData.blueprintLevel)
            {
                case 1:
                    hasResource = ResourceModel.Instance.ExpBall >= currentBlueprintData.upgradeExpCost1;
                    break;
                case 2:
                    hasResource = ResourceModel.Instance.ExpBall >= currentBlueprintData.upgradeExpCost2;
                    break;
                default:
                    break;
            }
            
            if (currentBlueprintData.isUnlocked && hasResource)
            {
                var monsterComponent = (DungeonMonsterBase)PlaceManager.Instance.GetMonsterTemplateByType(currentBlueprintData.monsterType);
                
                switch (currentBlueprintData.blueprintLevel)
                {
                    case 1:
                        ResourceModel.Instance.ExpBall -= currentBlueprintData.upgradeExpCost1;
                        monsterComponent.MaxHp = currentBlueprintData.maxHp2;
                        monsterComponent.AttackSpeed = currentBlueprintData.attackSpeed2;
                        monsterComponent.BasicInfo.physicalResistance = currentBlueprintData.physicalResistance2;
                        monsterComponent.BasicInfo.fireResistance = currentBlueprintData.fireResistance2;
                        monsterComponent.BasicInfo.iceResistance = currentBlueprintData.iceResistance2;
                        monsterComponent.BasicInfo.holyResistance = currentBlueprintData.holyResistance2;
                        monsterComponent.BasicInfo.posionResistance = currentBlueprintData.posionResistance2;
                        monsterComponent.skill = currentBlueprintData.skillData2;
                        break;
                    case 2:
                        ResourceModel.Instance.ExpBall -= currentBlueprintData.upgradeExpCost2;
                        monsterComponent.MaxHp = currentBlueprintData.maxHp3;
                        monsterComponent.AttackSpeed = currentBlueprintData.attackSpeed3;
                        monsterComponent.BasicInfo.physicalResistance = currentBlueprintData.physicalResistance3;
                        monsterComponent.BasicInfo.fireResistance = currentBlueprintData.fireResistance3;
                        monsterComponent.BasicInfo.iceResistance = currentBlueprintData.iceResistance3;
                        monsterComponent.BasicInfo.holyResistance = currentBlueprintData.holyResistance3;
                        monsterComponent.BasicInfo.posionResistance = currentBlueprintData.posionResistance3;
                        monsterComponent.skill = currentBlueprintData.skillData3;
                        break;
                    default:
                        break;
                }
                
                currentBlueprintData.blueprintLevel += 1;
     
                DungeonGameEntry.DungeonGameEntry.Event.Fire(this,
                    OnUpgradeBlueprintUpgradeEventArgs.Create(currentBlueprintData.blueprintID));
            }
        }
    }
    
    public class OnUpgradeBlueprintUnlockEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnUpgradeBlueprintUnlockEventArgs).GetHashCode();
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public int blueprintID
        {
            get;
            private set;
        }

        public bool isMonster
        {
            get;
            private set;
        }
        
        public static OnUpgradeBlueprintUnlockEventArgs Create(int blueprintID, bool isMonster)
        {
            OnUpgradeBlueprintUnlockEventArgs onUpgradeBlueprintUnlockEventArgs = ReferencePool.Acquire<OnUpgradeBlueprintUnlockEventArgs>();
            onUpgradeBlueprintUnlockEventArgs.blueprintID = blueprintID;
            onUpgradeBlueprintUnlockEventArgs.isMonster = isMonster;
            return onUpgradeBlueprintUnlockEventArgs;
        }

        public override void Clear()
        {
        }
    }
    
    public class OnUpgradeBlueprintUpgradeEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnUpgradeBlueprintUpgradeEventArgs).GetHashCode();
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public int blueprintID
        {
            get;
            private set;
        }
        
        public static OnUpgradeBlueprintUpgradeEventArgs Create(int blueprintID)
        {
            OnUpgradeBlueprintUpgradeEventArgs onUpgradeBlueprintUpgradeEventArgs = ReferencePool.Acquire<OnUpgradeBlueprintUpgradeEventArgs>();
            onUpgradeBlueprintUpgradeEventArgs.blueprintID = blueprintID;
            return onUpgradeBlueprintUpgradeEventArgs;
        }

        public override void Clear()
        {
        }
    }
}
