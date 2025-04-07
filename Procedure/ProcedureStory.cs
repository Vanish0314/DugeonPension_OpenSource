using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;


namespace Dungeon
{
    public class ProcedureStory : ProcedureBase
    {
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Debug.Log("Enter Procedure Story");
            //过场动画/UI/剧情
            base.OnEnter(procedureOwner);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            if (Input.GetKeyDown(KeyCode.A))//一些条件
                ChangeState<ProcedureFight>(procedureOwner);
            if (Input.GetKeyDown(KeyCode.Space))//另一些条件
                ChangeState<ProcedureChangeScene>(procedureOwner);
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
