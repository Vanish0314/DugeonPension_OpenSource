using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Hero;
using Dungeon.Common.MonoPool;
using Dungeon.Overload;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class CaptureButton : MonoPoolItem
    {
        [SerializeField] private Button m_Button;
        
        private HeroEntityBase m_TargetHero;
        private Vector3 m_Offset;

        public void Initialize(HeroEntityBase target, Vector3 offset)
        {
            m_TargetHero = target;
            m_Offset = offset;
            UpdatePosition();
            
            m_Button.onClick.RemoveAllListeners();
            m_Button.onClick.AddListener(OnClick);
            gameObject.SetActive(true);
        }

        public void UpdatePosition()
        {
            if (m_TargetHero != null)
            {
                transform.position = m_TargetHero.transform.position + m_Offset;
            }
        }

        private void OnClick()
        {
            if (m_TargetHero == null) return;
            
            // 执行捕捉逻辑
            DungeonGameEntry.DungeonGameEntry.OverloadPower.SpellCurse(m_TargetHero, CurseType.Capture);
            
            // 禁用并回收按钮
            ReturnToPool();
        }

        public override void OnSpawn(object data)
        {
            
        }

        public override void Reset()
        {
            m_Button.onClick.RemoveAllListeners();
            m_TargetHero = null;
            gameObject.SetActive(false);
        }

        public override void OnReturnToPool()
        {
            base.OnReturnToPool();
            transform.localPosition = Vector3.zero;
        }
    }
}