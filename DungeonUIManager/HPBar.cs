using Dungeon.Character;
using Dungeon.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class HPBar : MonoPoolItem
    {
        [Header("HP Bar")]
        [SerializeField] private Slider m_HPBarSlider;
        [SerializeField] private Image m_HPBarFill;
        [SerializeField] private TextMeshProUGUI m_HPBarText;
        [SerializeField] private Color m_HPFullColor = Color.green;
        [SerializeField] private Color m_HPLowColor = Color.red;
        
        [Header("Submissiveness Bar")]
        [SerializeField] private Slider m_SubmissivenessBarSlider;
        [SerializeField] private Image m_SubmissivenessBarFill;
        [SerializeField] private TextMeshProUGUI m_SubmissivenessBarText;
        [SerializeField] private Color m_SubmissivenessFullColor = Color.magenta;
        [SerializeField] private Color m_SubmissivenessLowColor = Color.gray;
        
        private ICombatable m_Combatable;

        public void Initialize(ICombatable combatable)
        {
            transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            m_Combatable = combatable;
            UpdateHPBar();
            m_SubmissivenessBarSlider.gameObject.SetActive(false);
            if (m_Combatable is AgentLowLevelSystem)
            {
                m_SubmissivenessBarSlider.gameObject.SetActive(true);
                UpdateSubmissivenessBar();
            }
        }

        public override void OnSpawn(object data)
        {
          
        }

        public override void Reset()
        {
            m_Combatable = null;
            m_HPBarSlider.value = 1f;
            m_HPBarFill.color = m_HPFullColor;
            m_HPBarText.text = string.Empty;
            m_SubmissivenessBarSlider.value = 1f;
            m_SubmissivenessBarFill.color = m_SubmissivenessFullColor;
            m_SubmissivenessBarText.text = string.Empty;
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
                if (m_Combatable is AgentLowLevelSystem)
                    UpdateSubmissivenessBar();
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
            m_HPBarText.text = m_Combatable.Hp.ToString();
        }

        private void UpdateSubmissivenessBar()
        {
            var hero = (AgentLowLevelSystem)m_Combatable;
            float submissiveness = hero.GetSubmissiveness() / 100f;
            m_SubmissivenessBarSlider.value = submissiveness;
            m_SubmissivenessBarFill.color = Color.Lerp(m_SubmissivenessLowColor, m_SubmissivenessFullColor, submissiveness);
            m_SubmissivenessBarText.text = hero.GetSubmissiveness().ToString();
        }

        public override void OnReturnToPool()
        {
            base.OnReturnToPool();
            m_Combatable = null;
        }
    }
}