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
        BusinessControl businessControl;
        
        private bool placeArmyEnd = false;
        
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Debug.Log("ProcedurePlaceArmy OnEnter");
            
            placeArmyEnd = false;
            
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnFightSceneLoadEventArgs.Create());

            GameEntry.Event.GetComponent<EventComponent>().Subscribe(OnPlaceArmyEndEventArgs.EventId, PlaceArmyEnd);
            
            BuildManager buildManager = UnityEngine.GameObject.Find("Manager").GetComponent<BuildManager>();

            businessControl = BusinessControl.Create(buildManager);
            
            GameEntry.UI.OpenUIForm(EnumUIForm.ResourceFrom);
            GameEntry.UI.OpenUIForm(EnumUIForm.PlaceArmyForm);

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
            BuildManager buildManager = UnityEngine.GameObject.Find("Manager").GetComponent<BuildManager>();
            
            // 关闭所有ui
            GameEntry.UI.CloseAllLoadedUIForms();
            
            // 禁用GridSystem（暂时）
            buildManager.DisableGridSystemTemp();
                
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
