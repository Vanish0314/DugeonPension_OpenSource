using GameFramework.Event;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using System;
using Dungeon.Data;

namespace Dungeon
{
    public class ProcedureChangeScene : ProcedureBase
    {
        private bool loadSceneCompleted = false;
        //初始化SceneData
        private SceneData sceneData = null;

        private int loadingSceneId = -1;

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }
    
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Debug.Log("ProcedureChangeScene: Enter");
            base.OnEnter(procedureOwner);
        
            //初始化算是
            loadSceneCompleted = false;
            loadingSceneId = -1;
        
            //订阅事件
            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            GameEntry.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
            GameEntry.Event.Subscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);

            //卸载所有场景
            string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            for (int i = 0; i < loadedSceneAssetNames.Length; i++)
            {
                GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i]);
            }
            
            //关闭所有UI
            GameEntry.UI.CloseAllLoadedUIForms();
            GameEntry.UI.CloseAllLoadingUIForms();

            //加载场景相关
            loadingSceneId = procedureOwner.GetData<VarInt32>(Constant.ProcedureData.NextSceneId).Value;//获取目标场景ID
            sceneData = GameEntry.Data.GetData<DataScene>().GetSceneData(loadingSceneId);//获取场景数据
            
            //日志
             if (sceneData == null)
             {
                 Debug.Log("ProcedureChangeScene: Scene not found");
                 Log.Warning("Can not can scene data id :'{0}'.", loadingSceneId.ToString());
                 return;
             }

            //根据数据加载场景
            GameEntry.Scene.LoadScene(sceneData.AssetPath, Constant.AssetPriority.SceneAsset, this);
            GameEntry.Event.GetComponent<EventComponent>().Fire(this,OnSceneLoadedEventArgs.Create(sceneData.Id));
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            
            if (loadSceneCompleted)
            {
                //切换到目标流程
                Type procedureType = Type.GetType(string.Format("Dungeon.{0}", sceneData.Procedure));//根据命名空间和sceneData来获取流程类型
                if (null != procedureType)
                {
               
                    ChangeState(procedureOwner, procedureType);
                }
                else
                    Log.Warning("Can not change state,scene procedure '{0}' error, from scene '{1}.{2}'.", sceneData.Procedure.ToString(), sceneData.Id, sceneData.AssetPath);
            }
        }
    
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            
            //初始化
            loadingSceneId = -1;

            //取消订阅
            GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            GameEntry.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
            GameEntry.Event.Unsubscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }

        private void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            loadSceneCompleted = true;
            
            Log.Info("Load scene '{0}' OK.", ne.SceneAssetName);
        }

        private void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Load scene '{0}' failure, error message '{1}'.", ne.SceneAssetName, ne.ErrorMessage);
        }

        private void OnLoadSceneUpdate(object sender, GameEventArgs e)
        {
            LoadSceneUpdateEventArgs ne = (LoadSceneUpdateEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Info("Load scene '{0}' update, progress '{1}'.", ne.SceneAssetName, ne.Progress.ToString("P2"));
        }

        private void OnLoadSceneDependencyAsset(object sender, GameEventArgs e)
        {
            LoadSceneDependencyAssetEventArgs ne = (LoadSceneDependencyAssetEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Info("Load scene '{0}' dependency asset '{1}', count '{2}/{3}'.", ne.SceneAssetName, ne.DependencyAssetName, ne.LoadedCount.ToString(), ne.TotalCount.ToString());
        }
    }
}
