using System.Collections;
using System.Collections.Generic;
using Dungeon.AgentLowLevelSystem;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon.Character.Hero
{
    public partial class HeroEntityBase : DungeonVisitorEntity
    {
        public override VisitInformation OnUnvisited(VisitInformation visiter)
        {
            throw new System.NotImplementedException();
        }

        public override VisitInformation OnVisited(VisitInformation visiter)
        {
            throw new System.NotImplementedException();
        }

        public override void InitViewer()
        {
            mViewer = GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>().GetViewer();
            mVision = mViewer.GetVision();
        }


    }
}
