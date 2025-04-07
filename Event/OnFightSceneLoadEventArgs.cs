using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnFightSceneLoadEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnFightSceneLoadEventArgs).GetHashCode();

        public OnFightSceneLoadEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnFightSceneLoadEventArgs Create()
        {
            OnFightSceneLoadEventArgs onFightSceneLoadEventArgs = ReferencePool.Acquire<OnFightSceneLoadEventArgs>();
            return onFightSceneLoadEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
