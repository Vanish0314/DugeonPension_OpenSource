using System.Collections;
using System.Collections.Generic;
using Dungeon.Overload;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    [System.Serializable]
    public class CursesUI
    {
        public CurseType type;
        public Text nameText;
        public Text infoText;
        public Image image;
    }
    public class CursesForm : UGuiForm
    {
        [SerializeField] private List<CursesUI> m_CursesUI = new List<CursesUI>();
        private List<OverloadCurse> m_Curses;
        private Dictionary<CurseType, CursesUI> m_CursesUIDict = new Dictionary<CurseType, CursesUI>();
       
        private void Awake()
        {
           InitializeOverloadCurseDictionary();
        }
        private void InitializeOverloadCurseDictionary()
        {
            m_Curses = DungeonGameEntry.DungeonGameEntry.OverloadPower.CurseList;
            foreach (var curse in m_CursesUI)
            {
                if (m_CursesUIDict.TryAdd(curse.type, curse))
                {
                    GameFrameworkLog.Error("Dungeon Game Entry has already been added: " + curse.ToString());
                    continue;
                }
            }
        }

        public void SetupAllCursesUI()
        {
            foreach (var curse in m_Curses)
            {
                SetupCurseUI(curse);
            }
        }

        private void SetupCurseUI(OverloadCurse curse)
        {
            var curseType = curse.curseType;
            if (m_CursesUIDict.TryGetValue(curseType, out CursesUI curseUI))
            {
                curseUI.nameText.text = curse.curseName;
                curseUI.infoText.text = curse.curseDescription;
                curseUI.image.sprite = curse.curseIcon;
            }
        }
    }
}
