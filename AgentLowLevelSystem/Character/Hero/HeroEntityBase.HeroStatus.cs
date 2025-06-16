using DG.Tweening;
using Dungeon.DungeonEntity;
using UnityEngine;

namespace Dungeon.Character
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

        public int GetMp()
        {
            return m_LowLevelSystem.Mp;
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
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.mass = 0.01f;
                rb.drag = 1f;
                
                // 死后获得资源
                GetComponent<AgentLowLevelSystem>().m_Backpack.GatherResource();
            });
        }

        public void OnRevival()
        {
            m_GoapActionProvider.enabled = true;

            m_LowLevelSystem.enabled = true;
            m_HighLevelSystem.enabled = true;
            m_GoapActionProvider.enabled = true;
            m_GoapAgent.enabled = true;

            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.mass = 1.0f;
            rb.drag = 0f;
        }

        public void OnArrivedAtDungeonExit()
        {
            m_IsHappyingAtDungeonExit = true;

            DOVirtual.DelayedCall(2f, () =>
            {
                //TODO(vanish): 设置动画为 happy

                m_LowLevelSystem.enabled = false;
                m_HighLevelSystem.enabled = false;
                m_GoapActionProvider.enabled = false;
                m_GoapAgent.enabled = false;

                var rb = GetComponent<Rigidbody2D>();
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.bodyType = RigidbodyType2D.Static;
            });
        }

        public bool IsHappyingAtDungeonExit()
        {
            return m_IsHappyingAtDungeonExit;
        }

        public void SetIsHappyingAtDungeonExit(bool value)
        {
            m_IsHappyingAtDungeonExit = value;
        }

        private bool m_IsHappyingAtDungeonExit;
    }
}
