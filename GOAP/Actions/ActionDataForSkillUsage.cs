using CrashKonijn.Agent.Core;
using Dungeon.Target;
using GameFramework;
using UnityEngine;

namespace Dungeon.GOAP.Action
{
    public class ActionDataForSkillUsage : IActionData
    {
        public SkillDesc SkillToUse => skill;
        public Vector3 PositionToUseSkill => skillPos;
        public Vector3 DirectionToUseSkill => Vector3.Normalize(skillPos);

        private ITarget _target;
        private SkillDesc skill;
        private Vector3 skillPos;

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
                }
                else
                {
                    GameFrameworkLog.Error("[ActionDataWithTransform] Target is not of type DungeonTransformTarget");
                }
#else
                DungeonSkillUsageTarget target = ((DungeonSkillUsageTarget)value);
                skillPos = target.Position;
                skill = target.skillToUse;
                _target = value;
#endif
            }
        }

    }
}
