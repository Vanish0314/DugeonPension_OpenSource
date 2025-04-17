using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnGameStartButtonClickEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnGameStartButtonClickEventArgs).GetHashCode();

        public OnGameStartButtonClickEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnGameStartButtonClickEventArgs Create()
        {
            OnGameStartButtonClickEventArgs onGameStartButtonClickEventArgs = ReferencePool.Acquire<OnGameStartButtonClickEventArgs>();
            return onGameStartButtonClickEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}