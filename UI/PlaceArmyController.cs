using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class PlaceArmyController : MonoBehaviour
    {
        private PlaceArmyForm m_PlaceArmyForm;
        private BuildManager m_BuildManager;
        private void Start()
        {
            m_PlaceArmyForm = GetComponent<PlaceArmyForm>();
            m_BuildManager = FindObjectOfType<BuildManager>();
            
            m_PlaceArmyForm.heroInfoButton.onClick.AddListener(OpenHeroInfo);
            m_PlaceArmyForm.startButton.onClick.AddListener(StartDungeon);
        }

        private void StartDungeon()
        {
            GameEntry.Event.GetComponent<EventComponent>().Fire(this,OnPlaceArmyEndEventArgs.Create());
        }

        private void OpenHeroInfo()
        {
            GameEntry.UI.OpenUIForm(EnumUIForm.HeroInfoForm);
        }
    }
}
