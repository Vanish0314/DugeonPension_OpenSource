using Dungeon.Common;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public class TryPlaceBuildingEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(TryPlaceBuildingEventArgs).GetHashCode();
        
        public TryPlaceBuildingEventArgs()
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

        public MonoPoolItem BuildingItem
        {
            get;
            private set;
        }

        public BuildingData BuildingData
        {
            get;
            private set;
        }
        public static TryPlaceBuildingEventArgs Create(Vector3 worldPosition, MonoPoolItem buildingItem, BuildingData buildingData)
        {
            TryPlaceBuildingEventArgs tryPlaceTrapEventArgs = ReferencePool.Acquire<TryPlaceBuildingEventArgs>();
            tryPlaceTrapEventArgs.WorldPosition = worldPosition;
            tryPlaceTrapEventArgs.BuildingItem = buildingItem;
            tryPlaceTrapEventArgs.BuildingData = buildingData;
            return tryPlaceTrapEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
