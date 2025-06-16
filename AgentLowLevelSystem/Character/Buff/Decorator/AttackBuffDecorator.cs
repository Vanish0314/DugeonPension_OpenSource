using Dungeon.Character;
using Dungeon.Character;

namespace Dungeon.Character
{
    public abstract class AttackBuffDecorator : IAttackable
    {
        protected IAttackable m_AttackableRef;
        protected float m_Duration;
        protected AttackBuffDecorator(IAttackable targetRef, float duration)
        {
            m_AttackableRef = targetRef;
            m_Duration = duration;
        }

        public abstract Damage Attack();
    }
}