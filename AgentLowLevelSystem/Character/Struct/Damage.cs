using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using UnityEngine;

namespace Dungeon.Character
{
    /// <summary>
    /// 
    /// </summary>
    public struct Damage
    {
        public DamageType damageType;
        public int damage;
        public float range;

        public IAttackable attacker;
    }

    public enum DamageType
    {
        Physical,
    }
}
