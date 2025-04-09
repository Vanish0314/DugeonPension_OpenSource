using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GameFramework;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon.GridSystem
{
    [CustomEditor(typeof(GridSystem))]
    public class GridSystemEditor : Editor
    {
        private string customMapName = "";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GridSystem gridSystem = (GridSystem)target;

            GUILayout.Space(10);
            EditorGUILayout.LabelField("地图保存设置", EditorStyles.boldLabel);

            customMapName = EditorGUILayout.TextField("保存名称", customMapName);

            if (GUILayout.Button("保存地图"))
            {
                if (string.IsNullOrEmpty(customMapName))
                {
                    Debug.LogError("请输入地图保存名称！");
                    return;
                }

                var data = SaveGridData(gridSystem);
                SaveToFile(data, customMapName);
            }

            if (GUILayout.Button("应用地图"))
            {
                ApplyGridData();
            }
        }

        private GridData SaveGridData(GridSystem gridSystem)
        {
            VisualGrid visualGrid = gridSystem.GetComponentInChildren<VisualGrid>();
            Grid unityGrid = gridSystem.GetComponent<Grid>();

            if (visualGrid == null)
            {
                GameFrameworkLog.Error("[GridSystemEditor] 找不到 VisualGrid!");
                return null;
            }

            Tilemap bg = GetTilemap(visualGrid, VisualGrid.VisualLayer.BackGround);
            Tilemap bld = GetTilemap(visualGrid, VisualGrid.VisualLayer.Buildings);
            Tilemap deco = GetTilemap(visualGrid, VisualGrid.VisualLayer.Decorate);

            if(bg == null || bld == null || deco == null)
            {
                GameFrameworkLog.Error("[GridSystemEditor] 找不到 Tilemap!\n");
            }

            BoundsInt bounds = IntersectBounds(bg.cellBounds, bld.cellBounds, deco.cellBounds);

            var data = new GridData
            {
                properties = new GridProperties
                {
                    width = bounds.size.x,
                    height = bounds.size.y,
                    originPoint = unityGrid.CellToWorld(bounds.min)
                }
            };

            SaveLayer("BackGround", bg, data);
            SaveLayer("Buildings", bld, data);
            SaveLayer("Decorate", deco, data);

            return data;
        }

        private void ApplyGridData()
        {
            GridSystem gridSystem = (GridSystem)target;

            FieldInfo field = typeof(GridSystem).GetField("gridData", BindingFlags.NonPublic | BindingFlags.Instance);
            GridData data = field?.GetValue(gridSystem) as GridData;

            if (data == null)
            {
                Debug.LogError("[GridSystemEditor] 无法获取 gridData,或数据为空!");
                return;
            }

            gridSystem.Load(data);
        }

        private void SaveLayer(string layerName, Tilemap tilemap, GridData data)
        {
            if (tilemap == null) return;

            var layerData = new GridData.TileLayerData
            {
                layerName = layerName
            };

            BoundsInt bounds = tilemap.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new(x, y, 0);
                    TileBase tileBase = tilemap.GetTile(pos);
                    if (tileBase is IDungeonTile dungeonTile)
                    {
                        var cell = new GridData.TileCellData
                        {
                            x = x,
                            y = y,
                            tilePath = dungeonTile.GetPath(),
                            blockTypeType = dungeonTile.BlockType,
                            funtionType = dungeonTile.FunctionType
                        };
                        layerData.tiles.Add(cell);
                    }
                }
            }

            data.layers.Add(layerData);
        }

        private Tilemap GetTilemap(VisualGrid grid, VisualGrid.VisualLayer layer)
        {
            var field = typeof(VisualGrid).GetField($"m_{layer}TileMap", BindingFlags.NonPublic | BindingFlags.Instance);
            return field?.GetValue(grid) as Tilemap;
        }

        private BoundsInt IntersectBounds(BoundsInt a, BoundsInt b, BoundsInt c)
        {
            int xMin = Mathf.Max(a.xMin, b.xMin, c.xMin);
            int yMin = Mathf.Max(a.yMin, b.yMin, c.yMin);
            int xMax = Mathf.Min(a.xMax, b.xMax, c.xMax);
            int yMax = Mathf.Min(a.yMax, b.yMax, c.yMax);
            return new BoundsInt(xMin, yMin, 0, xMax - xMin, yMax - yMin, 1);
        }

        private void SaveToFile(GridData data, string fileName, string path = "Assets/Save/GridData/")
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fullPath = Path.Combine(path, $"{fileName}.grid.json");

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(fullPath, json);

            Debug.Log($"地图数据已保存到: {fullPath}");
            AssetDatabase.Refresh();
        }
    }
}
