using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;


namespace Dungeon
{
    public class ProcedureBusiness : ProcedureBase
    {
        private ProcedureOwner procedureOwner;
        
        VarInt32 targetId = 3;
        
        BusinessControl businessControl;
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Debug.Log("ProcedureBusiness OnEnter");
            targetId = 3;//初始化场景id

            base.OnEnter(procedureOwner);
            
            BuildManager buildManager = UnityEngine.GameObject.Find("Manager").GetComponent<BuildManager>();

            businessControl = BusinessControl.Create(buildManager);
            
            GameEntry.UI.OpenUIForm(EnumUIForm.ResourceFrom);
            GameEntry.UI.OpenUIForm(EnumUIForm.TimelineForm);
            
            this.procedureOwner = procedureOwner;
            
            businessControl.OnEnter();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            if(!businessControl.m_IsBusiness)
                ChangeState<ProcedureChangeScene>(procedureOwner);
            
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (businessControl != null)
            {
                businessControl.Update(elapseSeconds, realElapseSeconds);
            }
        }
    
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            procedureOwner.SetData(Constant.ProcedureData.NextSceneId, targetId);
            
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }
    }
}
