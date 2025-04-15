using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using UnityEngine;

namespace Dungeon.Target
{
    public class DungeonSkillUsageTarget : ITarget
    {
        /// <summary>
        /// The position to use the skill for ranged attacks.
        /// direction to use skill for melee attacks.
        /// </summary>
        public Vector3 Position { get; set; }

        public Vector3 Direction { get; set; }

        public DungeonSkillUsageTarget(Vector3 position, Vector3 direction)
        {
            Position = position;
            Direction = direction;
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
