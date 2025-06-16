using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using UnityEngine;

namespace Dungeon
{
    public class MoveBuffDecorator : IMoveable
    {
        protected IDefensible m_MoveabelRef;
        protected float m_Duration;

        public MoveBuffDecorator(IDefensible targetRef, float duration)
        {
            m_MoveabelRef = targetRef;
            m_Duration = duration;
        }

        public bool MoveTo(Vector2Int positionInWorldCoord)
        {
            throw new System.NotImplementedException();
        }
    }
}
