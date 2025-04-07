using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon.GOAP.Keys.WorldKeys.Local
{
    public class LocalHeroInventoryItemOf<T> : WorldKeyBase where T : IInventoryItem
    {
        
    }

    public interface IInventoryItem{}
}
