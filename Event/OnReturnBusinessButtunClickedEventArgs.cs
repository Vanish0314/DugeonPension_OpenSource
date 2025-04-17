using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public class OnReturnBusinessButtunClickedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnReturnBusinessButtunClickedEventArgs).GetHashCode();

        public OnReturnBusinessButtunClickedEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnReturnBusinessButtunClickedEventArgs Create()
        {
            OnReturnBusinessButtunClickedEventArgs onReturnBusinessButtunClickedEventArgs = ReferencePool.Acquire<OnReturnBusinessButtunClickedEventArgs>();
            return onReturnBusinessButtunClickedEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
