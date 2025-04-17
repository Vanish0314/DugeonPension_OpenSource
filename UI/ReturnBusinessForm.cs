using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class ReturnBusinessForm : MonoBehaviour
    {
        [SerializeField] private Button m_ReturnBusinessButton;

        private void Awake()
        {
            m_ReturnBusinessButton.onClick.AddListener(ReturnBusiness);
        }

        private void ReturnBusiness()
        {
            // 发送事件
            GameEntry.Event.GetComponent<EventComponent>().Fire(this, OnReturnBusinessButtunClickedEventArgs.Create());
        }
    }
}
