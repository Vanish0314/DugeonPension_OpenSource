using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using UnityEngine;

namespace Dungeon.Character
{
    public interface IAttackable : ICharacterBehaviour
    {
        public Damage Attack();
    }
}
