using Dungeon.GridSystem;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;


namespace Dungeon
{
    public class ProcedureBusiness : ProcedureBase
    {
        private ProcedureOwner procedureOwner;
        
        BusinessControl m_BusinessControl;

        private bool m_IsBusiness;
            
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Debug.Log("ProcedureBusiness OnEnter");

            m_IsBusiness = true;
            
            // 暂时---------------------------------------------------
            BuildGridSystem.Instance.gameObject.SetActive(true);
            
            // 发送流程开始事件
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnBusinessStartEventArgs.Create());
            
            // 初始化BusinessControl
            m_BusinessControl = BusinessControl.Create(PlaceManager.Instance);
            
            // 初始化UI
            GameEntry.UI.OpenUIForm(EnumUIForm.ResourceFrom);
            GameEntry.UI.OpenUIForm(EnumUIForm.TimelineForm);
            // 暂时
            GameEntry.UI.OpenUIForm(EnumUIForm.HeroMenuForm);
            
            // 订阅TimeManager
            TimeManager.Instance.OnFiveMinutesElapsed += EndBusiness;
            
            this.procedureOwner = procedureOwner;
            
            m_BusinessControl.OnEnter();
            
            base.OnEnter(procedureOwner);
        }
        
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            if(!m_IsBusiness)
                ChangeState<ProcedureBusinessSettlement>(procedureOwner);
            
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_BusinessControl != null)
            {
                m_BusinessControl.Update(elapseSeconds, realElapseSeconds);
            }
        }
    
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            //----------------------------------------------------
            BuildGridSystem.Instance.gameObject.SetActive(false);
            
            GameEntry.UI.CloseAllLoadedUIForms();
            
            // 发送流程结束事件
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnBusinessEndEventArgs.Create());
            
            // 取消订阅TimeManager
            TimeManager.Instance.OnFiveMinutesElapsed -= EndBusiness;
            
            m_BusinessControl.OnLeave();
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }
        
        private void EndBusiness()
        {
            m_IsBusiness = false;
        }

    }
}
