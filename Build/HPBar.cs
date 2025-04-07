using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;
        
        private ICombatable target;
        private int maxHP;
        
        public void Initialize(ICombatable combatable)
        {
            this.target = combatable;
            this.maxHP = combatable.Hp; // 假设ICombatable有GetMaxHP方法
            
            UpdateHPDisplay();
        }
        
        private void Update()
        {
            UpdateHPDisplay();
        }
        
        private void UpdateHPDisplay()
        {
            if (target == null) return;
            
            float fillAmount = (float)target.Hp / maxHP;
            hpSlider.value = fillAmount;
        }
    }
}