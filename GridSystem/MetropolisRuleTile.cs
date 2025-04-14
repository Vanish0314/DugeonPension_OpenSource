using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon.GridSystem
{
    [CreateAssetMenu(menuName = "Tiles/Metropolis Rule Tile")]
    public class MetropolisRuleTile : RuleTile, IMetropolisTile
    {
        [SerializeField] private TilePlacementType placementType;//---------------------------
        
        public TilePlacementType PlacementType{ get => placementType; set => placementType = value; }//----------------
    }

    [CreateAssetMenu(menuName = "Tiles/Metropolis Tile")]
    public class MetropolisTile : TileBase, IMetropolisTile
    {
        [SerializeField] private TilePlacementType placementType;
        
        public TilePlacementType PlacementType{ get => placementType; set => placementType = value; }
    }

    public interface IMetropolisTile
    {
        public TilePlacementType PlacementType { get; set; }
    }
}
