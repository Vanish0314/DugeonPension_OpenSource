using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class HeroInfoController : MonoBehaviour
    {
        private HeroInfoForm m_HeroInfoForm;
        private void Start()
        {
            m_HeroInfoForm = GetComponent<HeroInfoForm>();

            m_HeroInfoForm.returnBtn.onClick.AddListener(ReturnPlaceArmy);
        }

        private void ReturnPlaceArmy()
        {
            GameEntry.UI.CloseUIForm(m_HeroInfoForm);
            GameEntry.UI.OpenUIForm(EnumUIForm.PlaceArmyForm);
        }
    }
}
