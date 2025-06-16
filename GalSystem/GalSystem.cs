using Dungeon.Character;
using Dungeon.Evnents;
using Dungeon.Overload;
using GameFramework;
using GameFramework.Event;
using NodeCanvas.DialogueTrees;
using NodeCanvas.DialogueTrees.UI.Examples;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

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
            "4. 勇者说服对话结束: <-OnHeroPersuadedEnd>\n" +
            "5. 教程对话结束: <-OnTutorialDialogueEnd>\n"
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

        public void PlayTutorialDialogue(TutorialType tutorial)
        {
            if (string.IsNullOrEmpty(name))
            {
                GameFrameworkLog.Error($"[GalSystem] 启动教程对话,对话名字不能为空");
                return;
            }

            var dto = GetDto(tutorial);
            if (dto == null)
            {
                GameFrameworkLog.Error($"[GalSystem] 启动教程对话,找不到{tutorial.ToString()}对应的Dto");
                return;
            }

            StartTutorialDialogue(dto);
        }

        public void PlayCorruptLevelDialogue(MetropolisHeroBase hero, DialogueType dialogueType)
        {
            m_DialogueTreeController.gameObject.SetActive(true);

            var dto = GetDto(hero.HeroName, dialogueType);

            if (dto != null)
            {
                StartDialogue(dto, hero);
            }
            else
            {
                GameFrameworkLog.Error($"[GalSystem] 找不到{hero}对应的Dto");
            }
        }
        
        private void Start()
        {
            m_GalGUI = GetComponentInChildren<DialogueUGUI>();
            m_Dto = transform.GetComponentInChildren<DtoManager>();
            m_Dao = transform.GetComponentInChildren<DaoManager>();
            m_DialogueTreeController = transform.GetComponentInChildren<DialogueTreeController>();

            m_GalGUI.Hide();

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

            GameEntry.Event.Subscribe(OnOneHeroEndBeingCapturedEventArgs.EventId, OnHeroEndBeingCapturedEventHandler);
            GameEntry.Event.Subscribe(OnOneHeroEndBeingPersuadedEventArgs.EventId, OnHeroEndBeingPersuadedEventHandler);
        }

        private void OnHeroEndBeingPersuadedEventHandler(object sender, GameEventArgs e)
        {
            if (e is OnOneHeroStartBeingPersuadedEventArgs args)
            {

            }
        }


        private void OnHeroEndBeingCapturedEventHandler(object sender, GameEventArgs e)
        {
            if (e is OnOneHeroEndBeingCapturedEventArgs args)
            {

            }
        }


        private void OnOneHeroStartBeingCapturedEventHandler(object sender, GameEventArgs e)
        {
            if (e is OnOneHeroStartBeingCapturedEventArgs args)
            {
                m_DialogueTreeController.gameObject.SetActive(true);

                var dto = GetDto(args.HeroEntity.HeroName, DialogueType.Capture);

                if (dto != null)
                {
                    StartDialogue(dto, args.HeroEntity);
                }
                else
                {
                    GameFrameworkLog.Error($"[GalSystem] 找不到{args.HeroEntity.HeroName}对应的Dto");
                }
            }
        }

        private void OnOneHeroStartBeingPersuadedEventHandler(object sender, GameEventArgs e)
        {
            if (e is OnOneHeroStartBeingPersuadedEventArgs args)
            {
                m_DialogueTreeController.gameObject.SetActive(true);

                var dto = GetDto(args.HeroEntity.HeroName, DialogueType.Convincing);

                StartDialogue(dto, args.HeroEntity);
            }
        }

        private void OnHeroArrivedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            if (e is OnHeroArrivedInDungeonEvent arg)
            {
                m_DialogueTreeController.gameObject.SetActive(true);

                var dto = GetDto(arg.MainHero.HeroName, DialogueType.EnterDungeon);
                if (dto == null)
                {
                    GameFrameworkLog.Error($"[GalSystem][对话树] 未找到名字为 {arg.MainHero.HeroName} 的 DialogueTree");
                }

                StartDialogue(dto, arg.MainHero);
            }
        }

        private void OnHeroStartExploreDungeonEventHandler(object sender, GameEventArgs e)
        {
            m_GalGUI.Hide();
            // m_DialogueTreeController.gameObject.SetActive(false);
        }

        private void StartDialogue(DialogueTree dialogueTree, MetropolisHeroBase hero)
        {
            m_DialogueTreeController.gameObject.SetActive(true);
            m_DialogueTreeController.updateMode = Graph.UpdateMode.FixedUpdate;
            m_DialogueTreeController.StartDialogue(dialogueTree, GetDungeonGalActor(hero.HeroName), null);
            
            dialoguing = true;
            DisableEventSystem();
            
            GameEntry.UI.GetUIForm(EnumUIForm.BuildForm)?.Close();
            GameEntry.UI.GetUIForm(EnumUIForm.PlaceArmyForm)?.Close();
            GameEntry.UI.GetUIForm(EnumUIForm.CurseForm)?.Close();
        }
        
        private void StartDialogue(DialogueTree dialogueTree, HeroEntityBase hero)
        {
            currentTalkingHero = hero;

            m_DialogueTreeController.gameObject.SetActive(true);
            m_DialogueTreeController.updateMode = Graph.UpdateMode.FixedUpdate;
            m_DialogueTreeController.StartDialogue(dialogueTree, GetDungeonGalActor(hero.HeroName), null);
            
            dialoguing = true;
            DisableEventSystem();
            
            GameEntry.UI.GetUIForm(EnumUIForm.BuildForm)?.Close();
            GameEntry.UI.GetUIForm(EnumUIForm.PlaceArmyForm)?.Close();
            GameEntry.UI.GetUIForm(EnumUIForm.CurseForm)?.Close();
        }
        private void StartTutorialDialogue(DialogueTree dialogueTree)
        {
            var dao = m_Dao.GetTutorialActor();

            if (dao == null)
            {
                GameFrameworkLog.Error($"[GalSystem] 未找到Tutorial Actor ,没有找到教程大姐姐,无法播放教程");
                return;
            }

            if(pauseTimeWhilePlayTuytorial)
                Time.timeScale = 0.01f;

            m_DialogueTreeController.gameObject.SetActive(true);
            m_DialogueTreeController.updateMode = Graph.UpdateMode.FixedUpdate;
            m_DialogueTreeController.StartDialogue(dialogueTree, dao, null);
            
            dialoguing = true;
            DisableEventSystem();
            
            GameEntry.UI.GetUIForm(EnumUIForm.BuildForm)?.Close();
            GameEntry.UI.GetUIForm(EnumUIForm.PlaceArmyForm)?.Close();
            GameEntry.UI.GetUIForm(EnumUIForm.CurseForm)?.Close();
        }
        private DialogueTree GetDto(string name, DialogueType type)
        {
            return m_Dto.GetClonedHeroDialogue(name, type);
        }
        private DialogueTree GetDto(TutorialType tutorial)
        {
            return m_Dto.GetClonedTutorialDialogue(tutorial);
        }
        private void UnSubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
            GameEntry.Event.Unsubscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);

            GameEntry.Event.Unsubscribe(OnOneHeroStartBeingPersuadedEventArgs.EventId, OnOneHeroStartBeingPersuadedEventHandler);
            GameEntry.Event.Unsubscribe(OnOneHeroStartBeingCapturedEventArgs.EventId, OnOneHeroStartBeingCapturedEventHandler);
        }

        public bool pauseTimeWhilePlayTuytorial = false;
        DialogueUGUI m_GalGUI;
        private DtoManager m_Dto;
        private DaoManager m_Dao;
        DialogueTreeController m_DialogueTreeController;

        public bool dialoguing = false;
        private EventSystem cachedEventSystem; // 缓存引用

        void DisableEventSystem() {
            cachedEventSystem = EventSystem.current; // 先保存
            if (cachedEventSystem != null) {
                cachedEventSystem.enabled = false;
            }
        }

        void EnableEventSystem() {
            if (cachedEventSystem != null) {
                cachedEventSystem.enabled = true;
            }
        }
    }
}
