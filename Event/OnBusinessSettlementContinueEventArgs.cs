using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnBusinessSettlementContinueEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnBusinessSettlementContinueEventArgs).GetHashCode();
        public OnBusinessSettlementContinueEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        
        public static OnBusinessSettlementContinueEventArgs Create()
        {
            OnBusinessSettlementContinueEventArgs onBusinessSettlementContinueEventArgs = ReferencePool.Acquire<OnBusinessSettlementContinueEventArgs>();
            return onBusinessSettlementContinueEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}