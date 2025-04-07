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

            GameEntry.UI.OpenUIForm(EnumUIForm.ResourceFrom);
            
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
                ChangeState<ProcedureEnd>(procedureOwner);
            
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }
    
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }
    }
}
