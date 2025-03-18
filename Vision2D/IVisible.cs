using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public interface IVisiter
    {

    }
    public interface IVisible
    {
        public VisitData OnVisited(IVisiter visiter);
        public VisitData OnUnvisited();
    }

    public struct VisitData
    {
        public string name;
    }
}
