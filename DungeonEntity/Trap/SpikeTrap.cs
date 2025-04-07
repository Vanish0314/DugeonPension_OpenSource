using System.Collections;
using System.Collections.Generic;
using Dungeon.AgentLowLevelSystem;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon.DungeonEntity.Trap
{
    public class SpikeTrap : DungeonTrapBase
    {
        public override VisitInformation OnUnvisited(VisitInformation visitInfo)
        {
            var result = new VisitInformation();
            result.visiter = visitInfo.visiter;
            result.visited = gameObject;
            
            return result;
        }

        public override VisitInformation OnVisited(VisitInformation visitInfo)
        {
            var result = new VisitInformation();
            
            var visiter = visitInfo.visiter;
            var low = visiter.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

            if (low == null)
                return result;

            result.visiter = visiter;

            var checkResut = low.DndCheck(mDndCheckTarget);

            if (checkResut.Succeed)
                result.visited = gameObject;

            return result;
        }
    }
}
