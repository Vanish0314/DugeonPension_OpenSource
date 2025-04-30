using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dungeon.Data;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Dungeon.Procedure
{
    public class ProcedureBattleField : DungoenProcedure.DungeonProcedure
    {
        private DataBase[] datas;

        private Dictionary<string, bool> m_LoadedFlag = new();

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
            ChangeState<ProcedureBattelFilding>(procedureOwner);
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

    public class ProcedureBattelFilding : DungoenProcedure.DungeonProcedure
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Task.Run(async () =>
            {
                await Task.Delay(1000);
                GameEntry.Event.Fire(OnPlayerSwitchToDungeonEvent.EventId, OnPlayerSwitchToDungeonEvent.Create());
                await Task.Delay(1000);
                GameEntry.Event.Fire(OnHeroArrivedInDungeonEvent.EventId, OnHeroArrivedInDungeonEvent.Create(
                    DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentMainHero()
                ));
            });
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
}
