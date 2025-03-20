using System.Collections;
using System.Collections.Generic;
using Dungeon.DungeonActor;
using UnityEngine;

namespace Dungeon
{
    public interface IVisiter : IDungeonBehavior
    {

    }
    public interface IVisible : IDungeonBehavior
    {
        public VisitData OnVisited(IVisiter visiter);
        public VisitData OnUnvisited();
    }

    public struct VisitData
    {
        public string name;
    }
}
