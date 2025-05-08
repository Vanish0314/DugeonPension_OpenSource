using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dungeon
{
    [System.Serializable]
    public struct Cost
    {
        [Header("经营")]
        public int gold;
        public int stone;
        public int wood;
        public int researchPoints;
        public int expBall;
        [Header("战斗")]
        public int magicPower;
        public int trapMaterial;
        public int conjuration;
    }
    
    [CreateAssetMenu(fileName = "NewBuildingData", menuName = "Build/Building Data")]
    public class BuildingData : ScriptableObject, IPlaceableData
    {
        public BuildingType buildingType; // 建筑唯一标识
        public Vector2Int size = Vector2Int.one; // 占用格子大小
        public Cost cost;

        public Vector2Int GetSize()
        {
            return new Vector2Int(size.x, size.y);
        }
    }
}
