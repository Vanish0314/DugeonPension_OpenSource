using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.GridSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon.GridSystem
{
    public class VisualGrid : MonoBehaviour
    {
        /// <summary>
        /// if backGroundTileMap is not null, will return it's grid properties, otherwise will return null
        /// this will create gird if it's not exist
        /// </summary>
        /// <returns></returns>
        public GridProperties? GetGridProperties()
        {
            m_BackGroundTileMap = transform.Find("BackGround")?.GetComponent<Tilemap>();
            if(m_BackGroundTileMap == null)
                return null;

            var bg = transform.Find("BackGround")?.gameObject ?? new GameObject("BackGround");
            var bt = transform.Find("Buildings")?.gameObject ?? new GameObject("Buildings");
            var bug = transform.Find("Debug")?.gameObject ?? new GameObject("Debug");
            bg.transform.parent = transform;
            bt.transform.parent = transform;
            bug.transform.parent = transform;

            m_BackGroundTileMap = bg.GetOrAddComponent<Tilemap>();
            m_BackGroundTileMapRenderer = bg.GetOrAddComponent<TilemapRenderer>();

            m_BuildingsTileMap = bt.GetOrAddComponent<Tilemap>();
            m_BuildingsTileMapRenderer = bt.GetOrAddComponent<TilemapRenderer>();

            m_DebugTileMap = bug.GetOrAddComponent<Tilemap>();
            m_DebugTileMapRenderer = bug.GetOrAddComponent<TilemapRenderer>();

            return new GridProperties()
            {
                width = m_BackGroundTileMap.size.x,
                height = m_BackGroundTileMap.size.y,
                cellSize = m_BackGroundTileMap.cellSize.x,
                originPoint = m_BackGroundTileMap.origin
            };
        }

        public void SetTileBases(DungeonRuleTile wallTile, DungeonRuleTile groundTile,DungeonRuleTile debugTile)
        {
            m_WallTile = wallTile;
            m_GroundTile = groundTile;
            m_DebugTile = debugTile;
        }

#region PUBLIC
        public void OnResize(GridProperties properties)
        {
            //TODO
        }

        public void SetTile(Vector2Int gridPos, TileDesc tileDesc)
        {
            TileBase tile;
            switch (tileDesc.type)
            {
                case TileType.Ground:
                    tile = m_GroundTile;
                    break;
                case TileType.Wall:
                    tile = m_WallTile;
                    break;
                case TileType.Debug:
                    tile = m_DebugTile;
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }

            m_BackGroundTileMap.SetTile(new Vector3Int(gridPos.x, gridPos.y, 0), tile);
        }

        public Tilemap GetBackGroundTileMap() => m_BackGroundTileMap;
        public DungeonRuleTile GetWallTile() => m_WallTile;
        public DungeonRuleTile GetGroundTile() => m_GroundTile;
#endregion

        private DungeonRuleTile m_WallTile;
        private DungeonRuleTile m_GroundTile;
        private DungeonRuleTile m_DebugTile;
        /// <summary>
        /// BackGround ,With only two kind of Tile: Groung , Wall
        /// </summary>
        private Tilemap m_BackGroundTileMap;
        private TilemapRenderer m_BackGroundTileMapRenderer;
        private Tilemap m_BuildingsTileMap;
        private TilemapRenderer m_BuildingsTileMapRenderer;
        private Tilemap m_DebugTileMap;
        private TilemapRenderer m_DebugTileMapRenderer;

    }
}
