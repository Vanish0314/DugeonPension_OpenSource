using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnFightSettlementContinueEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnFightSettlementContinueEventArgs).GetHashCode();
        public OnFightSettlementContinueEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        
        public static OnFightSettlementContinueEventArgs Create()
        {
            OnFightSettlementContinueEventArgs onFightSettlementContinueEventArgs = ReferencePool.Acquire<OnFightSettlementContinueEventArgs>();
            return onFightSettlementContinueEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}