using System.Collections;
using System.Collections.Generic;
using Dungeon;
using GameFramework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class ResourceFrom : UGuiForm
    {
        //获取UI组件
        [SerializeField] private Text GoldText;
        [SerializeField] private Text StoneText;
        [SerializeField] private Text MagicPowerText;
        [SerializeField] private Text MaterialText;

        public void UpdateGoldText(int goldValue)
        {
            GoldText.text = "金币：" + goldValue.ToString();
        }

        public void UpdateStoneText(int stoneValue)
        {
            StoneText.text = "石料：" + stoneValue.ToString();
        }

        public void UpdateMagicPowerText(int magicPowerValue)
        {
            MagicPowerText.text = "魔力：" + magicPowerValue.ToString();
        }

        public void UpdateMaterialText(int materialValue)
        {
            MaterialText.text = "材料：" + materialValue.ToString();
        }

        public void SetSomeUIActive(bool active)
        {
            StoneText.transform.parent.gameObject.SetActive(active);
        }
    }
}
