using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Hero;
using Dungeon.Evnents;
using Dungeon.Overload;
using GameFramework;
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
            currentTalkingHero.GoDie();
            GameEntry.Event.Fire(this, OnOneHeroEndBeingCapturedEventArgs.Create(currentTalkingHero,true));
        }
        private void OnHeroCapturedFailed()
        {
            currentTalkingHero.GoDie();
            GameEntry.Event.Fire(this, OnOneHeroEndBeingCapturedEventArgs.Create(currentTalkingHero, false));
        }
        private void OnHeroPersuadedEnd()
        {
            GameEntry.Event.Fire(this,OnOneHeroStartBeingPersuadedEventArgs.Create(currentTalkingHero));
        }
        private void ModifyGold(int value)
        {
            //TODO
            GameFrameworkLog.Warning("[GalSystem.ModifyGold] 暂未实现");
        }
        private void ModifyExp(int value)
        {
            //TODO
            GameFrameworkLog.Warning("[GalSystem.ModifyExp] 暂未实现");
        }
        private void ModifyHp(int value)
        {
            //TODO
            GameFrameworkLog.Warning("[GalSystem.ModifyHp] 暂未实现");
        }
        private void ModifyMp(int value)
        {
            //TODO
            GameFrameworkLog.Warning("[GalSystem.ModifyMp] 暂未实现");
        }
        private void ModifySubmissiveness(int value)
        {
            currentTalkingHero.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>().ModifySubmissiveness(value);
        }

        private HeroEntityBase currentTalkingHero;
    }
}
