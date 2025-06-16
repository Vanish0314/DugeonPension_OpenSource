using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class DungeonTransformTarget : ITarget
    {
        public Transform transform;
        public Vector3? cachedPosition;

        public DungeonTransformTarget(Transform entityTransform)
        {
            this.transform = entityTransform;
            cachedPosition = transform?.position;
        }
        public Vector3 Position
        {
            get
            {
                if (transform == null)
                {
                    if(cachedPosition == null)
                    {
                        GameFrameworkLog.Error("[DungeonTransformTarget] 空的transform和缓存位置,会导致错误行为");
                        return Vector3.zero;
                    }
                    else
                    {
                        return cachedPosition.Value;
                    }
                }
                else
                {
                    cachedPosition = transform.position;
                    return transform.position;
                }
            }
        }

        public bool IsValid()
        {
            return transform != null;
        }
    }
}
