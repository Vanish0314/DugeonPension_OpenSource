// CanteenUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Dungeon
{
    public class CanteenUI : MonoBehaviour
    {
        [Header("UI组件")] 
        [SerializeField] private GameObject cookUI;
        [SerializeField] private Transform foodButtonParent;
        [SerializeField] private Transform cookingSlotsParent;
        [SerializeField] private GameObject foodButtonPrefab;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Button startButton;
        [SerializeField] private GameObject progressBar;

        public Canteen m_Canteen;
        private Dictionary<FoodData, ProgressBar> m_ProgressBars = new();
        public FoodData mSelectedFood;

        private void Awake()
        {
            startButton.onClick.AddListener(OnStartClicked);
        }

        public void Initialize(Canteen canteen)
        {
            m_Canteen = canteen;
            CreateFoodButtons();
            CreateCookingSlots();
        }

        private void CreateFoodButtons()
        {
            foreach (Transform child in foodButtonParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var foodData in m_Canteen.foodConfig)
            {
                var button = Instantiate(foodButtonPrefab, foodButtonParent);
                button.GetComponent<Image>().sprite = foodData.foodIcon;
                button.GetComponent<Button>().onClick.AddListener(() => SelectFood(foodData));
            }
        }

        private void CreateCookingSlots()
        {
            foreach (Transform child in cookingSlotsParent)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < 4; i++)
            {
                var slot = Instantiate(slotPrefab, cookingSlotsParent);
                var slotScript = slot.GetComponent<CookingSlot>();
                slotScript.Initialize(i, this);
            }
        }

        public void ShowCookUI()
        {
            Debug.Log("Showing cook UI");
            cookUI.SetActive(true);
        }

        public void HideCookUI()
        {
            cookUI.SetActive(false);
        }
        
        public void UpdateSlot(int slotIndex, FoodData data)
        {
            if (slotIndex < 0 || slotIndex >= cookingSlotsParent.childCount)
            {
                Debug.LogError($"无效槽位索引: {slotIndex}");
                return;
            }

            var slot = cookingSlotsParent.GetChild(slotIndex).GetComponent<CookingSlot>();
            slot.UpdateSlot(data);
        }

        public void ShowProgressUI()
        {
            progressBar.SetActive(true);
        }

        public void HideProgressUI()
        {
            progressBar.SetActive(false);
        }
        
        public void UpdateProgress(FoodData data, float progress)
        {
            progressBar.GetComponent<ProgressBar>().SetProgress(progress);
        }

        public void ResetAllProgress()
        {
            foreach (var bar in m_ProgressBars.Values)
            {
                Destroy(bar.gameObject);
            }
            m_ProgressBars.Clear();
        }

        private void SelectFood(FoodData data)
        {
            mSelectedFood = data;
        }

        private void OnStartClicked()
        {
            m_Canteen.hasWork = true;
            HideCookUI();
        }
    }
}