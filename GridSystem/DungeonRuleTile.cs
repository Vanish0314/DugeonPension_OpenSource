using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.GridSystem
{
    [CreateAssetMenu(menuName = "Tiles/Dungeon Rule Tile")]
    public class DungeonRuleTile : RuleTile,IDungeonTile
    {
        [SerializeField] private TileType tileType;

        TileType IDungeonTile.TileType { get => tileType; set => tileType = value; }
    }

    public interface IDungeonTile
    {
        public TileType TileType { get; set; }
    }
}
