using System;
using System.Collections.Generic;
using Codice.Client.BaseCommands.Merge.IncomingChanges;
using Dungeon.Data;
using Dungeon.DungeonEntity;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using DungoenProcedure;
using GameFramework;
using GameFramework.Event;
using GameFramework.Procedure;
using GameFramework.Resource;
using GluonGui.Dialog;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace DungoenProcedure
{
    public abstract class DungeonProcedure : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            var className = GetType().Name;
            GameFrameworkLog.Info($"[{className}] 进入流程: 「{className}」");
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            var className = GetType().Name;
            GameFrameworkLog.Info($"[{className}] 离开流程: 「{className}」");
        }
    }
}

namespace Dungeon.Procedure
{
    /// <summary>
    /// ProcedureLauch
    /// 1. 获取存档信息与用户设置
    /// 2. 初始化部分系统
    /// 3. 一帧后切换到ProcedureOpenning
    /// 4. 配置全局初始静态数据
    /// </summary>
    public class ProcedureLauch : DungeonProcedure
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            // 声音配置：根据用户配置数据，设置即将使用的声音选项
            InitSoundSettings();
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            ChangeState<ProcedurePreload>(procedureOwner);
        }

        private void OnInitResourcesComplete()
        {
            GameFrameworkLog.Info("初始化资源完成");
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

    public class ProcedurePreload : DungeonProcedure
    {
        private DataBase[] datas;

        private Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            GameEntry.Event.Subscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            GameEntry.Event.Subscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            GameEntry.Event.Subscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);

            GameFramework.Data.Data[] _datas = GameEntry.Data.GetAllData();

            datas = new DataBase[_datas.Length];
            for (int i = 0; i < _datas.Length; i++)
            {
                if (_datas[i] is DataBase)
                {
                    datas[i] = _datas[i] as DataBase;
                }
                else
                {
                    throw new System.Exception(string.Format("Data {0} is not derive form DataBase", _datas[i].GetType()));
                }
            }

            PreloadResources();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            foreach (var item in m_LoadedFlag)
            {
                if (!item.Value)
                    return;
            }

            if (datas == null)
                return;

            foreach (var item in datas)
            {
                if (!item.IsPreloadReady)
                    return;
            }

            SetComponents();
            ChangeState<ProcedureOpenning>(procedureOwner);
        }


        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            GameEntry.Event.Unsubscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            GameEntry.Event.Unsubscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            GameEntry.Event.Unsubscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }

        private void PreloadResources()
        {
            LoadConfig("DefaultConfig");

            GameEntry.Data.PreLoadAllData();
        }

        private void SetComponents()
        {
            SetDataComponent();
            SetUIComponent();
        }

        private void SetDataComponent()
        {
            // GameEntry.Data.PreLoadAllData();
            GameEntry.Data.LoadAllData();
        }

        private void SetUIComponent()
        {
            UIGroupData[] uiGroupDatas = GameEntry.Data.GetData<DataUI>().GetAllUIGroupData();
            foreach (var item in uiGroupDatas)
            {
                GameEntry.UI.AddUIGroup(item.Name, item.Depth);
            }
        }
        private void LoadConfig(string configName)
        {
            string configAssetName = AssetUtility.GetConfigAsset(configName, false);
            m_LoadedFlag.Add(configAssetName, false);
            GameEntry.Config.ReadData(configAssetName, this);
        }
        private void OnLoadConfigSuccess(object sender, GameEventArgs e)
        {
            LoadConfigSuccessEventArgs ne = (LoadConfigSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LoadedFlag[ne.ConfigAssetName] = true;
            Log.Info("Load config '{0}' OK.", ne.ConfigAssetName);
        }

        private void OnLoadConfigFailure(object sender, GameEventArgs e)
        {
            LoadConfigFailureEventArgs ne = (LoadConfigFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load config '{0}' from '{1}' with error message '{2}'.", ne.ConfigAssetName, ne.ConfigAssetName, ne.ErrorMessage);
        }

        private void OnLoadDictionarySuccess(object sender, GameEventArgs e)
        {
            LoadDictionarySuccessEventArgs ne = (LoadDictionarySuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LoadedFlag[ne.DictionaryAssetName] = true;
            Log.Info("Load dictionary '{0}' OK.", ne.DictionaryAssetName);
        }

        private void OnLoadDictionaryFailure(object sender, GameEventArgs e)
        {
            LoadDictionaryFailureEventArgs ne = (LoadDictionaryFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load dictionary '{0}' from '{1}' with error message '{2}'.", ne.DictionaryAssetName, ne.DictionaryAssetName, ne.ErrorMessage);
        }

    }



    /// <summary>
    /// ProcedureOpenning
    /// 1. 播放开场Logo
    /// 2. 播放完毕切换到ProcedureStartScene
    /// 3. 播放动画时可以进行一些加载工作
    /// </summary>
    public class ProcedureOpenning : DungeonProcedure
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

    public class ProcedureChangeToStartScene : DungeonProcedure
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


    public class ProcedureStartScene : DungeonProcedure
    {
        private ProcedureOwner mOwner;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mOwner = procedureOwner;

            GameEntry.UI.OpenUIForm(EnumUIForm.GameStartForm);

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
    public class ProcedureLoadSavedGame : DungeonProcedure
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
    public class ProcedureCreateNewGame : DungeonProcedure
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
    public class ProcedureInitGameMain : DungeonProcedure
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
                ChangeState<ProcedureMetropolisStage>(procedureOwner); // 可根据实际流程改为等待手动切换
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
            GameEntry.Event.FireNow(this, OnProcedureInitGameMainLeaveEvent.Create());
            SceneManager.UnloadSceneAsync("GameStartMenuScene");

            metroplisSceneLoadAsyncOperation = null;
            dungeonSceneLoadAsyncOperation = null;
            metroplisLoaded = false;
            dungeonLoaded = false;

            base.OnLeave(procedureOwner, isShutdown);
        }
    }
    /// <summary>
    /// 进入模拟经营部分
    /// </summary>
    public class ProcedureMetropolisStage : DungeonProcedure
    {
        /// <summary>
        /// 1. 暂停地牢场景
        /// 2. 启用模拟经营场景
        /// 3. 关闭地牢系统
        /// 4. 开启模拟经营系统
        /// </summary>
        /// <param name="procedureOwner"></param>
        private BusinessControl m_MetropolisControl;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mOwner = procedureOwner;

            GameEntry.Event.Fire(this, OnSwitchedToMetroplisProcedureEvent.Create());

            DungeonGameEntry.DungeonGameEntry.Instance.DisableDungeon();
            DungeonGameEntry.DungeonGameEntry.Instance.EnableMetroplis();

            m_MetropolisControl = BusinessControl.Create(PlaceManager.Instance);
            m_MetropolisControl.OnEnter();
            
            GameEntry.UI.OpenUIForm(EnumUIForm.ResourceFrom);
            GameEntry.UI.OpenUIForm(EnumUIForm.TimelineForm);
            GameEntry.UI.OpenUIForm(EnumUIForm.StartFightButtonForm);

            SubscribeEvents();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            m_MetropolisControl.OnLeave();
            
            GameEntry.UI.CloseAllLoadedUIForms();

            UnsubscribeEvents();

            base.OnLeave(procedureOwner, isShutdown);
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
    public class ProcedureDungeonPlacingStage : DungeonProcedure
    {
        private PlaceArmyControl m_PlaceArmyControl;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mOwner = procedureOwner;
            
            m_PlaceArmyControl = PlaceArmyControl.Create(PlaceManager.Instance);
            m_PlaceArmyControl.OnEnter();

            GameEntry.Event.Fire(this, OnSwitchedToDungeonPlacingProcedureEvent.Create());

            DungeonGameEntry.DungeonGameEntry.Instance.DisableMetroplis();
            DungeonGameEntry.DungeonGameEntry.Instance.EnableDungeon();

            SubscribeEvents();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            m_PlaceArmyControl.OnLeave();
            
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
            ChangeState<ProcedureMetropolisStage>(mOwner);
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
    public class ProcedureDungeonPreHeroArrivedStage : DungeonProcedure
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
    public class ProcedureHeroExploringDungeonStage : DungeonProcedure
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
            GameEntry.Event.Subscribe(OnHeroTeamFinishDungeonExploreEvent.EventId, OnHeroFinishDungeonExploreEventHandler);
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
            GameEntry.Event.Unsubscribe(OnHeroTeamFinishDungeonExploreEvent.EventId, OnHeroFinishDungeonExploreEventHandler);
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
    public class ProcedureDialogueStage : DungeonProcedure
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
    public class ProcedureDungeonCalculationStage : DungeonProcedure
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mOwner = procedureOwner;

            GameEntry.Event.Fire(this, OnSwitchedToDungeonCalculationProcedureEvent.Create());

            SubscribeEvents();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            ChangeState<ProcedureMetropolisStage>(mOwner);
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
    /// 当玩家按下按钮时触发//TODO(xy)
    /// </summary>
    public sealed class OnStartNewGameButtonClickEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnStartNewGameButtonClickEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnStartNewGameButtonClickEvent Create()
        {
            OnStartNewGameButtonClickEvent a = ReferencePool.Acquire<OnStartNewGameButtonClickEvent>();
            return a;
        }

        public override void Clear()
        {
        }

    }
    /// <summary>
    /// 当玩家按下继续游戏按钮时触发//TODO(xy)
    /// </summary>
    public sealed class OnContinueGameButtonClickEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnContinueGameButtonClickEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnContinueGameButtonClickEvent Create()
        {
            OnContinueGameButtonClickEvent a = ReferencePool.Acquire<OnContinueGameButtonClickEvent>();
            return a;
        }

        public override void Clear()
        {
        }
    }
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

    /// <summary>
    /// 当有一个勇者到达了地牢终点房间时触发
    /// </summary>
    public sealed class OnOneHeroArrivedAtDungeonEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnOneHeroArrivedAtDungeonEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnOneHeroArrivedAtDungeonEvent Create()
        {
            OnOneHeroArrivedAtDungeonEvent a = ReferencePool.Acquire<OnOneHeroArrivedAtDungeonEvent>();
            return a;
        }

        public override void Clear()
        {
        }
    }

    /// <summary>
    /// 当整个勇者小队完成了探险,终点房安全,该结算时触发
    /// </summary>
    public sealed class OnHeroTeamFinishDungeonExploreEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnHeroTeamFinishDungeonExploreEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnHeroTeamFinishDungeonExploreEvent Create()
        {
            OnHeroTeamFinishDungeonExploreEvent a = ReferencePool.Acquire<OnHeroTeamFinishDungeonExploreEvent>();
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
        { }
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

    /// <summary>
    /// 当ProcedureInitGameMain结束时触发
    /// </summary>
    public sealed class OnProcedureInitGameMainLeaveEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnProcedureInitGameMainLeaveEvent).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public static OnProcedureInitGameMainLeaveEvent Create()
        {
            OnProcedureInitGameMainLeaveEvent a = ReferencePool.Acquire<OnProcedureInitGameMainLeaveEvent>();
            return a;
        }

        public override void Clear()
        {
        }
    }
}

#endregion