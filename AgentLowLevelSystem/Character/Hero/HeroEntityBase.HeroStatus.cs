using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dungeon.Character.Hero;
using Dungeon.Character.Struct;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon.Character.Hero
{
    public partial class HeroEntityBase : DungeonVisitorEntity
    {
        public bool IsAlive()
        {
            return m_LowLevelSystem.Hp > 0;
        }

        public void OnDie()
        {
            m_GoapActionProvider.enabled = false;
        }

        public void OnArrivedAtDungeonExit()
        {
            m_IsHappyingAtDungeonExit = true;

            DOVirtual.DelayedCall(1.5f,()=>{
                //TODO(vanish): 设置动画为 happy

                m_LowLevelSystem.enabled = false;
                m_HighLevelSystem.enabled = false;
                m_GoapActionProvider.enabled = false;
                m_GoapAgent.enabled = false;
                
                var rb= GetComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Static;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
            });
        }

        public bool IsHappyingAtDungeonExit()
        {
            return m_IsHappyingAtDungeonExit;
        }

        private bool  m_IsHappyingAtDungeonExit;
    }
}
