using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnBusinessStartEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnBusinessStartEventArgs).GetHashCode();

        public OnBusinessStartEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnBusinessStartEventArgs Create()
        {
            OnBusinessStartEventArgs onBusinessStartEventArgs = ReferencePool.Acquire<OnBusinessStartEventArgs>();
            return onBusinessStartEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
