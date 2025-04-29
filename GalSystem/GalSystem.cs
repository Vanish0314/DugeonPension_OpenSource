using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Hero;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using GameFramework.Event;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Gal
{
    public class GalSystem : MonoBehaviour
    {
#if UNITY_EDITOR
        [InfoBox(
         "对话树管理使用注意:\n" +
         "1. 在子物体 Dao Manager 中配置对话中正在说话的人的图片名称等\n" +
         "2. 在子物体 Dto Manager 中配置对话树\n" +
         "3. 在配置对话树时,Actor Paramater 只要填入左边的名字,不要填入右边的名字\n" +
         "4. 注意,对话树paramater左边的名字会用于资源查找,名字不对可能导致图片不显示,注意查看log\n" +
         "5. 配置Dao 时也需要注意名字\n" +
         "6. 通过Create->GalSystem->Dungeon Gal Actor来创建Dao"
        )]
        [LabelText("本策划记住啦")] public bool remberered;
#endif

        public void HeroEndedDialogureBeforeDungeonExplore()
        {
            GameEntry.Event.Fire(this, OnHeroStartExploreDungeonEvent.Create());
        }
        public DungeonGalActor GetDungeonGalActor(string name)
        {
            return m_Dao.TryGetActor(name, out IDialogueActor actor) ? (DungeonGalActor)actor : null;
        }

        private void Start()
        {
            m_GalGUI = transform.GetChild(0).gameObject;
            m_Dto = transform.GetComponentInChildren<DtoManager>();
            m_Dao = transform.GetComponentInChildren<DaoManager>();
            m_DialogueTreeController = transform.GetComponentInChildren<DialogueTreeController>();

            m_GalGUI.SetActive(false);

            SubscribeEvents();
        }
        void OnDestroy()
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
            GameEntry.Event.Subscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);
        }

        private void OnHeroArrivedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            m_GalGUI.SetActive(true);
            m_DialogueTreeController.gameObject.SetActive(true);
        }

        private void OnHeroStartExploreDungeonEventHandler(object sender, GameEventArgs e)
        {
            m_GalGUI.SetActive(false);
            m_DialogueTreeController.gameObject.SetActive(false);
        }

        private void UnSubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
            GameEntry.Event.Unsubscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);
        }

        private void OpenHeroEnterDungeonDialogue(HeroEntityBase hero)
        {

        }

        private void OpenHeroBeingPersuadedDialogue(HeroEntityBase hero)
        {

        }

        private void OpenHeroBeingCapturedDialogue(HeroEntityBase hero)
        {

        }

        private GameObject m_GalGUI;
        private DtoManager m_Dto;
        private DaoManager m_Dao;
        DialogueTreeController m_DialogueTreeController;


    }
}
