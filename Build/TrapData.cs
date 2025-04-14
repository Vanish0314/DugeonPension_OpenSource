using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "NewTrapData", menuName = "Build/Trap Data")]
    public class TrapData : ScriptableObject, IPlaceableData
    {
        public TrapType trapType;
        public GameObject trapPrefab;
        public Vector2Int size = Vector2Int.one;
        public TrapCost cost;

        [System.Serializable]
        public struct TrapCost
        {
            public int material;
        }

        public GameObject GetPrefab()
        {
            return trapPrefab;
        }

        public Vector2Int GetSize()
        {
            return new Vector2Int(size.x, size.y);
        }
    }
}
