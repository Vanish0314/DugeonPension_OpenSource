using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnOneHeroDiedInDungeonEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnOneHeroDiedInDungeonEvent).GetHashCode();
        public HeroEntityBase diedHero;

        public OnOneHeroDiedInDungeonEvent()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public static OnOneHeroDiedInDungeonEvent Create(HeroEntityBase diedHero)
        {
            OnOneHeroDiedInDungeonEvent onDungeonEndEventArgs = ReferencePool.Acquire<OnOneHeroDiedInDungeonEvent>();
            onDungeonEndEventArgs.diedHero = diedHero;
            return onDungeonEndEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}