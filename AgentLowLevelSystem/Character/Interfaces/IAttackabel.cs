using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Struct;
using UnityEngine;

namespace Dungeon.Character.Interfaces
{
    public interface IAttackable : ICharacterBehaviour
    {
        public Damage Attack();
    }
}
