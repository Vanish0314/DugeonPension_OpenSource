using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class MonsterLairUI : MonoBehaviour
    {
        [Header("生产状态")]
        [SerializeField] private GameObject statusUIPrefab;
        [SerializeField] private Slider statusSlider;
        
        [SerializeField] private Button gatherButtonPrefab;// 收获按钮
        [SerializeField] private Button upgradeButtonPrefab; // 唤起升级UI
        
        [Header("照看提醒")]
        public GameObject warningUIPrefab;

        private MonsterLair targetMonsterLair;
        private GameObject currentUI;

        private void Start()
        {
            targetMonsterLair = transform.parent.GetComponent<MonsterLair>();
            gatherButtonPrefab.onClick.AddListener(()=>targetMonsterLair.GatherResource());
            upgradeButtonPrefab.onClick.AddListener(OpenUpgradeUI);
        }

        protected virtual void OpenUpgradeUI()
        {
            GameEntry.UI.OpenUIForm(EnumUIForm.BlueprintUpgradeForm);
        }

        private void Update()
        {
            statusSlider.value = (float)targetMonsterLair.currentStock/targetMonsterLair.maxStock;
        }
        
        public void ShowMonsterLairUI()
        {
            CloseCurrentUI();
            ShowStatusUI();
            ShowGatherUI();
            ShowUpgradeUI();
        }

        private void ShowStatusUI()
        {
            currentUI = statusUIPrefab;
            currentUI.SetActive(true);
        }
        
        private void ShowGatherUI()
        {
            gatherButtonPrefab.gameObject.SetActive(true);
        }

        private void CloseGatherUI()
        {
            gatherButtonPrefab.gameObject.SetActive(false);
        }

        private void ShowUpgradeUI()
        {
            upgradeButtonPrefab.gameObject.SetActive(true);
        }

        private void CloseUpgradeUI()
        {
            upgradeButtonPrefab.gameObject.SetActive(false);
        }
        
        public void ShowWarningUI()
        {
            CloseAllUI();
            currentUI = warningUIPrefab;
            currentUI.SetActive(true);
        }
        
        public void CloseCurrentUI()
        {
            if (currentUI != null)
            {
                currentUI.SetActive(false);
            }
        }

        public void CloseAllUI()
        {
            CloseCurrentUI();
            CloseUpgradeUI();
            CloseGatherUI();
        }
    }
}
