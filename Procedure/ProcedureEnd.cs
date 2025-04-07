using System;
using Dungeon.DungeonGameEntry;
using GameFramework.Event;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;


namespace Dungeon
{
    public class ProcedureEnd : ProcedureBase
    {
        private VarInt32 targetId = 2;
        private ProcedureOwner mOwner;
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);

            mOwner = procedureOwner;
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnDungeonEndEventArgs.EventId,OnGameEnd);
        }

        private void OnGameEnd(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureChangeScene>(mOwner); 
        }


        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Debug.Log("ProcedureEnd OnEnter");
            
            targetId = 2;//初始化场景id
            //结算UI
            base.OnEnter(procedureOwner);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            if(Input.GetKeyDown(KeyCode.Space))//一些条件
                ChangeState<ProcedureStory>(procedureOwner);

            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }
    
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            procedureOwner.SetData(Constant.ProcedureData.NextSceneId, targetId);
            
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnDungeonEndEventArgs.EventId,OnGameEnd);
            
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }
    }
}
