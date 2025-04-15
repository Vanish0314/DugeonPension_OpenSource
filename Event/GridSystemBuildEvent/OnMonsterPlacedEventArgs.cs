using GameFramework;
using GameFramework.Event;

namespace Dungeon
{
    public class OnMonsterPlacedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnMonsterPlacedEventArgs).GetHashCode();

        public OnMonsterPlacedEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public MonsterData MonsterData
        {
            get;
            private set;
        }
        
        public static OnMonsterPlacedEventArgs Create(MonsterData monsterData)
        {
            OnMonsterPlacedEventArgs onMonsterPlacedEventArgs = ReferencePool.Acquire<OnMonsterPlacedEventArgs>();
            onMonsterPlacedEventArgs.MonsterData = monsterData;
            return onMonsterPlacedEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
