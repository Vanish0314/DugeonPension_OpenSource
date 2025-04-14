using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnFightStartEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnFightStartEventArgs).GetHashCode();

        public OnFightStartEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnFightStartEventArgs Create()
        {
            OnFightStartEventArgs onFightStartEventArgs = ReferencePool.Acquire<OnFightStartEventArgs>();
            return onFightStartEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
