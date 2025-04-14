using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnBusinessEndEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnBusinessEndEventArgs).GetHashCode();

        public OnBusinessEndEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnBusinessEndEventArgs Create()
        {
            OnBusinessEndEventArgs onBusinessEndEventArgs = ReferencePool.Acquire<OnBusinessEndEventArgs>();
            return onBusinessEndEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
