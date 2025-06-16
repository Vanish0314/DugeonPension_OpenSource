using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using Dungeon.GOAP;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class ActionDataDungeonExit : IActionData
    {
        public Vector3 exit;
        private ITarget _target;

        public ITarget Target
        {
            get
            {
                return _target;
            }
            set
            {
#if UNITY_EDITOR
                if (value is DungeonExitTarget target)
                {
                    _target = target;
                }
                else
                {
                    GameFrameworkLog.Error("[ActionDataWithTransform] Target is not of type DungeonTransformTarget");
                }
#else
                DungeonExitTarget target = (DungeonExitTarget)value;
                exit = target.Position;
                _target = value;
#endif
            }
        }

    }
}

