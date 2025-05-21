using CrashKonijn.Agent.Core;
using Dungeon.Target;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class ActionDataForSkillUsage : IActionData
    {
        public Vector3 PositionToUseSkill => posToUseSkill; // 怪物的位置
        public Vector3 DirectionToUseSkill => dirToUseSkill; // 怪物相对勇者的方向
 
        private ITarget _target;
        private Vector3 dirToUseSkill;
        private Vector3 posToUseSkill;

        public ITarget Target
        {
            get
            {
                return _target;
            }
            set
            {
#if UNITY_EDITOR
                if (value is DungeonSkillUsageTarget target)
                {
                    _target = target;
                    posToUseSkill = target.Position;
                    dirToUseSkill = target.Direction;
                }
                else
                {
                    GameFrameworkLog.Error("[ActionDataWithTransform] Target is not of type DungeonTransformTarget");
                }
#else
                DungeonSkillUsageTarget target = (DungeonSkillUsageTarget)value;
                posToUseSkill = target.Position;
                dirToUseSkill = target.Direction;
                _target = value;
#endif
            }
        }

    }
}
