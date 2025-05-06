using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "NewTrapData", menuName = "Build/Trap Data")]
    public class TrapData : ScriptableObject, IPlaceableData
    {
        public TrapType trapType;
        public Vector2Int size = Vector2Int.one;
        public Cost cost;
        
        public Vector2Int GetSize()
        {
            return new Vector2Int(size.x, size.y);
        }
    }
}
