using System;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class FightSettlementController : MonoBehaviour
    {
        private FightSettlementController m_FightSettlementController;
        private PlaceManager m_PlaceManager;
        
        [SerializeField]private Button mContinueButton;
        
        private void Awake()
        {
            m_FightSettlementController = GetComponent<FightSettlementController>();
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
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnFightSettlementContinueEventArgs.Create());
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
    }
}