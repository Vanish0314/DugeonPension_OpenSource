using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using Dungeon.Gal;
using GameFramework;
using GameFramework.Event;
using NUnit.Framework;
using UnityEngine;

namespace Dungeon.Tutorial
{
    public class TutorialHelper : MonoBehaviour
    {
        public float playTutorialDelay = 3.0f;

        public bool PlayerFirstManageFactory { get; private set; }

        public bool PlayerFirstDefendDungeon { get; private set; }

        public bool PlayerSecondDefendDungeon { get; private set; }

        private int m_playerGetInFactoryCount = 0;
        private int m_playerGetInDungeonCount = 0;
        private int m_playerGetAfterHeroArrive = 0;
        private bool shouldPlayFacTutorial = false;
        private bool shouldPlayDunTutorial = false;
        private bool shouldPlayAfterHeroArriveTutorial = false;
        private void OnEnable()
        {
            SubScribe();
        }

        private void OnDisable()
        {
            UnSubScribe();
        }

        private void FixedUpdate()
        {
            if (shouldPlayFacTutorial)
            {
                var tutorial = TutorialType.FirstFactory;
                switch (m_playerGetInFactoryCount)
                {
                    case 1:
                        tutorial = TutorialType.FirstFactory;
                        break;
                    case 2:
                        tutorial = TutorialType.SecondFactory;
                        break;
                    case 3:
                        tutorial = TutorialType.ThirdFactory;
                        break;

                    default:
                        return;
                }

                StartCoroutine(PlayTutorial(tutorial));
                shouldPlayFacTutorial = false;
            }
            if (shouldPlayDunTutorial)
            {
                var tutorial = TutorialType.FirstDungeon;
                switch (m_playerGetInDungeonCount)
                {
                    case 1:
                        tutorial = TutorialType.FirstDungeon;
                        break;
                    case 2:
                        tutorial = TutorialType.SecondDungeon;
                        break;
                    case 3:
                        tutorial = TutorialType.ThirdDungeon;
                        break;

                    default:
                        return;
                }

                StartCoroutine(PlayTutorial(tutorial));

                shouldPlayDunTutorial = false;
            }
            if (shouldPlayAfterHeroArriveTutorial)
            {
                var tutorial = TutorialType.FirstTeamArrival;
                switch (m_playerGetAfterHeroArrive)
                {
                    case 1:
                        tutorial = TutorialType.FirstTeamArrival;
                        break;
                    case 2:
                        tutorial = TutorialType.SecondTeamArrival;
                        break;
                    case 3:
                        tutorial = TutorialType.ThirdTeamArrival;
                        break;

                    default:
                        return;
                }

                StartCoroutine(PlayTutorial(tutorial));

                shouldPlayAfterHeroArriveTutorial = false;
            }

        }

        private IEnumerator PlayTutorial(TutorialType tutorialType)
        {
#if UNITY_EDITOR
            GameFrameworkLog.Info($"[TutorialHelper] 准备播放教程:{tutorialType.ToString()}");
#endif

            yield return new WaitForSeconds(playTutorialDelay);

            GameFrameworkLog.Info($"[TutorialHelper] 开始播放教程:{tutorialType.ToString()}");

            DungeonGameEntry.DungeonGameEntry.GalSystem.PlayTutorialDialogue(tutorialType);
        }

        private void SubScribe()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnSwitchedToMetroplisProcedureEvent.EventId, OnPlayerSwitchToMetroplisEventHandler);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnSwitchedToDungeonPlacingProcedureEvent.EventId, OnPlayerSwitchToDungeonEventHandler);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnHeroTeamDiedInDungeonEvent.EventId, OnHeroTeamDiedInDungeonEventHandler);
        }

        private void OnHeroTeamDiedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            playerWinnedInDungeon = true;
        }

        private void OnHeroStartExploreDungeonEventHandler(object sender, GameEventArgs e)
        {
            m_playerGetAfterHeroArrive++;
            shouldPlayAfterHeroArriveTutorial = true;

#if UNITY_EDITOR
            GameFrameworkLog.Info($"[TutorialHelper] 教程：玩家进入地牢 {m_playerGetAfterHeroArrive} 次, 改播放教程了");
#endif
        }

        bool playerWinnedInDungeon = true;
        private void OnPlayerSwitchToMetroplisEventHandler(object sender, GameEventArgs e)
        {
            if (playerWinnedInDungeon)
            {

                m_playerGetInFactoryCount++;
                shouldPlayFacTutorial = true;
                playerWinnedInDungeon = false;

#if UNITY_EDITOR
                GameFrameworkLog.Info($"[TutorialHelper] 教程：玩家进入工厂 {m_playerGetInFactoryCount} 次, 改播放教程了");
#endif
            }
        }
        private void OnPlayerSwitchToDungeonEventHandler(object sender, GameEventArgs e)
        {
            m_playerGetInDungeonCount++;
            shouldPlayDunTutorial = true;

#if UNITY_EDITOR
            GameFrameworkLog.Info($"[TutorialHelper] 教程：玩家进入地牢 {m_playerGetInDungeonCount} 次, 改播放教程了");
#endif
        }


        private void UnSubScribe()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnSwitchedToMetroplisProcedureEvent.EventId, OnPlayerSwitchToMetroplisEventHandler);
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnSwitchedToDungeonPlacingProcedureEvent.EventId, OnPlayerSwitchToDungeonEventHandler);
        }
    }
}
