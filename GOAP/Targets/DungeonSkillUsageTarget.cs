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
        public SkillDesc skillToUse;
        public Vector3 PositionOrDirToUseSkill => Position;
        public Vector3 Position { get; set; }

        public DungeonSkillUsageTarget(Vector3 PositionOrDirectionToUseSkill, SkillDesc skill)
        {
            Position = PositionOrDirectionToUseSkill;
            skillToUse = skill;
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
