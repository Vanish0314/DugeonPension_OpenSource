using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class BuildingBlueprintButton : MonoBehaviour
    {
        public BuildingBlueprintData buildingBlueprintData;

        private void OnEnable()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            GetComponent<Button>().onClick.RemoveListener(OnClick);
        }

        public void OnClick()
        {
            TechnologyTreeManager.Instance.currentBuildingBlueprintData = buildingBlueprintData;
            DungeonGameEntry.DungeonGameEntry.Event.Fire(this, OnBlueprintButtonClickedEventArgs.Create(buildingBlueprintData.BuildingID));
        }
    }
    
    public class OnBlueprintButtonClickedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnBlueprintButtonClickedEventArgs).GetHashCode();
        
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
        
        public static OnBlueprintButtonClickedEventArgs Create(int buildingID)
        {
            OnBlueprintButtonClickedEventArgs onBlueprintButtonClickedEventArgs = ReferencePool.Acquire<OnBlueprintButtonClickedEventArgs>();
            onBlueprintButtonClickedEventArgs.buildingID = buildingID;
            return onBlueprintButtonClickedEventArgs;
        }

        public override void Clear()
        {
        }
    }
}
