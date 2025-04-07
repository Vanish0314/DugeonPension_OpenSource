using UnityEngine;

namespace Dungeon
{
    /// <summary>
    /// 建筑数据基类（暂时）
    /// </summary>
    [CreateAssetMenu(fileName = "NewBuildingData", menuName = "Building/Building Data")]
    public class BuildingData : ScriptableObject
    {
        public string buildingId;          // 建筑唯一标识
        public GameObject buildingPrefab;  // 关联的Prefab
        public Vector2Int size = Vector2Int.one; // 占用格子大小
        public BuildingCost cost;

        [System.Serializable]
        public struct BuildingCost
        {
            public int gold;
            public int stone;
            public int magicPower;
            public int material;
        }
    }
}