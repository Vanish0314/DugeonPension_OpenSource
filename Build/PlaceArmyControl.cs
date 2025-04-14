using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace Dungeon
{
    public class PlaceArmyControl : IReference
    {
        private PlaceManager m_PlaceManager;

        public void OnEnter()
        {
            if (m_PlaceManager == null)
                return;

            m_PlaceManager.inputReader.OnBuildEvent += StartBuilding;
            m_PlaceManager.inputReader.OnBuildEndEvent += EndBuilding;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        public void OnLeave()
        {
            if (m_PlaceManager == null)
                return;

            m_PlaceManager.inputReader.OnBuildEvent -= StartBuilding;
            m_PlaceManager.inputReader.OnBuildEndEvent -= EndBuilding;
        }

        private void StartBuilding()
        {
            GameEntry.UI.OpenUIForm(EnumUIForm.PlaceArmyForm);
        }

        private void EndBuilding()
        {
            GameEntry.UI.GetUIForm(EnumUIForm.PlaceArmyForm).Close();
        }

        public static PlaceArmyControl Create(PlaceManager placeManager)
        {
            PlaceArmyControl placeArmyControl = ReferencePool.Acquire<PlaceArmyControl>();
            placeArmyControl.m_PlaceManager = placeManager;
            return placeArmyControl;
        }

        public void Clear()
        {
            m_PlaceManager = null;
        }
    }
}
