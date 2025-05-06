using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace Dungeon
{
    public class CurseUIControl : IReference
    {
        private CursesManager m_CurseManager;

        public void OnEnter()
        {
            if(m_CurseManager == null)
                return; 
            
            m_CurseManager.inputReader.OnOpenTargetUIEvent += StartCurse;
            m_CurseManager.inputReader.OnCloseTargetUIEvent += EndCurse;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
                
        }

        public void OnLeave()
        {
            if(m_CurseManager == null)
                return; 
            
            m_CurseManager.inputReader.OnOpenTargetUIEvent -= StartCurse;
            m_CurseManager.inputReader.OnCloseTargetUIEvent -= EndCurse;
        }

        private void StartCurse()
        {
            GameEntry.UI.OpenUIForm(EnumUIForm.CurseForm);
        }
        
        private void EndCurse()
        {
            GameEntry.UI.GetUIForm(EnumUIForm.CurseForm).Close();
        }
        
        public void Pause()
        {
            TimeManager.Instance.SetPaused(true);
        }

        public void Resume()
        {
            TimeManager.Instance.SetPaused(false);
        }
        
        public static CurseUIControl Create(CursesManager curseManager)
        {
            CurseUIControl curseUIControl = ReferencePool.Acquire<CurseUIControl>();
            curseUIControl.m_CurseManager = curseManager;
            return curseUIControl;
        }
        
        public void Clear()
        {
            m_CurseManager = null;
        }
    }
}
