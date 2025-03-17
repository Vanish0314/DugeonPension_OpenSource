using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Dungeon
{
    public class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem
    {
        public void MoveTo(Vector2Int targetPositionWorldCoord)
        {
            GameFrameworkLog.Info("AgentLowLevelSystem.MoveTo()");
            throw new System.NotImplementedException();
        }

        public void TargetChanged(ITarget target, bool inRange)
        {
            throw new System.NotImplementedException();
        }

        public void TargetInRange(ITarget target)
        {
            throw new System.NotImplementedException();
        }

        public void TargetLost()
        {
            throw new System.NotImplementedException();
        }

        public void TargetNotInRange(ITarget target)
        {
            throw new System.NotImplementedException();
        }
    }
}
