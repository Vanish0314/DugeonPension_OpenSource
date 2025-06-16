using System.Collections;
using System.Collections.Generic;
using Dungeon;
using GameFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dungeon
{
    public class MetropolisControl : IReference
    {
        private PlaceManager m_PlaceManager;

        public void OnEnter()
        {
            if(m_PlaceManager == null)
                return; 
            
            m_PlaceManager.inputReader.OnOpenTargetUIEvent += StartBuilding;
            m_PlaceManager.inputReader.OnCloseTargetUIEvent += EndBuilding;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
                
        }

        public void OnLeave()
        {
            if(m_PlaceManager == null)
                return; 
            
            m_PlaceManager.inputReader.OnOpenTargetUIEvent -= StartBuilding;
            m_PlaceManager.inputReader.OnCloseTargetUIEvent -= EndBuilding;
        }

        private void StartBuilding()
        {
            GameEntry.UI.OpenUIForm(EnumUIForm.BuildForm);
        }
        
        private void EndBuilding()
        {
            GameEntry.UI.GetUIForm(EnumUIForm.BuildForm).Close();
        }
        
        public void Pause()
        {
            TimeManager.Instance.SetPaused(true);
        }

        public void Resume()
        {
            TimeManager.Instance.SetPaused(false);
        }
        
        public static MetropolisControl Create(PlaceManager placeManager)
        {
            MetropolisControl metropolisControl = ReferencePool.Acquire<MetropolisControl>();
            metropolisControl.m_PlaceManager = placeManager;
            return metropolisControl;
        }
        
        public void Clear()
        {
            m_PlaceManager = null;
        }
    }
}
