using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnFightEndEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnFightEndEventArgs).GetHashCode();

        public OnFightEndEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnFightEndEventArgs Create()
        {
            OnFightEndEventArgs onFightEndEventArgs = ReferencePool.Acquire<OnFightEndEventArgs>();
            return onFightEndEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
