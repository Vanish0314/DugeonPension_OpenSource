using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class MetropolisHPForm : UGuiForm
    {
        [SerializeField] private Slider MetropolisHPSlider;

        private int flag = 0;
        private void OnEnable()
        {
            UpdateMetropolisHP(MetropolisHPModel.Instance.MetropolisHP);
            MetropolisHPModel.Instance.OnMetropolisHPChanged += UpdateMetropolisHP;
        }

        private void OnDestroy()
        {
            MetropolisHPModel.Instance.OnMetropolisHPChanged -= UpdateMetropolisHP;
        }
        
        private void UpdateMetropolisHP(float metropolisHP)
        {
            MetropolisHPSlider.value = metropolisHP/100;
        }
    }
}
