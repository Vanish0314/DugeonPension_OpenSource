using GameFramework;
using GameFramework.Event;

namespace Dungeon
{
    public class OnBuildingPlacedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnBuildingPlacedEventArgs).GetHashCode();

        public OnBuildingPlacedEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public BuildingData BuildingData
        {
            get;
            private set;
        }
        
        public static OnBuildingPlacedEventArgs Create(BuildingData buildingData)
        {
            OnBuildingPlacedEventArgs onBuildingPlacedEventArgs = ReferencePool.Acquire<OnBuildingPlacedEventArgs>();
            onBuildingPlacedEventArgs.BuildingData = buildingData;
            return onBuildingPlacedEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
