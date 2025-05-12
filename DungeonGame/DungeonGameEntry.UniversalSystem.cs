using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.DungeonGameEntry
{
    public partial class DungeonGameEntry : MonoBehaviour
    {
        private void InitUniversalSystem()
        {
            PlaceManager.Instance.Initialize();
            
            TimeManager.Instance.Subscribe();
            
            HPManager.Instance.Initialize();
            
            CursesManager.Instance.Initialize();
            
            DungeonHeroUIManager.Instance.Initialize();
        }
    }
}
