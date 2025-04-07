using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnPlaceArmyEndEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnPlaceArmyEndEventArgs).GetHashCode();

        public OnPlaceArmyEndEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnPlaceArmyEndEventArgs Create()
        {
            OnPlaceArmyEndEventArgs onPlaceArmyEndEventArgs = ReferencePool.Acquire<OnPlaceArmyEndEventArgs>();
            return onPlaceArmyEndEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
