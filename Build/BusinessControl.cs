using System.Collections;
using System.Collections.Generic;
using Dungeon;
using GameFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dungeon
{
    public class BusinessControl : IReference
    {
        private PlaceManager m_PlaceManager;

        public void OnEnter()
        {
            if(m_PlaceManager == null)
                return; 
            
            m_PlaceManager.inputReader.OnBuildEvent += StartBuilding;
            m_PlaceManager.inputReader.OnBuildEndEvent += EndBuilding;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
                
        }

        public void OnLeave()
        {
            if(m_PlaceManager == null)
                return; 
            
            m_PlaceManager.inputReader.OnBuildEvent -= StartBuilding;
            m_PlaceManager.inputReader.OnBuildEndEvent -= EndBuilding;
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
        
        public static BusinessControl Create(PlaceManager placeManager)
        {
            BusinessControl businessControl = ReferencePool.Acquire<BusinessControl>();
            businessControl.m_PlaceManager = placeManager;
            return businessControl;
        }
        
        public void Clear()
        {
            m_PlaceManager = null;
        }
    }
}
