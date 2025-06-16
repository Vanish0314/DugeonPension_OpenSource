using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Dungeon
{
    public class TechnologyTreeManager : MonoBehaviour
    {
        [Header("当前魔能")] public int currentMagicPower;
        [Header("当前研究点数")] public int currentTechnologyPoint = 10;
        [Header("下一点数所需魔能")] public int nextTechnologyPointMagicPower;
        [Header("当前解锁建筑数")] [SerializeField] private int currentBuildingCount;
        [Header("下一等级所需建筑数")] [SerializeField] private int nextLevelBuildingCount = 2;
        [Header("当前科技树等级")] public int currentLevel = 1;
        [Header("当前选中建筑蓝图")] public BuildingBlueprintData currentBuildingBlueprintData;

        public static TechnologyTreeManager Instance { get; private set; }

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
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnMagicPowerChangeEventArgs.EventId, UpdateTechnologyPoint);
        }

        private void Unsubscribe()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnMagicPowerChangeEventArgs.EventId, UpdateTechnologyPoint);
        }

        public void ModifyMagicPower(int magicPower)
        {
            currentMagicPower += magicPower;
            DungeonGameEntry.DungeonGameEntry.Event.Fire(this,OnMagicPowerChangeEventArgs.Create());
        }

        private void Upgrade()
        {
            if (currentBuildingCount < nextLevelBuildingCount)
                return;
            
            currentLevel++;
            nextLevelBuildingCount += 3;
        }

        private void UpdateTechnologyPoint(object sender, GameEventArgs gameEventArgs)
        {
            if (currentMagicPower < nextTechnologyPointMagicPower)
                return;
            
            currentTechnologyPoint++;
            currentMagicPower -= nextTechnologyPointMagicPower;
            nextTechnologyPointMagicPower *= nextLevelBuildingCount;
        }

        public void UnlockBuildingBlueprint()
        {
            if (currentBuildingBlueprintData == null)
                return;

            bool hasRequiredLevel = currentLevel >= currentBuildingBlueprintData.NeedLevel;
            bool hasEnoughPoints = currentTechnologyPoint >= currentBuildingBlueprintData.BuildingCost;
            bool prerequisitesMet = true;
    
            foreach (var prereq in currentBuildingBlueprintData.NeedBuildings)
            {
                if (!prereq.IsUnlocked)
                {
                    prerequisitesMet = false;
                    break;
                }
            }

            if (hasRequiredLevel && prerequisitesMet && 
                !currentBuildingBlueprintData.IsUnlocked && hasEnoughPoints)
            {
                currentTechnologyPoint -= currentBuildingBlueprintData.BuildingCost;
                currentBuildingBlueprintData.IsUnlocked = true;
                currentBuildingCount++;
        
                if (currentBuildingCount >= nextLevelBuildingCount)
                {
                    Upgrade();
                }

                DungeonGameEntry.DungeonGameEntry.Event.Fire(this,
                    OnBlueprintUnlockEventArgs.Create(currentBuildingBlueprintData.BuildingID));
            }
        }
    }

    public class OnMagicPowerChangeEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnMagicPowerChangeEventArgs).GetHashCode();
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnMagicPowerChangeEventArgs Create()
        {
            OnMagicPowerChangeEventArgs onMagicPowerChangeEventArgs = ReferencePool.Acquire<OnMagicPowerChangeEventArgs>();
            return onMagicPowerChangeEventArgs;
        }

        public override void Clear()
        {
        }
    }
    
    public class OnBlueprintUnlockEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnBlueprintUnlockEventArgs).GetHashCode();
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public int buildingID
        {
            get;
            private set;
        }
        
        public static OnBlueprintUnlockEventArgs Create(int buildingID)
        {
            OnBlueprintUnlockEventArgs onBlueprintUnlockEventArgs = ReferencePool.Acquire<OnBlueprintUnlockEventArgs>();
            onBlueprintUnlockEventArgs.buildingID = buildingID;
            return onBlueprintUnlockEventArgs;
        }

        public override void Clear()
        {
        }
    }
}
