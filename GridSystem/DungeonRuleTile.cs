using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dungeon.GridSystem
{
    [CreateAssetMenu(menuName = "Tiles/Dungeon Rule Tile")]
    public class DungeonRuleTile : RuleTile, IDungeonTile
    {
        [SerializeField] private TilePathBlockType tileType;
        [SerializeField] private TileFunctionType functionType;

        public TileFunctionType FunctionType { get => functionType; set => functionType = value; }
        public TilePathBlockType BlockType { get => tileType; set => tileType = value; }

#if UNITY_EDITOR
        public string GetPath()
        {
            string path = AssetDatabase.GetAssetPath(this);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("[DungeonTile] 无法获取资源路径!");
                return string.Empty;
            }

            int resourcesIndex = path.IndexOf("Resources/", System.StringComparison.Ordinal);
            if (resourcesIndex >= 0)
            {
                string relativePath = path.Substring(resourcesIndex + "Resources/".Length);
                return System.IO.Path.ChangeExtension(relativePath, null);
            }
            else
            {
                Debug.LogError($"[DungeonTile] 资源不在 Resources 文件夹下，路径为: {path}");
                return string.Empty;
            }
        }
#endif
    }

    [CreateAssetMenu(menuName = "Tiles/Dungeon Tile")]
    public class DungeonTile : TileBase, IDungeonTile
    {
        [SerializeField] private TilePathBlockType tileType;
        [SerializeField] private TileFunctionType functionType;

        public TileFunctionType FunctionType { get => functionType; set => functionType = value; }
        public TilePathBlockType BlockType { get => tileType; set => tileType = value; }

#if UNITY_EDITOR
        public string GetPath()
        {
            string path = AssetDatabase.GetAssetPath(this);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("[DungeonTile] 无法获取资源路径!");
                return string.Empty;
            }

            int resourcesIndex = path.IndexOf("Resources/", System.StringComparison.Ordinal);
            if (resourcesIndex >= 0)
            {
                string relativePath = path.Substring(resourcesIndex + "Resources/".Length);
                return System.IO.Path.ChangeExtension(relativePath, null);
            }
            else
            {
                Debug.LogError($"[DungeonTile] 资源不在 Resources 文件夹下，路径为: {path}");
                return string.Empty;
            }
        }
#endif
    }

    public interface IDungeonTile
    {
        public TilePathBlockType BlockType { get; set; }
        public TileFunctionType FunctionType { get; set; }
#if UNITY_EDITOR
        //TODO: 资源存储路径
        public string GetPath();
#endif
    }
}
