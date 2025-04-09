using System.Collections;
using System.Collections.Generic;
using Dungeon.BlackBoardSystem;
using UnityEngine;

namespace Dungeon.GridSystem
{
    public partial class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        public int GetInsistence(Blackboard blackboard)
        {
            return (int)InsistenceLevel.Dictator;
        }

        public void Execute(Blackboard blackboard)
        {
        }
    }
}
