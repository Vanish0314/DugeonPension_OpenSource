using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

namespace Dungeon
{
    public class HPForm : UGuiForm
    {
        public Slider HPSlider;
        public void UpdateHP(float hp)
        {
            HPSlider.value = hp;
        }
    }
}
