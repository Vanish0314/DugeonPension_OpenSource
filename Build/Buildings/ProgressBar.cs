using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Image fillImage;

        public void Initialize(string foodName)
        {
            slider.value = 0;
        }

        public void SetProgress(float progress)
        {
            slider.value = Mathf.Clamp01(progress);
        }
    }
}
