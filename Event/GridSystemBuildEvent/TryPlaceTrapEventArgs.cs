using Dungeon.Common;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public class TryPlaceTrapEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(TryPlaceTrapEventArgs).GetHashCode();
        
        public TryPlaceTrapEventArgs()
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

        public MonoPoolItem TrapItem
        {
            get;
            private set;
        }

        public TrapData TrapData
        {
            get;
            private set;
        }
        public static TryPlaceTrapEventArgs Create(Vector3 worldPosition, MonoPoolItem trapItem, TrapData trapData)
        {
            TryPlaceTrapEventArgs tryPlaceTrapEventArgs = ReferencePool.Acquire<TryPlaceTrapEventArgs>();
            tryPlaceTrapEventArgs.WorldPosition = worldPosition;
            tryPlaceTrapEventArgs.TrapItem = trapItem;
            tryPlaceTrapEventArgs.TrapData = trapData;
            return tryPlaceTrapEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
