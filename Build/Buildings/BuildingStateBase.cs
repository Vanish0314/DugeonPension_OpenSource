using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using GameFramework.Fsm;
using UnityEngine;

namespace Dungeon
{
    public abstract class BuildingStateBase : FsmState<MetropolisBuildingBase>
    {
        // 状态进入时调用
        protected override void OnEnter(IFsm<MetropolisBuildingBase> fsm) { }
        
        // 状态更新时调用
        protected override void OnUpdate(IFsm<MetropolisBuildingBase> fsm, float elapseSeconds, float realElapseSeconds) { }
    
        // 状态退出时调用
        protected override void OnLeave(IFsm<MetropolisBuildingBase> fsm, bool isShutdown) { }
    }
    
    // 待施工状态
    public class UnBuiltState : BuildingStateBase
    {
        private IFsm<MetropolisBuildingBase> m_Fsm;
        protected override void OnEnter(IFsm<MetropolisBuildingBase> fsm)
        {
            m_Fsm = fsm;
            
            MetropolisBuildingBase building = fsm.Owner;
            building.GetComponent<Collider2D>().enabled = true;
            building.GetComponent<SpriteRenderer>().sprite = building.constructionSprite;
            
            // 订阅
            GameEntry.Event.Subscribe(OnConstructionCompletedEvent.EventId, OnConstructionCompleted);
            
            // 粒子特效之类的
            // 其他……
            base.OnEnter(fsm);
        }

        private void OnConstructionCompleted(object sender, GameEventArgs gameEventArgs)
        {
            OnConstructionCompletedEvent eventData = (OnConstructionCompletedEvent)gameEventArgs;

            if (m_Fsm.Owner == eventData.BuildingFsm)
            {
                m_Fsm.Owner.FireAllWorkers();
                ChangeState<CompletedState>(m_Fsm);
            }
        }

        protected override void OnUpdate(IFsm<MetropolisBuildingBase> fsm, float elapseSeconds, float realElapseSeconds)
        {
            MetropolisBuildingBase building = fsm.Owner;
            
            if (building.constructionProgress < 1 && building.workingHeroes.Count > 0)
            {
                // hero进来施工
                building.StartConstructionProcess();
            }
            
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
        }

        protected override void OnLeave(IFsm<MetropolisBuildingBase> fsm, bool isShutdown)
        {
            MetropolisBuildingBase building = fsm.Owner;

            building.StopCurrentCoroutine();
            
            // 取消订阅
            GameEntry.Event.Unsubscribe(OnConstructionCompletedEvent.EventId, OnConstructionCompleted);
            
            base.OnLeave(fsm, isShutdown);
        }
    }

    // 完成状态
    public class CompletedState : BuildingStateBase
    {
        protected override void OnEnter(IFsm<MetropolisBuildingBase> fsm)
        {
            base.OnEnter(fsm);
            
            MetropolisBuildingBase building = fsm.Owner;
            building.GetComponent<SpriteRenderer>().sprite = building.completedSprite;
            
            building.StartCompletedBehavior();
        }
        
        protected override void OnUpdate(IFsm<MetropolisBuildingBase> fsm, float elapseSeconds, float realElapseSeconds)
        {
            MetropolisBuildingBase building = fsm.Owner;
            
            building.UpdateCompletedBehavior();
            
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
        }

        protected override void OnLeave(IFsm<MetropolisBuildingBase> fsm, bool isShutdown)
        {
            MetropolisBuildingBase building = fsm.Owner;

            base.OnLeave(fsm, isShutdown);
        }
    }
}
