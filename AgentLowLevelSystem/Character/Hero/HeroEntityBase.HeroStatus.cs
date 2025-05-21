using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dungeon.Character.Hero;
using Dungeon.Character.Struct;
using Dungeon.DungeonGameEntry;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon.Character.Hero
{
    public partial class HeroEntityBase : DungeonVisitorEntity
    {
        public int GetHp()
        {
            return m_LowLevelSystem.Hp;
        }
        public int GetMaxHp()
        {
            return m_LowLevelSystem.MaxHp;
        }

        public bool IsAlive()
        {
            return m_LowLevelSystem.IsAlive();
        }

        public bool IsFainted() => m_LowLevelSystem.IsFainted;

        public void GoDie()
        {
            m_LowLevelSystem.Hp -= 1000000;
        }

        public void OnDie()
        {
            m_GoapActionProvider.enabled = false;
            DungeonGameEntry.DungeonGameEntry.Event.Fire(OnOneHeroDiedInDungeonEvent.EventId, OnOneHeroDiedInDungeonEvent.Create(this));
            
            DOVirtual.DelayedCall(0.5f,()=>{
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
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.bodyType = RigidbodyType2D.Static;
            });
        }

        public bool IsHappyingAtDungeonExit()
        {
            return m_IsHappyingAtDungeonExit;
        }

        private bool  m_IsHappyingAtDungeonExit;
    }
}
