using System.Collections;
using System.Collections.Generic;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon
{
    public class DungeonMonster : DungeonVisibleEntity
    {
        public override VisitInformation OnUnvisited(VisitInformation visiter)
        {
            visiter.visited = this.gameObject;
            return visiter;
        }

        public override VisitInformation OnVisited(VisitInformation visiter)
        {
            visiter.visited = this.gameObject;
            return visiter;
        }
    }
}
