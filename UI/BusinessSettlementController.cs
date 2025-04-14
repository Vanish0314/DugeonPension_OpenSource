using System;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class BusinessSettlementController : MonoBehaviour
    {
        private BusinessSettlementForm m_BusinessSettlementForm;
        private PlaceManager m_PlaceManager;
        
        [SerializeField]private Button mContinueButton;
        private void Awake()
        {
            m_BusinessSettlementForm = GetComponent<BusinessSettlementForm>();
            m_PlaceManager = PlaceManager.Instance;
        }
        
        private void OnEnable()
        {
            mContinueButton.onClick.AddListener(OnContinueButtonClicked);
            SubscribeEvents();
            RefreshUI();
        }

        private void OnContinueButtonClicked()
        {
            GameEntry.Event.GetComponent<EventComponent>().Fire(this,OnBusinessSettlementContinueEventArgs.Create());
        }

        private void RefreshUI()
        {
            
        }
        
        private void SubscribeEvents()
        {
      
        }

        private void OnDisable()
        {

        }

        private void OnButtonClick()
        {

        }
    }
}