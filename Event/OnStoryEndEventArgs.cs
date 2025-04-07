using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnStoryEndEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnStoryEndEventArgs).GetHashCode();

        public OnStoryEndEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnStoryEndEventArgs Create()
        {
            OnStoryEndEventArgs onStoryEndEventArgs = ReferencePool.Acquire<OnStoryEndEventArgs>();
            return onStoryEndEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}