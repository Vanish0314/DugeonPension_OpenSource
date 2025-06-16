using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "BuildingBlueprintData", menuName = "Build/Building Blueprint Data")]
    public class BuildingBlueprintData : ScriptableObject
    {
        public int BuildingID;
        
        [Header("UI")]
        public string BuildingName;
        [TextArea(1,8)]
        public string BuildingDescription;
        public Sprite BuildingIcon;
        
        [Header("Need")]
        public BuildingBlueprintData[] NeedBuildings;
        public int NeedLevel;
        public int BuildingCost;
        
        [Header("Status")]
        public bool IsUnlocked = false;
    }
}
