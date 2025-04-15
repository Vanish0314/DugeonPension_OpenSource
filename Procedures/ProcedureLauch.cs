using System;
using Dungeon.DungeonEntity;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using GameFramework;
using GameFramework.Event;
using GameFramework.Procedure;
using GluonGui.Dialog;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Dungeon.Procedure
{
    /// <summary>
    /// ProcedureLauch
    /// 1. 获取存档信息与用户设置
    /// 2. 初始化部分系统
    /// 3. 一帧后切换到ProcedureOpenning
    /// </summary>
    public class ProcedureLauch : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            // 声音配置：根据用户配置数据，设置即将使用的声音选项
            InitSoundSettings();

            InitDataTables();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            ChangeState<ProcedureOpenning>(procedureOwner);
        }

        private void InitDataTables()
        {
            GameEntry.Data.PreLoadAllData();
        }

        private void InitSoundSettings()
        {
            // GameEntry.Sound.Mute("Music", GameEntry.Setting.GetBool(Constant.Setting.MusicMuted, false));
            // GameEntry.Sound.SetVolume("Music", GameEntry.Setting.GetFloat(Constant.Setting.MusicVolume, 0.3f));
            // GameEntry.Sound.Mute("Sound", GameEntry.Setting.GetBool(Constant.Setting.SoundMuted, false));
            // GameEntry.Sound.SetVolume("Sound", GameEntry.Setting.GetFloat(Constant.Setting.SoundVolume, 1f));
            // GameEntry.Sound.Mute("UISound", GameEntry.Setting.GetBool(Constant.Setting.UISoundMuted, false));
            // GameEntry.Sound.SetVolume("UISound", GameEntry.Setting.GetFloat(Constant.Setting.UISoundVolume, 1f));
            // Log.Info("Init sound settings complete.");
        }
    }

    /// <summary>
    /// ProcedureOpenning
    /// 1. 播放开场Logo
    /// 2. 播放完毕切换到ProcedureStartScene
    /// 3. 播放动画时可以进行一些加载工作
    /// </summary>
    public class ProcedureOpenning : ProcedureBase
    {
        private ProcedureOwner mOwner;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mOwner = procedureOwner;

            SubscribeEvent();
        }

        override protected void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            UnsubscribeEvent();
        }

        private void SubscribeEvent()
        {
            GameEntry.Event.Subscribe(OnOpenningLogoEndEvent.EventId, OnOpenningLogoEndEventHandler);
        }

        private void OnOpenningLogoEndEventHandler(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureChangeToStartScene>(mOwner);
        }

        private void UnsubscribeEvent()
        {
            GameEntry.Event.Unsubscribe(OnOpenningLogoEndEvent.EventId, OnOpenningLogoEndEventHandler);
        }


    }

    public class ProcedureChangeToStartScene : ProcedureBase
    {
        private bool m_IsLoadSceneComplete = false;
        private AsyncOperation m_LoadSceneAsyncOperation = null;
        private string m_SceneName = "GameStartMenuScene";

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Time.timeScale = 1f;

            m_LoadSceneAsyncOperation = SceneManager.LoadSceneAsync(m_SceneName);
            m_LoadSceneAsyncOperation.allowSceneActivation = false;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_LoadSceneAsyncOperation == null)
                return;

            float progress = Mathf.Clamp01(m_LoadSceneAsyncOperation.progress / 0.9f);

            GameFrameworkLog.Info($"加载进度: {(progress * 100f):F1}%");

            // TODO: 在这里更新 UI 进度条，比如：
            // loadingProgressBar.value = progress;

            if (progress >= 1f && !m_IsLoadSceneComplete)
            {
                m_IsLoadSceneComplete = true;
                m_LoadSceneAsyncOperation.allowSceneActivation = true;
                GameFrameworkLog.Info("场景加载完成，即将切换流程");
            }

            if (m_IsLoadSceneComplete && m_LoadSceneAsyncOperation.isDone)
            {
                ChangeState<ProcedureStartScene>(procedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            m_LoadSceneAsyncOperation = null;
            m_IsLoadSceneComplete = false;
        }
    }


    public class ProcedureStartScene : ProcedureBase
    {
        private ProcedureOwner mOwner;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mOwner = procedureOwner;

            GameEntry.UI.GetComponent<UIComponent>().OpenUIForm(EnumUIForm.GameStartForm);

            GameEntry.Event.Subscribe(OnStartNewGameButtonClickEvent.EventId, OnStartNewGameHandler);
            GameEntry.Event.Subscribe(OnContinueGameButtonClickEvent.EventId, OnContinueGameHandler);
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.UI.GetComponent<UIComponent>().CloseAllLoadedUIForms();

            GameEntry.Event.Unsubscribe(OnStartNewGameButtonClickEvent.EventId, OnStartNewGameHandler);
            GameEntry.Event.Unsubscribe(OnContinueGameButtonClickEvent.EventId, OnContinueGameHandler);
        }
        private void OnContinueGameHandler(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureLoadSavedGame>(mOwner);
        }

        private void OnStartNewGameHandler(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureCreateNewGame>(mOwner);
        }
    }

    /// <summary>
    /// 1. 加载存档信息
    /// </summary>
    public class ProcedureLoadSavedGame : ProcedureBase
    {
        //TODO(vanish): 加载存档信息

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            ChangeState<ProcedureInitGameMain>(procedureOwner);
        }
    }
    /// <summary>
    /// 1. 创建新存档
    /// </summary>
    public class ProcedureCreateNewGame : ProcedureBase
    {
        override protected void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            ChangeState<ProcedureInitGameMain>(procedureOwner);
        }
    }
    /// <summary>
    /// 1. 根据存档信息初始化游戏数据
    /// </summary>
    public class ProcedureInitGameMain : ProcedureBase
    {
        private AsyncOperation metroplisSceneLoadAsyncOperation = null;
        private AsyncOperation dungeonSceneLoadAsyncOperation = null;

        private UnityEngine.SceneManagement.Scene metroplisScene;
        private UnityEngine.SceneManagement.Scene dungeonScene;

        private bool metroplisLoaded = false;
        private bool dungeonLoaded = false;

        private string metroplis = "MetroplisGameScene";
        private string dungeon = "DungeonGameScene";

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            LoadScenes();
        }

        private void LoadScenes()
        {
            metroplisSceneLoadAsyncOperation = SceneManager.LoadSceneAsync(metroplis, LoadSceneMode.Additive);
            dungeonSceneLoadAsyncOperation = SceneManager.LoadSceneAsync(dungeon, LoadSceneMode.Additive);

            metroplisSceneLoadAsyncOperation.allowSceneActivation = false;
            dungeonSceneLoadAsyncOperation.allowSceneActivation = false;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (metroplisSceneLoadAsyncOperation == null || dungeonSceneLoadAsyncOperation == null)
                return;

            float progress = Mathf.Clamp01((metroplisSceneLoadAsyncOperation.progress + dungeonSceneLoadAsyncOperation.progress) / 1.8f);
            GameFrameworkLog.Info($"加载进度: {(progress * 100f):F1}%");

            if (progress >= 1f)
            {
                metroplisSceneLoadAsyncOperation.allowSceneActivation = true;
                dungeonSceneLoadAsyncOperation.allowSceneActivation = true;
            }

            if (!metroplisLoaded && metroplisSceneLoadAsyncOperation.isDone)
            {
                metroplisScene = SceneManager.GetSceneByName(metroplis);
                SetSceneActiveState(metroplisScene, false);
                metroplisLoaded = true;
                GameFrameworkLog.Info("Metropolis 场景加载完毕并已禁用。");
            }

            if (!dungeonLoaded && dungeonSceneLoadAsyncOperation.isDone)
            {
                dungeonScene = SceneManager.GetSceneByName(dungeon);
                SetSceneActiveState(dungeonScene, false);
                dungeonLoaded = true;
                GameFrameworkLog.Info("Dungeon 场景加载完毕并已禁用。");
            }

            if (metroplisLoaded && dungeonLoaded)
            {
                GameFrameworkLog.Info("所有场景已加载完毕并禁用，可根据需要启用场景。");

                //TODO: Init Scene by saved data

                ChangeState<ProcedureMetroplisStage>(procedureOwner); // 可根据实际流程改为等待手动切换
            }
        }

        private void SetSceneActiveState(UnityEngine.SceneManagement.Scene scene, bool active)
        {
            if (!scene.IsValid())
            {
                Debug.LogWarning($"Scene 无效: {scene.name}");
                return;
            }

            foreach (GameObject go in scene.GetRootGameObjects())
            {
                go.SetActive(active);
            }

            GameFrameworkLog.Info($"{scene.name} 场景 {(active ? "启用" : "禁用")}");
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            metroplisSceneLoadAsyncOperation = null;
            dungeonSceneLoadAsyncOperation = null;
            metroplisLoaded = false;
            dungeonLoaded = false;
        }
    }
    /// <summary>
    /// 进入模拟经营部分
    /// </summary>
    public class ProcedureMetroplisStage : ProcedureBase
    {
        /// <summary>
        /// 1. 暂停地牢场景
        /// 2. 启用模拟经营场景
        /// 3. 关闭地牢系统
        /// 4. 开启模拟经营系统
        /// </summary>
        /// <param name="procedureOwner"></param>
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mOwner = procedureOwner;

            GameEntry.Event.Fire(this, OnSwitchedToMetroplisProcedureEvent.Create());

            SubscribeEvents();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnPlayerSwitchToDungeonEvent.EventId, OnPlayerSwitchToDungeonHandler);
            GameEntry.Event.Subscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonHandler);
        }

        private void OnHeroArrivedInDungeonHandler(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureDungeonPreHeroArrivedStage>(mOwner);
        }

        private void OnPlayerSwitchToDungeonHandler(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureDungeonPlacingStage>(mOwner);
        }

        private void UnsubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnPlayerSwitchToDungeonEvent.EventId, OnPlayerSwitchToDungeonHandler);
            GameEntry.Event.Unsubscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonHandler);
        }

        private ProcedureOwner mOwner;

    }
    /// <summary>
    /// 进入地牢建造部分
    /// </summary>
    public class ProcedureDungeonPlacingStage : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mOwner = procedureOwner;

            GameEntry.Event.Fire(this, OnSwitchedToDungeonPlacingProcedureEvent.Create());

            SubscribeEvents();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
            GameEntry.Event.Subscribe(OnPlayerSwitchToMetroplisEvent.EventId, OnPlayerSwitchToMetroplisEventHandler);
            GameEntry.Event.Subscribe(OnPlayerJumpToHeroArrivedEvent.EventId, OnPlayerJumpToHeroArrivedEventHandler);
        }

        private void OnPlayerJumpToHeroArrivedEventHandler(object sender, GameEventArgs e)
        {

        }

        private void OnPlayerSwitchToMetroplisEventHandler(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureMetroplisStage>(mOwner);
        }

        private void OnHeroArrivedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureDungeonPreHeroArrivedStage>(mOwner);
        }

        private void UnsubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEventHandler);
            GameEntry.Event.Unsubscribe(OnPlayerSwitchToMetroplisEvent.EventId, OnPlayerSwitchToMetroplisEventHandler);
            GameEntry.Event.Unsubscribe(OnPlayerJumpToHeroArrivedEvent.EventId, OnPlayerJumpToHeroArrivedEventHandler);
        }

        private ProcedureOwner mOwner;
    }
    /// <summary>
    /// 当勇者到达地牢时
    /// </summary>
    public class ProcedureDungeonPreHeroArrivedStage : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mOwner = procedureOwner;

            GameEntry.Event.Fire(this, OnSwitchedToDungeonPreHeroArrivedProcedureEvent.Create());

            SubscribeEvents();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);
        }

        private void OnHeroStartExploreDungeonEventHandler(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureHeroExploringDungeonStage>(mOwner);
        }

        private void UnsubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);
        }

        private ProcedureOwner mOwner;
    }
    /// <summary>
    /// 当勇者开始攻略地牢时
    /// </summary>
    public class ProcedureHeroExploringDungeonStage : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mOwner = procedureOwner;

            GameEntry.Event.Fire(this, OnSwitchedToHeroExploringDungeonProcedureEvent.Create());

            SubscribeEvents();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnHeroFinishDungeonExploreEvent.EventId, OnHeroFinishDungeonExploreEventHandler);
            GameEntry.Event.Subscribe(OnHeroTeamDiedInDungeonEvent.EventId, OnHeroTeamDiedInDungeonEventHandler);
        }

        private void OnHeroTeamDiedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureDungeonCalculationStage>(mOwner);
        }

        private void OnHeroFinishDungeonExploreEventHandler(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureDungeonCalculationStage>(mOwner);
        }

        private void UnsubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnHeroFinishDungeonExploreEvent.EventId, OnHeroFinishDungeonExploreEventHandler);
            GameEntry.Event.Unsubscribe(OnHeroTeamDiedInDungeonEvent.EventId, OnHeroTeamDiedInDungeonEventHandler);
        }

        private ProcedureOwner mOwner;
    }
    /// <summary>
    /// 当需要播放对话时
    /// 1. 暂停游戏
    /// 2. 开始对话
    /// 3. 对话完毕结算后恢复游戏
    /// </summary>
    public class ProcedureDialogueStage : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
    /// <summary>
    /// 地牢攻略结束,进行结算
    /// </summary>
    public class ProcedureDungeonCalculationStage : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mOwner = procedureOwner;

            GameEntry.Event.Fire(this, OnSwitchedToDungeonCalculationProcedureEvent.Create());

            SubscribeEvents();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnDungeonCalculationFinishedEvent.EventId, OnDungeonCalculationFinishedEventHandler);
        }

        private void OnDungeonCalculationFinishedEventHandler(object sender, GameEventArgs e)
        {

        }

        private void UnsubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnDungeonCalculationFinishedEvent.EventId, OnDungeonCalculationFinishedEventHandler);
        }

        private ProcedureOwner mOwner;
    }
}




