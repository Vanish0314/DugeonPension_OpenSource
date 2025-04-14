using Dungeon.DungeonGameEntry;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;


namespace Dungeon
{
    public class ProcedureFight : ProcedureBase
    {
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }
        
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Debug.Log("ProcedureFight OnEnter");

            // 发送流程开始事件
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnFightStartEventArgs.Create());
            
            base.OnEnter(procedureOwner);

            DungeonGameEntry.DungeonGameEntry.GridSystem.gameObject.SetActive(true);
            DungeonGameEntry.DungeonGameEntry.GOAP.gameObject.SetActive(true);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            Log.Debug("ProcedureFight OnUpdate");
            //战斗相关机制
            //**********
            if (Input.GetKeyDown(KeyCode.Space))//一些条件
                ChangeState<ProcedureFightSettlement>(procedureOwner);
            
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }
    
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            // 发送流程结束事件
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnFightEndEventArgs.Create());
            
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }
    }
}
