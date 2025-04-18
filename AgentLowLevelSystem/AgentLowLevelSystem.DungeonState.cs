using System;
using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using Dungeon.Character.Hero;
using Dungeon.DungeonEntity.InteractiveObject;
using Dungeon.DungeonEntity.Monster;
using Dungeon.DungeonEntity.Trap;
using Dungeon.Vision2D;
using GameFramework;
using PlasticPipe.Tube;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem,ICombatable
    {
        private void OnDied()
        {
            SetAnimatorState(ANIMATOR_BOOL_DIE,99999);
            m_Vision.gameObject.SetActive(false);
            this.GetComponent<HeroEntityBase>().OnDie();
        }
    }
}
