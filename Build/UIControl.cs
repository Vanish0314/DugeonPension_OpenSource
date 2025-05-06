using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class UIControl : MonoBehaviour
    {
        public GameObject tile = null;
        MetropolisControl control;
        // Start is called before the first frame update
        void Start()
        {
            control = MetropolisControl.Create(PlaceManager.Instance);
            
            control.OnEnter();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameEntry.UI.OpenUIForm(EnumUIForm.ResourceFrom);
                tile.SetActive(true);
            }
        }
    }
}
