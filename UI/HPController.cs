using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class HPController : MonoBehaviour
    {
        private HPForm m_HPForm;

        private void Start()
        {
            m_HPForm = GetComponent<HPForm>();

            HPModel.Instance.OnHPChanged += UpdateHP;
        }

        private void UpdateHP()
        {
            m_HPForm.UpdateHP(TimelineModel.Instance.Timeline);
        }
    }
}
