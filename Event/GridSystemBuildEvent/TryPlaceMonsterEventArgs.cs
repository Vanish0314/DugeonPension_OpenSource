using Dungeon.Common;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public class TryPlaceMonsterEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(TryPlaceMonsterEventArgs).GetHashCode();
        
        public TryPlaceMonsterEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public Vector3 WorldPosition
        {
            get;
            private set;
        }

        public MonoPoolItem MonsterItem
        {
            get;
            private set;
        }

        public MonsterData MonsterData
        {
            get;
            private set;
        }
        public static TryPlaceMonsterEventArgs Create(Vector3 worldPosition, MonoPoolItem monsterItem, MonsterData monsterData)
        {
            TryPlaceMonsterEventArgs tryPlaceMonsterEventArgs = ReferencePool.Acquire<TryPlaceMonsterEventArgs>();
            tryPlaceMonsterEventArgs.WorldPosition = worldPosition;
            tryPlaceMonsterEventArgs.MonsterItem = monsterItem;
            tryPlaceMonsterEventArgs.MonsterData = monsterData;
            return tryPlaceMonsterEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
