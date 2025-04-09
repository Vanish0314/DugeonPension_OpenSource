using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.GridSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon.GridSystem
{
    [HideInInspector]
    public class VisualGrid : MonoBehaviour
    {
        public void Load(GridData data)
        {
            // TODO
            throw new NotImplementedException();
        }
        public void Init()
        {
            var bg = transform.Find("BackGround")?.gameObject ?? new GameObject("BackGround");
            var bt = transform.Find("Buildings")?.gameObject ?? new GameObject("Buildings");
            var dc = transform.Find("Debug")?.gameObject ?? new GameObject("Debug");
            bg.transform.parent = transform;
            bt.transform.parent = transform;
            dc.transform.parent = transform;

            m_BackGroundTileMap = bg.GetOrAddComponent<Tilemap>();
            m_BuildingsTileMap = bt.GetOrAddComponent<Tilemap>();
            m_DecorateTileMap = dc.GetOrAddComponent<Tilemap>();
        }
        [Obsolete("Visual should only responsible for rendering, not for getting grid properties")]
        public GridProperties GetGridProperties() => new()
        {
            width = m_BackGroundTileMap.size.x,
            height = m_BackGroundTileMap.size.y,
            originPoint = m_BackGroundTileMap.origin
        };
        public void SetTile(Vector2Int gridPos, DungeonRuleTile tile)
        {
            switch (mFunctionToLayerMap[tile.FunctionType])
            {
                case VisualLayer.BackGround:
                    m_BackGroundTileMap.SetTile(new Vector3Int(gridPos.x, gridPos.y, 0), tile);
                    break;
                case VisualLayer.Buildings:
                    m_BuildingsTileMap.SetTile(new Vector3Int(gridPos.x, gridPos.y, 0), tile);
                    break;
                case VisualLayer.Decorate:
                    m_DecorateTileMap.SetTile(new Vector3Int(gridPos.x, gridPos.y, 0), tile);
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
