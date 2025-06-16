using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using static UnityGameFramework.Runtime.DebuggerComponent;
using Dungeon.DungeonEntity;
using static Dungeon.GridSystem.GridSystem;
using System.Linq;
using System;

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
        public void FixedUpdate()
        {
            UpdateDynamicalMap();

            if (Time.frameCount % 10 == 0)
            {
                try
                {
                    ReleaseMap();
                }
                catch
                {
                }
            }
        }

        private void ReleaseMap()
        {
            foreach (var (key, trap) in trapMap)
            {
                if (trap.isInPool)
                {
                    trapMap.Remove(key);
                }
            }

            foreach (var (key, interactiveObject) in interactMap)
            {
                if (interactiveObject.isInPool)
                {
                    interactMap.Remove(key);
                }
            }

            foreach (var (key, monster) in monsterMap)
            {
                if (monster.isInPool)
                {
                    monsterMap.Remove(key);
                }
            }
        }

        private List<Vector2Int> lastFrameDynamicWallPositions = new();

        private void UpdateDynamicalMap()
        {
            foreach (var pos in lastFrameDynamicWallPositions)
            {
                if (wallMap.Get(pos.x, pos.y).type == TilePathBlockType.DynamicObstacle)
                {
                    wallMap.Set(pos.x, pos.y, new LogicalCell(TilePathBlockType.Ground));
                }
            }

            lastFrameDynamicWallPositions.Clear();

            foreach (var (key, monster) in monsterMap)
            {
                var pos = WorldToGridPosition(monster.transform.position);
                if (wallMap.Get(pos.x, pos.y).type == TilePathBlockType.Ground)
                {
                    wallMap.Set(pos.x, pos.y, new LogicalCell(TilePathBlockType.DynamicObstacle));
                    lastFrameDynamicWallPositions.Add(pos);
                }
            }

            foreach (var entity in DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.currentBehavouringHeroTeam)
            {
                var pos = WorldToGridPosition(entity.transform.position);
                if (wallMap.Get(pos.x, pos.y).type == TilePathBlockType.Ground)
                {
                    wallMap.Set(pos.x, pos.y, new LogicalCell(TilePathBlockType.DynamicObstacle));
                    lastFrameDynamicWallPositions.Add(pos);
                }
            }
        }

        public void Init(GridData data)
        {
            gridProperties = data.properties;
            OnResize(gridProperties);

            foreach (var layer in data.layers)
            {
                foreach (var tileCell in layer.tiles)
                {
                    var gridPos = WorldToGridPosition(new Vector3(tileCell.x, tileCell.y, 0));

                    if (IsUnReachable(gridPos))
                        continue;

                    var logicalCell = new LogicalCell(tileCell.blockTypeType);
                    wallMap.Set(gridPos, logicalCell);
                }
            }
        }

        public void Clear()
        {
            wallMap.Clear();
            gridProperties = new GridProperties();
        }

        public void SetTile(Vector2Int gridPos, IDungeonTile tile)
        {
            wallMap.Set(gridPos.x, gridPos.y, new LogicalCell(tile.BlockType));
        }

        public void AddTrap(Vector2Int gridPos, DungeonTrapBase trap)
        {
            var gridPoses = new List<Vector2Int>();
            for (int x = 0; x < trap.size.x; x++)
            {
                for (int y = 0; y < trap.size.y; y++)
                {
                    gridPoses.Add(new Vector2Int(gridPos.x + x, gridPos.y + y));
                }
            }

            foreach (var pos in gridPoses)
            {
                if (trapMap.ContainsKey(pos))
                    return;
            }

            foreach (var pos in gridPoses)
            {
                if (trap.isCollider)
                    wallMap.Set(pos.x, pos.y, new LogicalCell(TilePathBlockType.Wall));

                trapMap.Add(pos, trap);
            }
        }

        public void AddInteractiveObject(Vector2Int gridPos, DungeonInteractiveObjectBase interactiveObject)
        {
            if (interactMap.ContainsKey(gridPos))
                return;

            wallMap.Set(gridPos.x, gridPos.y, new LogicalCell(TilePathBlockType.Wall));

            interactMap.Add(gridPos, interactiveObject);
        }

        public void AddMonster(Vector2Int gridPos, DungeonMonsterBase monster)
        {
            if (monsterMap.ContainsKey(gridPos))
                return;

            monsterMap.Add(gridPos, monster);
        }

        public void RemoveTrap(Vector2Int gridPos)
        {
            var trap = GetTrap(gridPos);
            if (trap == null)
            {
                GameFrameworkLog.Error($"[LogicalGrid] 网格中没有注册陷阱,但是被调用了移除陷阱的指令.\n陷阱位置:({gridPos.x},{gridPos.y})");
            }

            Vector2Int startPos = gridPos - trap.size + Vector2Int.one;
            var gridPoses = new List<Vector2Int>();

            for (int x = 0; x < trap.size.x; x++)
            {
                for (int y = 0; y < trap.size.y; y++)
                {
                    gridPoses.Add(new Vector2Int(startPos.x + x, startPos.y + y));
                }
            }

            foreach (var pos in gridPoses)
            {
                if (trapMap.ContainsKey(pos))
                {
                    if (trap.isCollider)
                        wallMap.Set(pos.x, pos.y, new LogicalCell(TilePathBlockType.Ground));

                    trapMap.Remove(pos);
                }
            }
        }

        public void RemoveInteractiveObject(Vector2Int gridPos)
        {
            if (interactMap.ContainsKey(gridPos))
            {
                interactMap.Remove(gridPos);
            }
        }

        public void RemoveMonster(Vector2Int gridPos)
        {
            if (monsterMap.ContainsKey(gridPos))
            {
                monsterMap.Remove(gridPos);
            }
        }

        public bool IsUnReachable(Vector2Int gridPos)
        {
            return wallMap.Get(gridPos.x, gridPos.y).type == TilePathBlockType.Wall;
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition.x - gridProperties.originPoint.x) / GridProperties.cellSize);
            int y = Mathf.FloorToInt((worldPosition.y - gridProperties.originPoint.y) / GridProperties.cellSize);
            return new Vector2Int(x, y);
        }

        public Vector3 GridToWorldPosition(int x, int y)
        {
            return new Vector3(x * GridProperties.cellSize + gridProperties.originPoint.x,
                y * GridProperties.cellSize + gridProperties.originPoint.y, 0) + offset;
        }

        public Vector3 SnapToGridCorner(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / GridProperties.cellSize) * (int)GridProperties.cellSize;
            int y = Mathf.FloorToInt(worldPosition.y / GridProperties.cellSize) * (int)GridProperties.cellSize;
            return new Vector3(x, y, 0);
        }

        public Vector3 SnapToGridCenter(Vector3 worldPosition)
        {
            return SnapToGridCorner(worldPosition) +
                   new Vector3(GridProperties.cellSize / 2, GridProperties.cellSize / 2, 0);
        }

        public int GetGridDistance(Vector2Int gridPosition1, Vector2Int gridPosition2)
        {
            GridPath path = FindPath_AStar(gridPosition1, gridPosition2);

            if (path.path == null)
                return -1;
            return path.path.Count;
        }

        public int GetGridDistance(Vector3 worldPosition1, Vector3 worldPosition2)
        {
            Vector2Int gridPosition1 = WorldToGridPosition(worldPosition1);
            Vector2Int gridPosition2 = WorldToGridPosition(worldPosition2);
            return GetGridDistance(gridPosition1, gridPosition2);
        }

        public System.Object GetStaticEntity(Vector2Int gridPos)
        {
            if (interactMap.ContainsKey(gridPos))
            {
                return interactMap[gridPos];
            }
            else if (trapMap.ContainsKey(gridPos))
            {
                return trapMap[gridPos];
            }
            else if (monsterMap.ContainsKey(gridPos))
            {
                return monsterMap[gridPos];
            }

            return null;
        }

        public T GetStaticEntity<T>(Vector2Int gridPos) where T : DungeonEntity.DungeonEntity
        {
            if (interactMap.ContainsKey(gridPos))
            {
                return interactMap[gridPos] as T;
            }
            else if (trapMap.ContainsKey(gridPos))
            {
                return trapMap[gridPos] as T;
            }

            return null;
        }

        public DungeonTrapBase GetTrap(Vector2Int gridPos)
        {
            if (trapMap.ContainsKey(gridPos))
            {
                return trapMap[gridPos];
            }

            return null;
        }

        public DungeonInteractiveObjectBase GetInteractiveObject(Vector2Int gridPos)
        {
            if (interactMap.ContainsKey(gridPos))
            {
                return interactMap[gridPos];
            }

            return null;
        }

        public List<Room> CalculateRoom()
        {
            if (wallMap == null) return null;

            int width = wallMap.Width;
            int height = wallMap.Height;

            bool[,] visited = new bool[width, height];
            Dictionary<Vector2Int, int> positionToRoomId = new();
            List<List<Vector2Int>> roomCells = new();

            int[,] directions = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

            // Step 1: Flood fill to find all rooms
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (visited[x, y]) continue;

                    LogicalCell cell = wallMap.Get(x, y);
                    if (cell.type != TilePathBlockType.Ground) continue;

                    // New room found
                    List<Vector2Int> currentRoomCells = new();
                    Queue<Vector2Int> queue = new();
                    queue.Enqueue(new Vector2Int(x, y));
                    visited[x, y] = true;

                    while (queue.Count > 0)
                    {
                        Vector2Int pos = queue.Dequeue();
                        currentRoomCells.Add(pos);
                        positionToRoomId[pos] = roomCells.Count;

                        for (int i = 0; i < 4; i++)
                        {
                            int nx = pos.x + directions[i, 0];
                            int ny = pos.y + directions[i, 1];

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height && !visited[nx, ny])
                            {
                                LogicalCell neighbor = wallMap.Get(nx, ny);
                                if (neighbor.type == TilePathBlockType.Ground)
                                {
                                    visited[nx, ny] = true;
                                    queue.Enqueue(new Vector2Int(nx, ny));
                                }
                            }
                        }
                    }

                    roomCells.Add(currentRoomCells);
                }
            }

            // Step 2: Build Room objects (without neighbours yet)
            positionToRoom.Clear();

            List<Room> rooms = new();
            for (int i = 0; i < roomCells.Count; i++)
            {
                List<Vector2Int> cells = roomCells[i];
                Vector2 avg = Vector2.zero;
                foreach (var pos in cells)
                    avg += new Vector2(pos.x, pos.y);
                avg /= cells.Count;

                Vector2Int center = new(Mathf.RoundToInt(avg.x), Mathf.RoundToInt(avg.y));

                Room newRoom = new(center, $"Room_{i}_{UnityEngine.Random.Range(0, 1000000)}", new Room[0], cells);
                rooms.Add(newRoom);

                foreach (var pos in cells)
                {
                    positionToRoom.Add(pos, newRoom);
                }
            }

            // Step 3: Determine neighbours via corridor
            Dictionary<int, HashSet<int>> roomNeighbours = new();
            HashSet<MyVector2Int>
                visitedCorridor = new(); // Don't know why Unity's Vector2Int doesn't work with HashSet here;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    LogicalCell cell = wallMap.Get(x, y);
                    if (cell.type != TilePathBlockType.corridor)
                        continue;
                    if (visitedCorridor.Contains(new MyVector2Int(x, y)))
                        continue;

                    HashSet<int> adjacentRooms = new();
                    Queue<Vector2Int> corridorQueue = new();
                    corridorQueue.Enqueue(new Vector2Int(x, y));

                    // BFS to process the whole corridor
                    while (corridorQueue.Count > 0)
                    {
                        Vector2Int pos = corridorQueue.Dequeue();

                        for (int i = 0; i < 4; i++)
                        {
                            int nx = pos.x + directions[i, 0];
                            int ny = pos.y + directions[i, 1];

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                if (visitedCorridor.Contains(new MyVector2Int(nx, ny)))
                                    continue;

                                Vector2Int neighborPos = new(nx, ny);
                                if (wallMap.Get(nx, ny).type == TilePathBlockType.corridor)
                                {
                                    corridorQueue.Enqueue(neighborPos);
                                }
                                else if (wallMap.Get(nx, ny).type == TilePathBlockType.Ground)
                                {
                                    if (!positionToRoomId.ContainsKey(neighborPos))
                                        continue;

                                    int roomId = positionToRoomId[neighborPos];
                                    adjacentRooms.Add(roomId);
                                }

                                visitedCorridor.Add(new MyVector2Int(nx, ny));
                            }
                        }
                    }

                    // If two different rooms are adjacent via this corridor
                    if (adjacentRooms.Count >= 2)
                    {
                        var ids = adjacentRooms.ToList();
                        for (int i = 0; i < ids.Count; i++)
                        {
                            for (int j = i + 1; j < ids.Count; j++)
                            {
                                if (!roomNeighbours.ContainsKey(ids[i]))
                                    roomNeighbours[ids[i]] = new HashSet<int>();
                                if (!roomNeighbours.ContainsKey(ids[j]))
                                    roomNeighbours[ids[j]] = new HashSet<int>();

                                roomNeighbours[ids[i]].Add(ids[j]);
                                roomNeighbours[ids[j]].Add(ids[i]);
                            }
                        }
                    }
                }
            }

            // Step 4: Assign neighbours to Room
            for (int i = 0; i < rooms.Count; i++)
            {
                if (roomNeighbours.TryGetValue(i, out var neighbours))
                {
                    rooms[i].neighbours = neighbours.Select(id => rooms[id]).ToArray();
                }
                else
                {
                    rooms[i].neighbours = Array.Empty<Room>();
                }
            }

            return rooms;
        }

        public void GetRoomAt(Vector2Int gridPos, out Room room)
        {
            room = null;

            if (!wallMap.IsValidCoordinate(gridPos.x, gridPos.y))
                return;

            LogicalCell cell = wallMap.Get(gridPos.x, gridPos.y);
            if (cell.type != TilePathBlockType.Ground)
                return;

            positionToRoom.TryGetValue(gridPos, out room);
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

                    GameFrameworkLog.Info(
                        $"[LogicalGrid] Path found from {fromPosInGridCoord.x}, {fromPosInGridCoord.y} to {toPosInGridCoord.x}, {toPosInGridCoord.y}");

                    return path;
                }

                openList.Remove(current);
                closedList.Add(current);

                List<Vector2Int> neighbors = GetNeighbors(current);
                foreach (var neighbor in neighbors)
                {
                    if (closedList.Contains(neighbor) || !IsCellReachable(neighbor.x, neighbor.y))
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

            GameFrameworkLog.Info("[LogicalGrid] No path found from {0}, {1} to {2}, {3}", fromPosInGridCoord.x,
                fromPosInGridCoord.y, toPosInGridCoord.x, toPosInGridCoord.y);
            return path;
        }

        /// <summary>
        /// 会忽略起点和终点是动态障碍
        /// </summary>
        /// <param name="fromPosInGridCoord"></param>
        /// <param name="toPosInGridCoord"></param>
        /// <returns></returns>
        public GridPath FindPath_AStar_IgnoreFromToDynamicObstacle(Vector2Int fromPosInGridCoord,
            Vector2Int toPosInGridCoord)
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

                    GameFrameworkLog.Info(
                        $"[LogicalGrid] Path found from {fromPosInGridCoord.x}, {fromPosInGridCoord.y} to {toPosInGridCoord.x}, {toPosInGridCoord.y}");

                    return path;
                }

                openList.Remove(current);
                closedList.Add(current);

                List<Vector2Int> neighbors =
                    GetNeighbors_IgnoreFromToDynamicObstacle(current, fromPosInGridCoord, toPosInGridCoord);
                foreach (var neighbor in neighbors)
                {
                    if (closedList.Contains(neighbor) ||
                        wallMap.Get(neighbor.x, neighbor.y).type == TilePathBlockType.Wall)
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

            GameFrameworkLog.Info("[LogicalGrid] No path found from {0}, {1} to {2}, {3}", fromPosInGridCoord.x,
                fromPosInGridCoord.y, toPosInGridCoord.x, toPosInGridCoord.y);
            return path;
        }

        private void OnResize(GridProperties properties)
        {
            wallMap = new Map2D<LogicalCell>(properties.width, properties.height);
            wallMap.FillAll(new LogicalCell());
        }

        private List<Vector2Int> GetNeighbors(Vector2Int node)
        {
            List<Vector2Int> neighbors = new();
            Vector2Int[] directions = new Vector2Int[]
            {
                new(0, 1), new(1, 0), new(0, -1), new(-1, 0)
            };

            foreach (var direction in directions)
            {
                int newX = node.x + direction.x;
                int newY = node.y + direction.y;
                if (IsCellReachable(newX, newY))
                {
                    neighbors.Add(new Vector2Int(newX, newY));
                }
            }

            return neighbors;
        }

        private List<Vector2Int> GetNeighbors_IgnoreFromToDynamicObstacle(Vector2Int node, Vector2Int from,
            Vector2Int to)
        {
            List<Vector2Int> neighbors = new();
            Vector2Int[] directions = new Vector2Int[]
            {
                new(0, 1), new(1, 0), new(0, -1), new(-1, 0)
            };

            foreach (var direction in directions)
            {
                int newX = node.x + direction.x;
                int newY = node.y + direction.y;

                if (newX == to.x && newY == to.y)
                {
                    if (wallMap.Get(newX, newY).type == TilePathBlockType.DynamicObstacle)
                    {
                        neighbors.Add(new Vector2Int(newX, newY));
                        continue;
                    }
                }

                if (IsCellReachable(newX, newY))
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

        private Vector2Int GetLowestFNode(List<Vector2Int> openList, Dictionary<Vector2Int, int> gCosts,
            Dictionary<Vector2Int, int> hCosts)
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

        private bool IsCellReachable(int x, int y)
        {
            return x >= 0 &&
                   x < gridProperties.width &&
                   y >= 0 &&
                   y < gridProperties.height &&
                   wallMap.Get(x, y).type != TilePathBlockType.Wall &&
                   wallMap.Get(x, y).type != TilePathBlockType.DynamicObstacle;
        }

        public bool IsValidGridPosition(Vector2Int gridPos)
        {
            var x = gridPos.x;
            var y = gridPos.y;
            return x >= 0 &&
                   x < gridProperties.width &&
                   y >= 0 &&
                   y < gridProperties.height;
        }

        [HideInInspector] public Dictionary<Vector2Int, DungeonInteractiveObjectBase> interactMap = new();
        [HideInInspector] public Dictionary<Vector2Int, DungeonTrapBase> trapMap = new();
        [HideInInspector] public Dictionary<Vector2Int, DungeonMonsterBase> monsterMap = new();
        private GridProperties gridProperties;
        private Map2D<LogicalCell> wallMap;
        private Dictionary<Vector2Int, Room> positionToRoom = new();

        private Vector3 originPoint => gridProperties.originPoint;
        public Vector3 GridDownLeftOriginPoint => GridToWorldPosition(0, 0);
        private Vector3 offset => new(GridProperties.cellSize / 2, GridProperties.cellSize / 2, 0);

#if UNITY_EDITOR
        [Header("Gizmos")] public bool enableGizmos = true;
        public Color textColor = Color.red;
        public Vector2Int posGap = new(10, 10);
        [UnityEngine.Range(1, 100)] public float gridColorScale = 1;

        private void OnDrawGizmos()
        {
            if (!enableGizmos) return;

            float cellSize = GridProperties.cellSize;
            Color[] cachedColors = new Color[gridProperties.width * gridProperties.height];

            for (int x = 0; x < gridProperties.width; x++)
            {
                for (int y = 0; y < gridProperties.height; y++)
                {
                    cachedColors[x * gridProperties.height + y] =
                        new Color(x / (float)gridProperties.width, y / (float)gridProperties.height, 0.0f) *
                        gridColorScale;
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

                    var cell = wallMap.Get(x, y);
                    if (cell.type == TilePathBlockType.Wall)
                    {
                        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                        Gizmos.DrawCube(cellWorldPos, new Vector3(cellSize, cellSize, 0));
                    }
                    else if (cell.type == TilePathBlockType.DynamicObstacle)
                    {
                        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
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
                        Handles.Label(cellWorldPos - new Vector3(0, 1, 0), $"({cellWorldPos.x},{cellWorldPos.y})",
                            labelStyle);
                    }

                    if (x == gridProperties.width - 1 && y == gridProperties.height - 1)
                    {
                        Handles.Label(cellWorldPos, $"({x},{y})", labelStyle);
                        Handles.Label(cellWorldPos + new Vector3(0, 1, 0), $"({cellWorldPos.x},{cellWorldPos.y})",
                            labelStyle);
                    }
                }
            }

            if (positionToRoom != null && positionToRoom.Count > 0)
            {
                Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f); // 半透明蓝色

                foreach (var kvp in positionToRoom)
                {
                    Vector3 worldPos = GridToWorldPosition(kvp.Key.x, kvp.Key.y);
                    Gizmos.DrawCube(worldPos, new Vector3(cellSize, cellSize, 0));
                }

                HashSet<Room> drawnRooms = new();
                foreach (var kvp in positionToRoom)
                {
                    Room room = kvp.Value;
                    if (room == null || drawnRooms.Contains(room)) continue;

                    drawnRooms.Add(room);
                    Vector3 labelPos = GridToWorldPosition(room.centerPos.x, room.centerPos.y);
                    labelStyle.normal.textColor = new Color(1f, 0.8f, 0.4f);
                    Handles.Label(labelPos + new Vector3(0, 0.3f, 0), room.name, labelStyle);
                }
            }
            else
            {
                CalculateRoom();
            }
        }

        [DungeonGridWindow("SayHello")]
        private static void SayHello()
        {
            GameFrameworkLog.Info("Hello, World!");
        }
#endif
        private struct MyVector2Int : IEquatable<MyVector2Int>
        {
            public int x, y;

            public MyVector2Int(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public MyVector2Int(Vector2Int v)
            {
                x = v.x;
                y = v.y;
            }

            public override bool Equals(object obj)
            {
                return obj is MyVector2Int other && Equals(other);
            }

            public bool Equals(MyVector2Int other)
            {
                return x == other.x && y == other.y;
            }

            public override int GetHashCode()
            {
                int hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                return hashCode;
            }

            public static bool operator ==(MyVector2Int left, MyVector2Int right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(MyVector2Int left, MyVector2Int right)
            {
                return !(left == right);
            }
        }
    }
}