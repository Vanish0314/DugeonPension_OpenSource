using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using UnityEngine;

namespace Dungeon.GOAP.Target
{
    public class DungeonTransformTarget : ITarget
    {
        public Transform transform;

        public DungeonTransformTarget(Transform entityTransform)
        {
            this.transform = entityTransform;
        }
        public Vector3 Position => transform.position;

        public bool IsValid()
        {
            return transform != null;
        }
    }
}
