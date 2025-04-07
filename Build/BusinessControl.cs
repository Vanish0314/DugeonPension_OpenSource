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
        private BuildManager m_BuildManager;
        
        public bool m_IsBusiness;

        public void OnEnter()
        {
            if(m_BuildManager == null)
                return; 
            
            m_IsBusiness = true;
            
            m_BuildManager.inputReader.OnBuildEvent += StartBuilding;
            m_BuildManager.inputReader.OnBuildEndEvent += EndBuilding;

            TimeManager.Instance.OnFiveMinutesElapsed += EndBuisness;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
                
        }

        public void OnLeave()
        {
            if(m_BuildManager == null)
                return; 
            
            m_IsBusiness = false;
            
            m_BuildManager.inputReader.OnBuildEvent -= StartBuilding;
            m_BuildManager.inputReader.OnBuildEndEvent -= EndBuilding;

            TimeManager.Instance.OnFiveMinutesElapsed -= EndBuisness;
        }

        private void StartBuilding()
        {
            Debug.Log("StartBuilding");
            GameEntry.UI.OpenUIForm(EnumUIForm.BuildForm);
        }
        
        private void EndBuilding()
        {
            Debug.Log("EndBuilding");
            GameEntry.UI.GetUIForm(EnumUIForm.BuildForm).Close();//-------------------
        }

        private void EndBuisness()
        {
            m_IsBusiness = false;
        }

        
        public void Pause()
        {
            TimeManager.Instance.SetPaused(true);
        }

        public void Resume()
        {
            TimeManager.Instance.SetPaused(false);
        }
        
        public static BusinessControl Create(BuildManager buildManager)
        {
            BusinessControl businessControl = ReferencePool.Acquire<BusinessControl>();
            businessControl.m_BuildManager = buildManager;
            businessControl.m_IsBusiness = true;
            return businessControl;
        }
        
        public void Clear()
        {
            
        }
    }

}
