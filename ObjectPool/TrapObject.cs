using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.ObjectPool;
using UnityEngine;

namespace Dungeon
{
    public class TrapObject : ObjectBase
    {
        public static TrapObject Create(object target)
        {
            TrapObject trapObject = ReferencePool.Acquire<TrapObject>();
            trapObject.Initialize(target);
            return trapObject;
        }

        protected override void Release(bool isShutdown)
        {
            GameObject trap = Target as GameObject;
            if (trap != null)
            {
                Object.Destroy(trap);
            }
        }
    }
}
