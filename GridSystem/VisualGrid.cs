using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.GridSystem;
using GameFramework;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon.GridSystem
{
    [HideInInspector]
    public class VisualGrid : MonoBehaviour
    {
        public void Load(GridData data)
        {
            Init();

            var runtimeTileCache = new Dictionary<string, IDungeonTile>();

            foreach (var layer in data.layers)
            {
                foreach (var tile in layer.tiles)
                {
#if UNITY_EDITOR
                    if (string.IsNullOrEmpty(tile.tilePath))
                    {
                        GameFrameworkLog.Error("[VisualGrid] Tile path is null");
                    }
                    var tileAsset1 = Resources.Load<TileBase>(tile.tilePath);
                    if (tileAsset1 == null)
                    {
                        GameFrameworkLog.Error($"[VisualGrid] Tile not found at path: {tile.tilePath}");
                    }
                    if (tileAsset1 is not IDungeonTile dungeonTile1)
                    {
                        GameFrameworkLog.Error($"[VisualGrid] Tile is not a IDungeonTile: {tile.tilePath}");
                    }
#endif
                    if (!runtimeTileCache.TryGetValue(tile.tilePath, out var dungeonTile))
                    {
                        var tileAsset = Resources.Load<TileBase>(tile.tilePath);
                        if (tileAsset is not IDungeonTile castedTile)
                            continue;

                        dungeonTile = castedTile;
                        runtimeTileCache[tile.tilePath] = dungeonTile;
                    }

                    SetTile(new Vector2Int(tile.x, tile.y), dungeonTile);
                }
            }
        }
        public void Clear()
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
            m_BackGroundTileMap = null;
            m_BuildingsTileMap = null;
            m_DecorateTileMap = null;
        }

        public void Init()
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }

            m_BackGroundTileMap = GetOrCreateTilemap("BackGround");
            m_BuildingsTileMap = GetOrCreateTilemap("Buildings");
            m_DecorateTileMap = GetOrCreateTilemap("Debug");
        }

        private Tilemap GetOrCreateTilemap(string name)
        {
            var child = transform.Find(name);
            var obj = child != null ? child.gameObject : new GameObject(name);
            obj.transform.SetParent(transform);
            obj.GetOrAddComponent<TilemapRenderer>();
            obj.layer = LayerMask.NameToLayer("GridMap");
            return obj.GetOrAddComponent<Tilemap>();
        }

        [Obsolete("Visual should only be responsible for rendering, not for getting grid properties")]
        public GridProperties GetGridProperties() => new()
        {
            width = m_BackGroundTileMap.size.x,
            height = m_BackGroundTileMap.size.y,
            originPoint = m_BackGroundTileMap.origin
        };

        public void SetTile(Vector2Int gridPos, IDungeonTile tile)
        {
            if (!mFunctionToLayerMap.TryGetValue(tile.FunctionType, out var layer))
            {
                GameFrameworkLog.Error($"[VisualGrid] Unknown FunctionType: {tile.FunctionType}");
                return;
            }

            var pos = new Vector3Int(gridPos.x, gridPos.y, 0);
            var tileBase = tile as TileBase;

            switch (layer)
            {
                case VisualLayer.BackGround:
                    m_BackGroundTileMap.SetTile(pos, tileBase);
                    break;
                case VisualLayer.Buildings:
                    m_BuildingsTileMap.SetTile(pos, tileBase);
                    break;
                case VisualLayer.Decorate:
                    m_DecorateTileMap.SetTile(pos, tileBase);
                    break;
            }
        }
        private Tilemap m_BackGroundTileMap;
        private Tilemap m_BuildingsTileMap;
        private Tilemap m_DecorateTileMap;

        public enum VisualLayer
        {
            BackGround,
            Buildings,
            Decorate
        }
        public static Dictionary<TileFunctionType, VisualLayer> mFunctionToLayerMap = new()
        {
            { TileFunctionType.Default, VisualLayer.BackGround },
            { TileFunctionType.Trap, VisualLayer.Decorate },
            { TileFunctionType.Treasure, VisualLayer.Decorate },
            { TileFunctionType.Door, VisualLayer.Buildings },
        };

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                var bg = transform.Find("BackGround")?.gameObject ?? new GameObject("BackGround");
                var bt = transform.Find("Buildings")?.gameObject ?? new GameObject("Buildings");
                var dc = transform.Find("Debug")?.gameObject ?? new GameObject("Debug");

                m_BackGroundTileMap = bg.GetOrAddComponent<Tilemap>();
                m_BuildingsTileMap = bt.GetOrAddComponent<Tilemap>();
                m_DecorateTileMap = dc.GetOrAddComponent<Tilemap>();
            }
#endif
        }
    }
}