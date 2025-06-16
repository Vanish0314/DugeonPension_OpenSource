using Dungeon.Character;
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
            dialoguing = false;
            EnableEventSystem();
            GameEntry.Event.Fire(this, OnHeroStartExploreDungeonEvent.Create());
        }
        private void OnHeroCapturedSuccessfully()
        {
            dialoguing = false;
            EnableEventSystem();
            currentTalkingHero.GoDie();
            GameEntry.Event.Fire(this, OnOneHeroEndBeingCapturedEventArgs.Create(currentTalkingHero, true));
        }
        private void OnHeroCapturedFailed()
        {
            dialoguing = false;
            EnableEventSystem();
            currentTalkingHero.GoDie();
            GameEntry.Event.Fire(this, OnOneHeroEndBeingCapturedEventArgs.Create(currentTalkingHero, false));
        }
        private void OnHeroPersuadedEnd()
        {
            dialoguing = false;
            EnableEventSystem();
            GameEntry.Event.FireNow(this, OnOneHeroEndBeingPersuadedEventArgs.Create(currentTalkingHero));
        }
        private void ModifyGold(int value)
        {
            //TODO
            dialoguing = false;
            EnableEventSystem();
            GameFrameworkLog.Warning("[GalSystem.ModifyGold] 暂未实现");
        }
        private void ModifyExp(int value)
        {
            //TODO
            dialoguing = false;
            EnableEventSystem();
            GameFrameworkLog.Warning("[GalSystem.ModifyExp] 暂未实现");
        }
        private void ModifyHp(int value)
        {
            //TODO
            dialoguing = false;
            EnableEventSystem();
            GameFrameworkLog.Warning("[GalSystem.ModifyHp] 暂未实现");
        }
        private void ModifyMp(int value)
        {
            //TODO
            dialoguing = false;
            EnableEventSystem();
            GameFrameworkLog.Warning("[GalSystem.ModifyMp] 暂未实现");
        }
        private void ModifySubmissiveness(int value)
        {
            dialoguing = false;
            EnableEventSystem();
            currentTalkingHero.GetComponent<AgentLowLevelSystem>().ModifySubmissiveness(value);
        }

        private void OnTutorialDialogueEnd()
        {
            dialoguing = false;
            EnableEventSystem();
            Time.timeScale = 1;
        }

        private HeroEntityBase currentTalkingHero;
    }
}
