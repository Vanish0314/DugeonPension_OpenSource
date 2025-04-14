using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnStartFightButtonClickEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnStartFightButtonClickEventArgs).GetHashCode();

        public OnStartFightButtonClickEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnStartFightButtonClickEventArgs Create()
        {
            OnStartFightButtonClickEventArgs onStartFightButtonClickEventArgs = ReferencePool.Acquire<OnStartFightButtonClickEventArgs>();
            return onStartFightButtonClickEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}