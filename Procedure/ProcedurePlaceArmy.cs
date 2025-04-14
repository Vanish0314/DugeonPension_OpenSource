using GameFramework;
using GameFramework.Event;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;


namespace Dungeon
{
    public class ProcedurePlaceArmy : ProcedureBase
    {
        private PlaceArmyControl m_PlaceArmyControl;

        private bool placeArmyEnd;
        
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Debug.Log("ProcedurePlaceArmy OnEnter");
            
            placeArmyEnd = false;
            
            //GridSystem.GridSystem
            
            // 发送流程开始事件
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnFightSceneLoadEventArgs.Create());
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnPlaceArmyStartEventArgs.Create());
            
            // 订阅事件
            GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnStartFightButtonClickEventArgs.EventId, PlaceArmyEnd);

            m_PlaceArmyControl = PlaceArmyControl.Create(PlaceManager.Instance);
            
            GameEntry.UI.OpenUIForm(EnumUIForm.ResourceFrom);
            GameEntry.UI.OpenUIForm(EnumUIForm.HeroInfoForm);
            GameEntry.UI.OpenUIForm(EnumUIForm.StartFightButtonForm);
            
            m_PlaceArmyControl.OnEnter();

            base.OnEnter(procedureOwner);
        }
        
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            if (placeArmyEnd)//另一些条件
                ChangeState<ProcedureFight>(procedureOwner);
            
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }
    
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            // 发送流程结束事件
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnPlaceArmyEndEventArgs.Create());
            
            // 取消订阅
            GameEntry.Event.GetComponent<EventComponent>().Unsubscribe(OnStartFightButtonClickEventArgs.EventId, PlaceArmyEnd);
            
            // 关闭ui
            GameEntry.UI.GetUIForm(EnumUIForm.PlaceArmyForm)?.Close();
            GameEntry.UI.GetUIForm(EnumUIForm.HeroInfoForm)?.Close();
            GameEntry.UI.GetUIForm(EnumUIForm.StartFightButtonForm)?.Close();
            
            m_PlaceArmyControl.OnLeave();
            
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }

        private void PlaceArmyEnd(object sender, GameEventArgs e)
        {
            placeArmyEnd = true;
        }
    }
}
