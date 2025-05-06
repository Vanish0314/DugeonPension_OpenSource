using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Overload;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Dungeon
{
    [Serializable]
    public class CurseTypeButton
    {
        public CurseType type;
        public Button button;
    }
    public class CurseController : MonoBehaviour
    {
        [SerializeField] private CursesForm mCursesForm;
        
        [SerializeField] private CurseTypeButton[] buttons;
        
        private CursesManager m_CursesManager;

        private void Awake()
        {
            mCursesForm = gameObject.GetComponent<CursesForm>();
            m_CursesManager = CursesManager.Instance;
            
            InitializeButtons();
        }
        
        private void OnEnable()
        {
            SubscribeEvents();
            RefreshAllUI();
        }
        
        private void InitializeButtons()
        {
            foreach (var btn in buttons)
            {
                btn.button.onClick.AddListener(() => OnCurseButtonClick(btn.type));
            }
        }

        private void OnCurseButtonClick(CurseType btnType)
        {
            m_CursesManager.SelectCurse(btnType);
        }
        
        private void SubscribeEvents()
        {
            
        }
        
        private void RefreshAllUI()
        {
            mCursesForm.SetupAllCursesUI();
        }

    }
}
