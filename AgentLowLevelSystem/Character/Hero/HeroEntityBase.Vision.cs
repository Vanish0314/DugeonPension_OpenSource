using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using Dungeon.DungeonEntity;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon.Character
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
            mViewer = GetComponent<AgentLowLevelSystem>().GetViewer();
            mVision = mViewer.GetVision();
        }


    }
}
