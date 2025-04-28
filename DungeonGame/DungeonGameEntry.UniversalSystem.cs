using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.DungeonGameEntry
{
    public partial class DungeonGameEntry : MonoBehaviour
    {
        private void InitUniversalSystem()
        {
            PlaceManager placeManager = GameObject.FindObjectOfType<PlaceManager>();
            placeManager.Initialize();
            
            TimeManager timeManager = GameObject.FindObjectOfType<TimeManager>();
            timeManager.Subscribe();
            
            HPManager.Instance.Initialize();
        }
    }
}
