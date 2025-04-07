using System.Collections;
using System.Collections.Generic;
using Dungeon.GridSystem;
using Dungeon.Common;
using GameFramework;
using UnityEngine;
using static UnityGameFramework.Runtime.DebuggerComponent;
using DG.Tweening.Core.Easing;
using UnityEngine.Tilemaps;
using UnityGameFramework.Runtime;
using System.Threading.Tasks;





#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dungeon.GridSystem
{
    /// <summary>
    /// LogicalGrid Coord:
    /// Start from donw left corner of the grid with x = 0 and y = 0
    /// End at top right corner of the grid with x = width - 1 and y = height - 1
    /// The mid point of the grid is (CellSize/2, CellSize/2)
    /// </summary>
    public class LogicalGrid : MonoBehaviour
    {
        private GridProperties gridProperties;
        private Map2D<LogicalCell> grid;
        private Vector3 originPoint => gridProperties.originPoint;
        public Vector3 GridDownLeftOriginPoint => GridToWorldPosition(0, 0);
        private Vector3 offset => new Vector3(gridProperties.cellSize / 2, gridProperties.cellSize / 2, 0);

        private void InitializeGrid()
        {
            if (grid != null)
                return;

            grid = new Map2D<LogicalCell>(gridProperties.width, gridProperties.height);
            grid.FillAll(new LogicalCell());
        }
        public void OnResize(GridProperties properties)
        {
            gridProperties = properties;

            InitializeGrid();
        }
        public void Init(GridProperties properties)
        {
            gridProperties = properties;

            OnResize(properties);
        }

        public void Init(VisualGrid visualGrid)
        {
            var tilemap = visualGrid.GetBackGroundTileMap();
            var gridDesc = new GridProperties();

            gridDesc.width = tilemap.size.x;
            gridDesc.height = tilemap.size.y;
            gridDesc.cellSize = tilemap.cellSize.x;
            gridDesc.originPoint = tilemap.origin;
            Init(gridDesc);


            var bounds = tilemap.cellBounds;
            
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int tilePosition = new (x, y, 0);
                    TileBase tile = tilemap.GetTile(tilePosition);

                    // Trans world position to grid position
                    var gridX = x - bounds.xMin;
                    var gridY = y - bounds.yMin;

                    if (tile is IDungeonTile dungeonTile)
                    {
                        if (dungeonTile.TileType == TileType.Wall)
                            grid.Set(gridX, gridY, new LogicalCell(GridCellReachableType.UnReachable));
                        else
                            grid.Set(gridX, gridY, new LogicalCell(GridCellReachableType.Reachabel));
                    }

                }
            }
        }

#if UNITY_EDITOR
        [Header("Gizmos")]
        public bool enableGizmos = true;
        public Color textColor = Color.red;
        public Vector2Int posGap = new(10, 10);
        [Range(1, 100)] public float gridColorScale = 1;
        private void OnDrawGizmos()
        {
            if (!enableGizmos) return;

            float cellSize = gridProperties.cellSize;
            Color[] cachedColors = new Color[gridProperties.width * gridProperties.height];

            for (int x = 0; x < gridProperties.width; x++)
            {
                for (int y = 0; y < gridProperties.height; y++)
                {
                    cachedColors[x * gridProperties.height + y] = new Color(x / (float)gridProperties.width, y / (float)gridProperties.height, 0.0f) * gridColorScale;
                }
            }

            GUIStyle labelStyle = new();
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = textColor;

            for (int x = 0; x < gridProperties.width; x++)
            {
                for (int y = 0; y < gridProperties.height; y++)
                {
                    Vector3 cellWorldPos = GridToWorldPosition(x, y);

                    var cell = grid.Get(x, y);
                    if (cell.type == GridCellReachableType.UnReachable)
                    {
                        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                        Gizmos.DrawCube(cellWorldPos, new Vector3(cellSize, cellSize, 0));
                    }
                    else
                    {
                        Gizmos.color = cachedColors[x * gridProperties.height + y];
                        Gizmos.DrawWireCube(cellWorldPos, new Vector3(cellSize, cellSize, 0));
                    }

                    if (x % posGap.x == 0 && y % posGap.y == 0)
                    {
                        Handles.Label(cellWorldPos, $"({x},{y})", labelStyle);
                    }
                    if (x == gridProperties.width - 1 && y == gridProperties.height - 1)
                    {
                        Handles.Label(cellWorldPos, $"({x},{y})", labelStyle);
                    }
                }
            }
        }

        [DungeonGridWindow("SayHello")]
        private static void SayHello()
        {
            GameFrameworkLog.Info("Hello, World!");
        }
        [DungeonGridWindow("FindPath")]
        private static void FindPath()
        {
            Log.Info("[LogicalGrid] FindPath Start. From {0}, {1} to {2}, {3}", 1, 1, 20, 20);

            var go = GameObject.Find("GridSystem");
            var path = go.GetComponent<LogicalGrid>().FindPath_AStar(new Vector2Int(5, 5), DungeonGameEntry.DungeonGameEntry.GridSystem.WorldToGridPosition(Vector3.zero));
            foreach (var node in path.path)
            {
                GameFrameworkLog.Info(node.ToString());
                
                TileDesc tileDesc = new(){type = TileType.Debug};
                var gridProperties = DungeonGameEntry.DungeonGameEntry.GridSystem.GetGridProperties();
                var a =new Vector2Int((int)gridProperties.originPoint.x, (int)gridProperties.originPoint.y);
                DungeonGameEntry.DungeonGameEntry.GridSystem.SetTile(new Vector2Int(node.x + a.x, node.y + a.y),tileDesc);
            }

            if(path.path.Count == 0)
                Log.Info("[LogicalGrid] No path found. From {0}, {1} to {2}, {3}", 1, 1, 20, 20);
        }

