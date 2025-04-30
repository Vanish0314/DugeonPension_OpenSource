using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Hero;
using Dungeon.Evnents;
using Dungeon.Overload;
using UnityEngine;

namespace Dungeon.Gal
{
    public partial class GalSystem : MonoBehaviour
    {
        private void OnHeroFinishedFirstAppearance()
        {
            GameEntry.Event.Fire(this, OnHeroStartExploreDungeonEvent.Create());
        }
        private void OnHeroCapturedSuccessfully()
        {
            //TODO
            currentTalkingHero.GoDie();
            GameEntry.Event.Fire(this, OnOneHeroEndBeingCapturedEventArgs.Create(currentTalkingHero,true));
        }
        private void OnHeroCapturedFailed()
        {
            //TODO
            currentTalkingHero.GoDie();
            GameEntry.Event.Fire(this, OnOneHeroEndBeingCapturedEventArgs.Create(currentTalkingHero, false));
        }
        private void OnHeroPersuadedEnd()
        {
            //TODO
            GameEntry.Event.Fire(this,OnOneHeroStartBeingPersuadedEventArgs.Create(currentTalkingHero));
        }
        private void ModifyGold(int value)
        {
            //TODO
        }
        private void ModifyExp(int value)
        {
            //TODO
        }
        private void ModifyHp(int value)
        {
            //TODO
        }
        private void ModifyMp(int value)
        {
            //TODO
        }
        private void ModifySubmissiveness(int value)
        {
            //TODO
        }

        private HeroEntityBase currentTalkingHero;
    }
}
