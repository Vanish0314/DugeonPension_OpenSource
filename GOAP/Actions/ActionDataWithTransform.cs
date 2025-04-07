using CrashKonijn.Agent.Core;
using Dungeon.GOAP.Target;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class ActionDataWithTransform : IActionData
    {
        public Transform transform => _target.transform;
        private DungeonTransformTarget _target;
        public ITarget Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (value is DungeonTransformTarget target)
                {
                    _target = target;
                }
                else
                {
                    GameFrameworkLog.Error("[ActionDataWithTransform] Target is not of type DungeonTransformTarget");
                }
            }
        }

    }
}
