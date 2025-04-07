using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnDungeonEndEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnDungeonEndEventArgs).GetHashCode();

        public OnDungeonEndEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnDungeonEndEventArgs Create()
        {
            OnDungeonEndEventArgs onDungeonEndEventArgs = ReferencePool.Acquire<OnDungeonEndEventArgs>();
            return onDungeonEndEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}