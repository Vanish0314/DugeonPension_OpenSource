using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class CookingSlot : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Sprite defaultSprite;
        private int m_SlotIndex;
        private CanteenUI m_UI;
        private FoodData m_CurrentFood;

        public void Initialize(int index, CanteenUI ui)
        {
            m_SlotIndex = index;
            m_UI = ui;
            GetComponent<Button>().onClick.AddListener(OnSlotClicked);
        }

        // 在 CookingSlot 类中增强状态控制
        public void UpdateSlot(FoodData data)
        {
            m_CurrentFood = data;
    
            // 当数据为空时显示默认状态
            if (data == null)
            {
                iconImage.sprite = defaultSprite;
                iconImage.enabled = true;
                return;
            }

            // 正常显示食物
            iconImage.sprite = data.foodIcon;
            iconImage.enabled = true;
        }
        
        private void OnSlotClicked()
        {
            if (m_UI.mSelectedFood != null)
            {
                // 尝试添加食物
                if (m_CurrentFood == null)
                {
                    m_UI.m_Canteen.AddToCookingPlan(m_UI.mSelectedFood, m_SlotIndex);
                    m_UI.mSelectedFood = null; // 清空选择
                }
            }
            else
            {
                // 移除当前食物
                if (m_CurrentFood != null)
                {
                    m_UI.m_Canteen.RemoveFromCookingPlan(m_SlotIndex);
                }
            }
        }
    }
}
