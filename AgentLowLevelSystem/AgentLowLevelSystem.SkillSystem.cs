using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using GameFramework;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem
    {
        public void UseSkill(Skill skill, Vector3 targetPosOrDirection)
        {
            GameFrameworkLog.Info("[AgentLowLevelSystem] UseSkill: " + skill.desc.name);
        }
        public void UseSkill(SkillDesc skill, Vector3 targetPosOrDirection)
        {
            GameFrameworkLog.Info("[AgentLowLevelSystem] UseSkill: " + skill.name);
        }
    }


}
