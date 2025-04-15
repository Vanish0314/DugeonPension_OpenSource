using GameFramework;
using GameFramework.Event;

namespace Dungeon
{
    public class OnTrapPlacedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnTrapPlacedEventArgs).GetHashCode();

        public OnTrapPlacedEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public TrapData TrapData
        {
            get;
            private set;
        }
        
        public static OnTrapPlacedEventArgs Create(TrapData trapData)
        {
            OnTrapPlacedEventArgs onTrapPlacedEventArgs = ReferencePool.Acquire<OnTrapPlacedEventArgs>();
            onTrapPlacedEventArgs.TrapData = trapData;
            return onTrapPlacedEventArgs;
        }
        
        public override void Clear()
        {
            TrapData = null;
        }
    }
}
