using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.GridSystem
{
    [CreateAssetMenu(menuName = "Tiles/Dungeon Rule Tile")]
    public class DungeonRuleTile : RuleTile,IDugeonTile
    {
        [SerializeField] private TileType tileType;

        TileType IDugeonTile.TileType { get => tileType; set => tileType = value; }
    }

    public interface IDugeonTile
    {
        public TileType TileType { get; set; }
    }
}
