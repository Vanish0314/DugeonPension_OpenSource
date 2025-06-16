using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using Dungeon.Character;
using UnityEngine;

namespace Dungeon.Character
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
