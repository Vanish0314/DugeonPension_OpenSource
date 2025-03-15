using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Interfaces;
using Dungeon.Character.Struct;
using UnityEngine;

namespace Dungeon.Character.Buff.Decorator
{
    public abstract class DefenceBuffDecorator : IDefensible
    {
        protected IDefensible m_DefensibleRef;
        protected float m_Duration;

        public DefenceBuffDecorator(IDefensible targetRef, float duration)
        {
            m_DefensibleRef = targetRef;
            m_Duration = duration;
        }

        public abstract void TakeDamage(Damage damage);
    }
}
