using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour
    {
        public void AddBuff(Buff buff)
        {
            if (buffs.Contains(buff)) // TODO: 同类型 etc.
            {
                return;
            }

            buffs.Add(buff);
            buff.OnApplied(m_Properties);
        }
        private void UpdateBuffs()
        {
            foreach (Buff buff in buffs)
            {
                buff.OnUpdate(Time.deltaTime);
                if (buff.remainingDuration <= 0)
                {
                    buffs.Remove(buff);
                    buff.OnRemoved(m_Properties);
                }
            }
        }

        private List<Buff> buffs = new();
    }

    public abstract class Buff
    {
        public Buff(float duration)
        {
        }
        public virtual void OnUpdate(float deltaTime)
        {
            remainingDuration -= deltaTime;
        }
        public abstract void OnApplied(HeroProperties propertiesToApply); public abstract void OnRemoved(HeroProperties propertiesToRemove);
        public float remainingDuration;
        // private int remainingTimesToUse;
    }

    public class Buff_Charming : Buff
    {
        public Buff_Charming(float duration, int amount) : base(duration)
        {
            this.amount = amount;
        }

        public override void OnApplied(HeroProperties propertiesToApply)
        {
            propertiesToApply.dndSkillData.Charisma += amount;
        }

        public override void OnRemoved(HeroProperties propertiesToRemove)
        {
            propertiesToRemove.dndSkillData.Charisma -= amount;
        }
        private int amount;
    }
}
