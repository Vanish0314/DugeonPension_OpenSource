using CrashKonijn.Agent.Core;
using Dungeon.GOAP;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP
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
