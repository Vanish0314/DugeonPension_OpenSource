using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Struct;
using UnityEngine;

namespace Dungeon.Character.Interfaces
{
    public interface IDefensible : ICharacterBehaviour
    {
        public void TakeDamage(Damage damage);
    }
}
