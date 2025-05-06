using Dungeon.Common.MonoPool;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class HPBar : MonoPoolItem
    {
        [Header("HP Bar")]
        [SerializeField] private Slider m_HPBarSlider;
        [SerializeField] private Image m_HPBarFill;
        [SerializeField] private Color m_HPFullColor = Color.green;
        [SerializeField] private Color m_HPLowColor = Color.red;
        
        [Header("MP Bar")]
        [SerializeField] private Slider m_MPBarSlider;
        [SerializeField] private Image m_MPBarFill;
        [SerializeField] private Color m_MPFullColor = Color.blue;
        [SerializeField] private Color m_MPLowColor = Color.gray;
        
        private ICombatable m_Combatable;

        public void Initialize(ICombatable combatable)
        {
            transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            m_Combatable = combatable;
            UpdateHPBar();
            UpdateMPBar();
        }

        public override void OnSpawn(object data)
        {
            if (data is ICombatable combatable)
            {
                m_Combatable = combatable;
                UpdateHPBar();
                UpdateMPBar();
            }
        }

        public override void Reset()
        {
            m_Combatable = null;
            m_HPBarSlider.value = 1f;
            m_HPBarFill.color = m_HPFullColor;
            m_MPBarSlider.value = 1f;
            m_MPBarFill.color = m_MPFullColor;
        }

        private void Update()
        {
            if (m_Combatable == null)
            {
                ReturnToPool();
                return;
            }
            
            if (m_Combatable != null)
            {
                UpdateHPBar();
                UpdateMPBar();
            }
            
            if (m_Combatable.Hp <= 0)
            {
                ReturnToPool();
            }
        }

        private void UpdateHPBar()
        {
            float hpRatio = m_Combatable.Hp / (float)m_Combatable.MaxHp;
            m_HPBarSlider.value = hpRatio;
            m_HPBarFill.color = Color.Lerp(m_HPLowColor, m_HPFullColor, hpRatio);
        }

        private void UpdateMPBar()
        {
            float mpRatio = m_Combatable.Mp / (float)m_Combatable.MaxMp;
            m_MPBarSlider.value = mpRatio;
            m_MPBarFill.color = Color.Lerp(m_MPLowColor, m_MPFullColor, mpRatio);
        }

        public override void OnReturnToPool()
        {
            base.OnReturnToPool();
            m_Combatable = null;
        }
    }
}