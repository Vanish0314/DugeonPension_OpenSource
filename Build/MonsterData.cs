using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "NewMonsterData", menuName = "Build/Monster Data")]
    public class MonsterData : ScriptableObject, IPlaceableData
    {
        public MonsterType monsterType;
        public Vector2Int size = Vector2Int.one;
        public Cost cost;
        
        public Vector2Int GetSize()
        {
            return new Vector2Int(size.x, size.y);
        }
    }
}
