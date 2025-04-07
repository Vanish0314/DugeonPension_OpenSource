// using System.Collections.Generic;
// using Dungeon.GridSystem;
// using UnityEngine;
// using UnityEngine.Tilemaps;

// [CreateAssetMenu(menuName = "Tiles/DualGrid Rule Tile")]
// public class DualGridRuleTile : RuleTile<RuleTile.TilingRule.Neighbor>
// {

//     [Header("Grid System")]
//     public GridSystem grid;
//     /*
//     neighbor:
//     0,1,2

//     m_TilingRules[]:
//     0: uppper left
//     1: up
//     2: upper right
//     3:left
//     4:right
//     5:down left
//     6:down
//     7:down right
//     */
//     public Dictionary<int, int> i2tile = new()
//     {
//         { 0, 0 },
//         { 1, 2 },
//         { 2, 5 },
//         { 3, 7 }
//     };
//     public sealed override bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, ref Matrix4x4 transform)
//     {
//         for (int i = 0; i < 3; i++)
//         {
//             int neighborStatus = rule.m_Neighbors[0];
//             Vector3Int neighborPos = rule.m_NeighborPositions[0];

//             TileBase tile = tilemap.GetTile(neighborPos + position);
//             if (!RuleMatch(neighborStatus, tile))
//             {
//                 return false;
//             }
//         }

//         transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
//         return true;
//     }

//     public sealed override bool RuleMatch(int neighbor, TileBase other)
//     {
//         if (other is RuleOverrideTile ruleOverrideTile)
//         {
//             other = ruleOverrideTile.m_InstanceTile;
//         }

//         return neighbor switch
//         {
//             1 => other == this,
//             2 => other != this,
//             _ => true,
//         };
//     }

// }
