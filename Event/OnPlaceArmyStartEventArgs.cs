using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnPlaceArmyStartEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnPlaceArmyStartEventArgs).GetHashCode();

        public OnPlaceArmyStartEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnPlaceArmyStartEventArgs Create()
        {
            OnPlaceArmyStartEventArgs onPlaceArmyStartEventArgs = ReferencePool.Acquire<OnPlaceArmyStartEventArgs>();
            return onPlaceArmyStartEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
