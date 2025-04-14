using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    [Obsolete]
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


    public sealed class OnStartNewGameButtonClickEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnStartNewGameButtonClickEvent).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public override void Clear()
        {
        }

        public static OnStartNewGameButtonClickEvent Create()
        {
            var handler = ReferencePool.Acquire<OnStartNewGameButtonClickEvent>();
            return handler;
        }
    }

    public sealed class OnContinueGameButtonClickEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnContinueGameButtonClickEvent).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public override void Clear()
        {
        }

        public static OnContinueGameButtonClickEvent Create()
        {
            var handler = ReferencePool.Acquire<OnContinueGameButtonClickEvent>();
            return handler;
        }
    }
}