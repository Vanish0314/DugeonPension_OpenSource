using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using UnityEngine;

namespace Dungeon.Character
{
    public interface IDefensible : ICharacterBehaviour
    {
        public void TakeDamage(Damage damage);
    }
}
