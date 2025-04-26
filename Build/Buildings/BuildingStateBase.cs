using System.Collections;
using System.Collections.Generic;
using GameFramework.Fsm;
using UnityEngine;

namespace Dungeon
{
    public abstract class BuildingStateBase : FsmState<MetropolisBuildingBase>
    {
        // 状态进入时调用
        protected override void OnEnter(IFsm<MetropolisBuildingBase> fsm) { }
    
        // 状态退出时调用
        protected override void OnLeave(IFsm<MetropolisBuildingBase> fsm, bool isShutdown) { }
    
        // 状态更新时调用
        protected override void OnUpdate(IFsm<MetropolisBuildingBase> fsm, float elapseSeconds, float realElapseSeconds) { }
    }
    
    // 待施工状态
    public class AreaState : BuildingStateBase
    {
        protected override void OnEnter(IFsm<MetropolisBuildingBase> fsm)
        {
            MetropolisBuildingBase building = fsm.Owner;
            building.GetComponent<Collider2D>().enabled = true;
            Debug.Log($"进入待施工状态: {building.name}");
        }
    }

    // 施工中状态
    public class ConstructionState : BuildingStateBase
    {
        private IEnumerator m_ConstructionCoroutine;

        protected override void OnEnter(IFsm<MetropolisBuildingBase> fsm)
        {
            MetropolisBuildingBase building = fsm.Owner;
           // m_ConstructionCoroutine = ConstructionProcess(building);
            building.StartCoroutine(m_ConstructionCoroutine);
        }

        protected override void OnLeave(IFsm<MetropolisBuildingBase> fsm, bool isShutdown)
        {
            if (m_ConstructionCoroutine != null)
            {
                fsm.Owner.StopCoroutine(m_ConstructionCoroutine);
            }
        }

        // private IEnumerator ConstructionProcess(MetropolisBuildingBase building)
        // {
        //     building.constructionProgress = 0;
        //     while (building.constructionProgress < 1f)
        //     {
        //        // building.constructionSpeed = building.workingHeroes.Count > 0 ? 
        //             building.currentEfficiency : 0;
        //         building.constructionProgress += Time.deltaTime / 
        //             building.constructionDuration * building.constructionSpeed;
        //         yield return null;
        //     }
        //     ChangeState<CompletedState>(fsm);
        // }
    }

    // 完成状态
    public class CompletedState : BuildingStateBase
    {
        protected override void OnEnter(IFsm<MetropolisBuildingBase> fsm)
        {
            MetropolisBuildingBase building = fsm.Owner;
            building.GetComponent<SpriteRenderer>().sprite = building.completedSprite;
            Debug.Log($"建筑完成: {building.name}");
        }
    }
}
