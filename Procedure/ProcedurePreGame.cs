using System.Collections;
using System.Collections.Generic;
using Dungeon.Data;
using GameFramework;
using GameFramework.Event;
using GameFramework.Data;
using UnityGameFramework.Runtime;
using GameFramework.Procedure;
using GameFramework.DataTable;
using GameFramework.Resource;
using UnityEngine;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using LoadDataTableSuccessEventArgs = UnityGameFramework.Runtime.LoadDataTableSuccessEventArgs;
using LoadDataTableFailureEventArgs = UnityGameFramework.Runtime.LoadDataTableFailureEventArgs;
using System;

namespace Dungeon
{
    [Obsolete]
    public class ProcedurePreLoad : ProcedureBase
    {
        private bool initResourceComplete = false;
        
        private DataBase[] datas;

        private Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            if (GameEntry.Resource.ResourceMode == ResourceMode.Package)
            {
                initResourceComplete = false;
            
                GameEntry.Resource.InitResources(OnInitResourceComplete);
            }
            
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
            
            if (datas == null)
                return;

            SetComponents();
            procedureOwner.SetData<VarInt32>(Constant.ProcedureData.NextSceneId, GameEntry.Config.GetInt("Scene.GameStart"));
            
            //if (initResourceComplete)
            ChangeState<ProcedureChangeScene>(procedureOwner);
        }


        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }

        private void PreloadResources()
        {
            // Preload configs
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
        
        private void OnInitResourceComplete()
        {
            initResourceComplete = true;
            Debug.Log("Init resources complete.");
        }
    }
}