#region Events
namespace Dungeon.Evnents
{
    /// <summary>
    /// 当玩家按下切换到模拟经营按钮时触发 //TODO(xy)
    /// </summary>
    public sealed class OnPlayerSwitchToMetroplisEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnPlayerSwitchToMetroplisEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnPlayerSwitchToMetroplisEvent Create()
        {
            OnPlayerSwitchToMetroplisEvent onStartFightButtonClickEventArgs = ReferencePool.Acquire<OnPlayerSwitchToMetroplisEvent>();
            return onStartFightButtonClickEventArgs;
        }

        public override void Clear()
        {
        }
    }

    /// <summary>
    /// 当玩家按下切换到地牢按钮时触发 //TODO(xy)
    /// </summary>
    public sealed class OnPlayerSwitchToDungeonEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnPlayerSwitchToDungeonEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnPlayerSwitchToDungeonEvent Create()
        {
            OnPlayerSwitchToDungeonEvent onSwitchToDungeonEventArgs = ReferencePool.Acquire<OnPlayerSwitchToDungeonEvent>();
            return onSwitchToDungeonEventArgs;
        }

        public override void Clear()
        {
        }
    }

    /// <summary>
    /// 当勇者到达地牢时触发 //TODO(xy)
    /// 1. 时间到了
    /// 2. 玩家按下快进到勇者到达按钮,勇者到达地牢,然后触发
    /// </summary>
    public sealed class OnHeroArrivedInDungeonEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnHeroArrivedInDungeonEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnHeroArrivedInDungeonEvent Create()
        {
            OnHeroArrivedInDungeonEvent onHeroArrivedInDungeonEventArgs = ReferencePool.Acquire<OnHeroArrivedInDungeonEvent>();
            return onHeroArrivedInDungeonEventArgs;
        }

        public override void Clear()
        {
        }
    }

    /// <summary>
    /// 当切换到ProcedureMetroplisStage时触发
    /// </summary>
    public sealed class OnSwitchedToMetroplisProcedureEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnSwitchedToMetroplisProcedureEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnSwitchedToMetroplisProcedureEvent Create()
        {
            OnSwitchedToMetroplisProcedureEvent onSwitchedToMetroplisProcedureEventArgs = ReferencePool.Acquire<OnSwitchedToMetroplisProcedureEvent>();
            return onSwitchedToMetroplisProcedureEventArgs;
        }

        public override void Clear()
        {
        }
    }

    /// <summary>
    /// 当切换到ProcedureDungeonPlacingStage时触发
    /// </summary>
    public sealed class OnSwitchedToDungeonPlacingProcedureEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnSwitchedToDungeonPlacingProcedureEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnSwitchedToDungeonPlacingProcedureEvent Create()
        {
            OnSwitchedToDungeonPlacingProcedureEvent onSwitchedToDungeonPlacingProcedureEventArgs = ReferencePool.Acquire<OnSwitchedToDungeonPlacingProcedureEvent>();
            return onSwitchedToDungeonPlacingProcedureEventArgs;
        }

        public override void Clear()
        {
        }
    }

    /// <summary>
    /// 当玩家按下快进到勇者到达按钮时触发 //TODO(xy)
    /// </summary>
    public sealed class OnPlayerJumpToHeroArrivedEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnPlayerJumpToHeroArrivedEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnPlayerJumpToHeroArrivedEvent Create()
        {
            OnPlayerJumpToHeroArrivedEvent onPlayerJumpToHeroArrivedEventArgs = ReferencePool.Acquire<OnPlayerJumpToHeroArrivedEvent>();
            return onPlayerJumpToHeroArrivedEventArgs;
        }

        public override void Clear()
        {
        }
    }

    /// <summary>
    /// 当切换到ProcedureDungeonPreHeroArrivedStage时触发
    /// </summary>
    public sealed class OnSwitchedToDungeonPreHeroArrivedProcedureEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnSwitchedToDungeonPreHeroArrivedProcedureEvent).GetHashCode();

        public override int Id
        {
            get
            {                
                return EventId;
            }
        }
        public static OnSwitchedToDungeonPreHeroArrivedProcedureEvent Create()
        {
            OnSwitchedToDungeonPreHeroArrivedProcedureEvent onSwitchedToDungeonPreHeroArrivedProcedureEventArgs = ReferencePool.Acquire<OnSwitchedToDungeonPreHeroArrivedProcedureEvent>();
            return onSwitchedToDungeonPreHeroArrivedProcedureEventArgs;
        }

        public override void Clear()
        {   
        }
    }

    /// <summary>
    /// 当勇者开始攻略地牢时触发 //TODO(xy)
    /// </summary>
    public sealed class OnHeroStartExploreDungeonEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnHeroStartExploreDungeonEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnHeroStartExploreDungeonEvent Create()
        {
            OnHeroStartExploreDungeonEvent onHeroStartExploreDungeonEventArgs = ReferencePool.Acquire<OnHeroStartExploreDungeonEvent>();
            return onHeroStartExploreDungeonEventArgs;
        }

        public override void Clear()
        {
        }
    }

    public sealed class OnSwitchedToHeroExploringDungeonProcedureEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnSwitchedToHeroExploringDungeonProcedureEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnSwitchedToHeroExploringDungeonProcedureEvent Create()
        {
            OnSwitchedToHeroExploringDungeonProcedureEvent a = ReferencePool.Acquire<OnSwitchedToHeroExploringDungeonProcedureEvent>();
            return a;
        }

        public override void Clear()
        {
        }
    }
    
    public sealed class OnHeroFinishDungeonExploreEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnHeroFinishDungeonExploreEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnHeroFinishDungeonExploreEvent Create()
        {
            OnHeroFinishDungeonExploreEvent a = ReferencePool.Acquire<OnHeroFinishDungeonExploreEvent>();
            return a;
        }

        public override void Clear()
        {
        }
    }

    public sealed class OnHeroTeamDiedInDungeonEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnHeroTeamDiedInDungeonEvent).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnHeroTeamDiedInDungeonEvent Create()
        {
            OnHeroTeamDiedInDungeonEvent a = ReferencePool.Acquire<OnHeroTeamDiedInDungeonEvent>();
            return a;
        }

        public override void Clear()
        {}
    }

    public sealed class OnSwitchedToDungeonCalculationProcedureEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnSwitchedToDungeonCalculationProcedureEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnSwitchedToDungeonCalculationProcedureEvent Create()
        {
            OnSwitchedToDungeonCalculationProcedureEvent a = ReferencePool.Acquire<OnSwitchedToDungeonCalculationProcedureEvent>();
            return a;
        }

        public override void Clear()
        {
        }
    }

    public sealed class OnDungeonCalculationFinishedEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnDungeonCalculationFinishedEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnDungeonCalculationFinishedEvent Create()
        {
            OnDungeonCalculationFinishedEvent a = ReferencePool.Acquire<OnDungeonCalculationFinishedEvent>();
            return a;
        }

        public override void Clear()
        {
        }
    }
    /// <summary>
    /// 当结束放映logo时触发
    /// </summary>
    public sealed class OnOpenningLogoEndEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnOpenningLogoEndEvent).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public static OnOpenningLogoEndEvent Create()
        {
            OnOpenningLogoEndEvent OnOpenningLogoEndEvent = ReferencePool.Acquire<OnOpenningLogoEndEvent>();
            return OnOpenningLogoEndEvent;
        }
        
        public override void Clear()
        {
        }
    }
}

#endregion