#endif

        #region PUBLIC
        public void SetTile(Vector2Int gridPos, GridCellReachableType type)
        {
            grid.Set(gridPos.x, gridPos.y, new LogicalCell { type = type });
        }
        public bool IsUnReachable(Vector2Int gridPos)
        {
            return grid.Get(gridPos.x, gridPos.y).type == GridCellReachableType.UnReachable;
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            worldPosition -= offset;
            int x = Mathf.FloorToInt((worldPosition.x - gridProperties.originPoint.x) / gridProperties.cellSize);
            int y = Mathf.FloorToInt((worldPosition.y - gridProperties.originPoint.y) / gridProperties.cellSize);
            return new Vector2Int(x, y);
        }

        public Vector3 GridToWorldPosition(int x, int y)
        {
            return new Vector3(x * gridProperties.cellSize + gridProperties.originPoint.x, y * gridProperties.cellSize + gridProperties.originPoint.y, 0) + offset;
        }

        /// <summary>
        /// return in grid coordinate
        /// </summary>
        /// <param name="fromPosInGridCoord"></param>
        /// <param name="toPosInGridCoord"></param>
        /// <returns></returns>
        public GridPath FindPath_AStar(Vector2Int fromPosInGridCoord, Vector2Int toPosInGridCoord)
        {
            GridPath path = new();
            path.path = new Stack<Vector2Int>();
            List<Vector2Int> openList = new();
            HashSet<Vector2Int> closedList = new();

            Dictionary<Vector2Int, int> gCosts = new();
            Dictionary<Vector2Int, int> hCosts = new();
            Dictionary<Vector2Int, Vector2Int> cameFrom = new();

            openList.Add(fromPosInGridCoord);
            gCosts[fromPosInGridCoord] = 0;
            hCosts[fromPosInGridCoord] = GetHeuristic(fromPosInGridCoord, toPosInGridCoord);

            while (openList.Count > 0)
            {
                Vector2Int current = GetLowestFNode(openList, gCosts, hCosts);

                if (current == toPosInGridCoord)
                {
                    Vector2Int pathNode = current;
                    while (cameFrom.TryGetValue(pathNode, out var parent))
                    {
                        path.path.Push(pathNode);
                        pathNode = parent;
                    }
                    path.path.Push(fromPosInGridCoord);
                    
                    return path;
                }

                openList.Remove(current);
                closedList.Add(current);

                List<Vector2Int> neighbors = GetNeighbors(current);
                foreach (var neighbor in neighbors)
                {
                    if (closedList.Contains(neighbor) || !IsValidCoordinate(neighbor.x, neighbor.y))
                        continue;

                    int tentativeG = gCosts[current] + 1;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                    else if (tentativeG >= gCosts[neighbor])
                    {
                        continue;
                    }

                    cameFrom[neighbor] = current;
                    gCosts[neighbor] = tentativeG;
                    hCosts[neighbor] = GetHeuristic(neighbor, toPosInGridCoord);
                }
            }

            Log.Info("[LogicalGrid] No path found from {0}, {1} to {2}, {3}", fromPosInGridCoord.x, fromPosInGridCoord.y, toPosInGridCoord.x, toPosInGridCoord.y);
            return path;
        }

        private List<Vector2Int> GetNeighbors(Vector2Int node)
        {
            List<Vector2Int> neighbors = new();
            Vector2Int[] directions = new Vector2Int[]
            {
            new (0, 1), new (1, 0), new (0, -1), new (-1, 0)
            };

            foreach (var direction in directions)
            {
                int newX = node.x + direction.x;
                int newY = node.y + direction.y;
                if (IsValidCoordinate(newX, newY))
                {
                    neighbors.Add(new Vector2Int(newX, newY));
                }
            }

            return neighbors;
        }

        private int GetHeuristic(Vector2Int a, Vector2Int b)
        {
            // 使用曼哈顿距离作为启发式函数
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private Vector2Int GetLowestFNode(List<Vector2Int> openList, Dictionary<Vector2Int, int> gCosts, Dictionary<Vector2Int, int> hCosts)
        {
            Vector2Int lowestFNode = openList[0];
            int lowestF = gCosts[lowestFNode] + hCosts[lowestFNode];

            foreach (var node in openList)
            {
                int fCost = gCosts[node] + hCosts[node];
                if (fCost < lowestF)
                {
                    lowestF = fCost;
                    lowestFNode = node;
                }
            }

            return lowestFNode;
        }

        public bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && 
                   x < gridProperties.width && 
                   y >= 0 && 
                   y < gridProperties.height &&
                   grid.Get(x, y).type == GridCellReachableType.Reachabel;
        }

        #endregion


    }
}
