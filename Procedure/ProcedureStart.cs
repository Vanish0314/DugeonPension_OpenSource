using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Dungeon
{
    public class ProcedureStart : ProcedureBase
    {
        VarInt32 targetId = 2;

        private static float mTimer = 1f;
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Debug.Log("Procedure Start");
            
            targetId = 2;//初始化场景id

            base.OnEnter(procedureOwner);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            mTimer -= elapseSeconds;

            if (mTimer <= 0)//点击开始游戏按钮
            {
                ChangeState<ProcedureChangeScene>(procedureOwner);
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
