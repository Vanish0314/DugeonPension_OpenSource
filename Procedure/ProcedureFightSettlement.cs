using System;
using Dungeon.DungeonGameEntry;
using GameFramework.Event;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;


namespace Dungeon
{
    public class ProcedureFightSettlement : ProcedureBase
    {
        private VarInt32 targetId = 2;

        private bool m_IsContinue = false;
        
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Debug.Log("ProcedureEnd OnEnter");
            
            targetId = 2;//初始化场景id
            m_IsContinue = false;

            //结算UI
            GameEntry.UI.OpenUIForm(EnumUIForm.FightSettlementForm);
            //订阅
            GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnFightSettlementContinueEventArgs.EventId, OnProcedureContinue);
           
            base.OnEnter(procedureOwner);
        }

        private void OnProcedureContinue(object sender, GameEventArgs e)
        {
            m_IsContinue = true;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            if(m_IsContinue)//一些条件
                ChangeState<ProcedureChangeScene>(procedureOwner);

            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }
    
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            procedureOwner.SetData(Constant.ProcedureData.NextSceneId, targetId);
            
            //取消订阅
            GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnFightSettlementContinueEventArgs.EventId, OnProcedureContinue);
            
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }
    }
}
