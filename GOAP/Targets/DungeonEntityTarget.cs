using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using Dungeon.DungeonEntity;
using UnityEngine;

namespace Dungeon.Target
{
    [Obsolete("Use DungeonTransformTarget instead")]
    public class DungeonEntityTarget<TEntity> : ITarget where TEntity : DungeonEntity.DungeonEntity
    {
        public DungeonEntity.DungeonEntity Entity;
        public Vector3 Position => Entity.transform.position;

        public DungeonEntityTarget(TEntity entity)
        {
            this.Entity = entity;
        }
        public bool IsValid()
        {
            return Entity != null;
        }
    }
}
