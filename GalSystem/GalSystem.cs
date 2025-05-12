using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dungeon.Character.Hero;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using Dungeon.Overload;
using GameFramework;
using GameFramework.Event;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Gal
{
    public partial class GalSystem : MonoBehaviour
    {
#if UNITY_EDITOR
        [InfoBox(
         "对话树管理使用注意:\n" +
         "1. 在子物体 Dao Manager 中配置对话中正在说话的人的图片名称等\n" +
         "2. 在子物体 Dto Manager 中配置对话树\n" +
         "3. 在配置对话树时,Actor Paramater 只要填入左边的名字,不要填入右边的名字\n" +
         "4. 注意,对话树paramater左边的名字会用于资源查找,名字不对可能导致图片不显示,注意查看log\n" +
         "5. 配置Dao 时也需要注意名字\n" +
         "6. 通过Create->GalSystem->Dungeon Gal Actor来创建Dao\n" +
         "7. 在对话结束时,需要调用: Reflected -> SendMessage -> System -> (string) -> 填入参数\n" +
         "8. 参数规则在下面\n" +
         "9. 方法名称: “GalParser”" +
         "10. 配置了的Dto的勇者,需要在DungeonSystem -> AdventurersGuildSystem中有相应勇者的配置"
        )]
        [LabelText("本策划记住啦")] public bool MeRemberered0;

        [InfoBox(
            "填入参数规则(在<>中的内容):\n" +
            "1. 勇者初次亮相结束 : <-OnHeroFinishedFirstAppearance>\n" +
            "2. 勇者捕获对话结束,结果是成功被捕获 : <-OnHeroCapturedSuccessfully>\n" +
            "3. 勇者捕获对话结束,结果是失败被捕获 : <-OnHeroCapturedFailed>\n" +
            "4. 勇者说服对话结束: <-OnHeroPersuadedEnd>\n"
        )]
        [LabelText("本策划所有对话树都填啦")] public bool MeRemberered1;

        [InfoBox(
            "如果需要更改数值,方法如下({x}是需要修改的数值):\n" +
            "1. 增加(扣除)金币: <-ModifyGold>={x}\n" +
            "2. 增加(扣除)经验: <-ModifyExp>={x}\n" +
            "3. 增加(扣除)体力: <-ModifyHp>={x}\n" +
            "4. 增加(扣除)魔法: <-ModifyMp>={x}\n" +
            "5. 增加(扣除)屈服度: <-ModifySubmissiveness>={x}\n"
        )]
        [LabelText("本还有需要的话会提TODO的")] public bool MeRemberered2;
#endif

        public DungeonGalActor GetDungeonGalActor(string name)
        {
            return m_Dao.TryGetActor(name, out IDialogueActor actor) ? (DungeonGalActor)actor : null;
        }
        public Transform GetControllerTransform()
        {
            return m_DialogueTreeController.transform;
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

            GameEntry.Event.Subscribe(OnOneHeroStartBeingPersuadedEventArgs.EventId, OnOneHeroStartBeingPersuadedEventHandler);
            GameEntry.Event.Subscribe(OnOneHeroStartBeingCapturedEventArgs.EventId, OnOneHeroStartBeingCapturedEventHandler);

            GameEntry.Event.Subscribe(OnOneHeroEndBeingCapturedEventArgs.EventId, OnHeroStartExploreDungeonEventHandler);
            GameEntry.Event.Subscribe(OnOneHeroEndBeingPersuadedEventArgs.EventId, OnHeroStartExploreDungeonEventHandler);
        }

        private void OnOneHeroStartBeingCapturedEventHandler(object sender, GameEventArgs e)
        {
            if (e is OnOneHeroStartBeingCapturedEventArgs args)
            {
                m_GalGUI.SetActive(true);
                m_DialogueTreeController.gameObject.SetActive(true);

                var dto = GetDto(args.HeroEntity.HeroName, DialogueType.Capture);

                StartDialogue(dto, args.HeroEntity.HeroName);
            }
        }

        private void OnOneHeroStartBeingPersuadedEventHandler(object sender, GameEventArgs e)
        {
            if (e is OnOneHeroStartBeingPersuadedEventArgs args)
            {
                m_GalGUI.SetActive(true);
                m_DialogueTreeController.gameObject.SetActive(true);

                var dto = GetDto(args.HeroEntity.HeroName, DialogueType.Convincing);

                StartDialogue(dto, args.HeroEntity.HeroName);
            }
        }

        private void OnHeroArrivedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            if (e is OnHeroArrivedInDungeonEvent arg)
            {
                m_GalGUI.SetActive(true);
                m_DialogueTreeController.gameObject.SetActive(true);

                var dto = GetDto(arg.MainHero.HeroName, DialogueType.EnterDungeon);
                if (dto == null)
                {
                    GameFrameworkLog.Error($"[GalSystem][对话树] 未找到名字为 {arg.MainHero.HeroName} 的 DialogueTree");
                }

                StartDialogue(dto, arg.MainHero.HeroName);
            }
        }

        private void OnHeroStartExploreDungeonEventHandler(object sender, GameEventArgs e)
        {
            m_GalGUI.SetActive(false);
            m_DialogueTreeController.gameObject.SetActive(false);
        }
        private void StartDialogue(DialogueTree dialogueTree, string heroName)
        {

            // Done in dto manager
            // foreach (var param in dialogueTree.actorParameters)
            // {
            //     if (param == null || string.IsNullOrEmpty(param.name))
            //         continue;

            //     var actor = GetDungeonGalActor(param.name);

            //     if (actor != null)
            //     {
            //         param.actor = actor;
            //     }
            //     else
            //     {
            //         GameFrameworkLog.Error($"[GalSystem] 未找到名字为 {param.name} 的 DialogueActor");
            //     }
            // }

            m_DialogueTreeController.updateMode = Graph.UpdateMode.FixedUpdate;

            m_DialogueTreeController.StartDialogue(dialogueTree, GetDungeonGalActor(heroName), null);
        }
        private DialogueTree GetDto(string name, DialogueType type)
        {
            return m_Dto.GetClonedHeroDialogue(name, type);
        }
        private void UnSubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
            GameEntry.Event.Unsubscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);

            GameEntry.Event.Unsubscribe(OnOneHeroStartBeingPersuadedEventArgs.EventId, OnOneHeroStartBeingPersuadedEventHandler);
            GameEntry.Event.Unsubscribe(OnOneHeroStartBeingCapturedEventArgs.EventId, OnOneHeroStartBeingCapturedEventHandler);
        }

        private GameObject m_GalGUI;
        private DtoManager m_Dto;
        private DaoManager m_Dao;
        DialogueTreeController m_DialogueTreeController;


    }
}
