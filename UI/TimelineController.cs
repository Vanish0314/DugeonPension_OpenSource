using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class TimelineController : MonoBehaviour
    {
       private TimelineForm m_TimelineForm;

       private void Start()
       {
           m_TimelineForm = GetComponent<TimelineForm>();

           TimelineModel.Instance.OnTimelineChanged += UpdateTimeline;
       }

       private void UpdateTimeline()
       {
           m_TimelineForm.UpdateTimeline(TimelineModel.Instance.Timeline);
       }
    }
}
