using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class MonsterLair : ProducingBuildingBase
    {
        private void Start()
        {
            ResourceModel.Instance.MagicPower += 10;
            string text = "+" + 10;
            FeelSystem.Instance.FloatingText(text, transform, produceData.productionGradient);
        }
    }
}
