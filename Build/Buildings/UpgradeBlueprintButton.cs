using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class UpgradeBlueprintButton : MonoBehaviour
    {
        public UpgradeBlueprintData blueprintData;

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
            UpgradeBlueprintManager.Instance.currentBlueprintData = blueprintData;
            DungeonGameEntry.DungeonGameEntry.Event.Fire(this,
                OnUpgradeBlueprintButtonClickedEventArgs.Create(blueprintData.blueprintID));
        }
    }
    
    public class OnUpgradeBlueprintButtonClickedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnUpgradeBlueprintButtonClickedEventArgs).GetHashCode();
        
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
        
        public static OnUpgradeBlueprintButtonClickedEventArgs Create(int blueprintID)
        {
            OnUpgradeBlueprintButtonClickedEventArgs onUpgradeBlueprintButtonClickedEventArgs = ReferencePool.Acquire<OnUpgradeBlueprintButtonClickedEventArgs>();
            onUpgradeBlueprintButtonClickedEventArgs.blueprintID = blueprintID;
            return onUpgradeBlueprintButtonClickedEventArgs;
        }

        public override void Clear()
        {
        }
    }
}
