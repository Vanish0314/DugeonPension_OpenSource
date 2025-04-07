using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.Character.Interfaces
{
    public interface IMoveable : ICharacterBehaviour
    {
        /// <summary>
        /// return whether could move to the given position in world coordinate
        /// </summary>
        /// <param name="positionInWorldCoord"></param>
        /// <returns></returns>
        public bool MoveTo(Vector2Int positionInWorldCoord);
    }
}
