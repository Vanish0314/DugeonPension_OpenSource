using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Interfaces;
using UnityEngine;

namespace Dungeon.Character.Struct
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
