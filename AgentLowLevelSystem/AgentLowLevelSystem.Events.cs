using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using Dungeon.DungeonGameEntry;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon.Character
{
    public partial class AgentLowLevelSystem : MonoBehaviour
    {
        private void SubscribeEvents()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnOneHeroDiedInDungeonEvent.EventId, OnOneHeroDiedInDungeonEventHandler);
        }

        private void OnOneHeroDiedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            var args = (OnOneHeroDiedInDungeonEvent)e;

            if(args.diedHero != GetComponent<HeroEntityBase>())
            {
                GameFrameworkLog.Info($"[AgentLowLevelSystem] {args.diedHero.name} 似了,这让{m_Properties.heroName} 非常不爽,屈服度增加10");
                ModifySubmissiveness(0);
            }
        }

        private void UnsubscribeEvents()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnOneHeroDiedInDungeonEvent.EventId, OnOneHeroDiedInDungeonEventHandler);
        }
    }
}
