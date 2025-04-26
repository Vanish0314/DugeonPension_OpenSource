using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class Castle : ProducingBuildingBase
    {
        private float m_Timer;
        protected override void Update()
        {
            base.Update();
            
            m_Timer += Time.deltaTime;
            float interval = produceData.GetProductionInterval();

            if (m_Timer >= interval)
            {
                m_Timer = 0;
                BuildModel.Instance.ModifyCount(BuildingType.MonsterLair,Random.Range(0,3));
                BuildModel.Instance.ModifyCount(BuildingType.Quarry,Random.Range(1,4));
                FeelSystem.Instance.FloatingText("随机图纸", transform, produceData.productionGradient);
            }
        }
    }
}
