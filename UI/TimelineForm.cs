using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class TimelineForm : UGuiForm
    {
        [SerializeField] private Slider timelineSlider;

        public void UpdateTimeline(float timeline)
        {
            timelineSlider.value = timeline;
        }
    }
}
