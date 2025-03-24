using System.Collections;
using System.Collections.Generic;
using Dungeon.AgentLowLevelSystem;
using Dungeon.DungeonEntity;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon.DungeonEntity.Trap
{
    public abstract class DungeonTrap : DungeonVisibleEntity
    {
        [SerializeField] protected DndCheckTarget mDndCheckTarget;
    }
}
