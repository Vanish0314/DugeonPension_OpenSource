using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct GridProperties
{
    public int width;      // 网格宽度
    public int height;     // 网格高度
    public float cellSize;     // 每个单元格的大小
    public Vector3 originPoint; // 网格原点

    public GridProperties(int width = 128, int height = 128, float size = 1.0f, Vector3 origin = default)
    {
        this.width = width;
        this.height = height;
        cellSize = size;
        originPoint = origin;
    }
}


public class VisualGrid : MonoBehaviour
{
    private void Awake() 
    {
        var bg = new GameObject("BackGround");
        var bt = new GameObject("Buildings");
        bg.transform.parent = transform;
        bt.transform.parent = transform;
        
        m_BackGroundTileMap = bg.AddComponent<Tilemap>();
        m_BackGroundTileMapRenderer = bg.AddComponent<TilemapRenderer>();

        m_BuildingsTileMap = bt.AddComponent<Tilemap>();
        m_BuildingsTileMapRenderer = bt.AddComponent<TilemapRenderer>();

        //TODO order in layer
    }

    public void OnResize(GridProperties properties)
    {
        //TODO
    }
    public Tilemap m_BackGroundTileMap;
    public TilemapRenderer m_BackGroundTileMapRenderer;
    public Tilemap m_BuildingsTileMap;
    public TilemapRenderer m_BuildingsTileMapRenderer;

}

public class LogiaclCell
{
    public GridCellReachableType type;
}


public class LogicalGrid : MonoBehaviour
{
    private GridProperties gridProperties;
    private Map2D<LogiaclCell> grid;
    private Vector3 OriginPoint => gridProperties.originPoint;
    private Vector3 offset => new Vector3(gridProperties.cellSize/2,gridProperties.cellSize/2,0);

    private void InitializeGrid()
    {
        grid.Resize(gridProperties.width,gridProperties.height);
    }
    public void OnResize(GridProperties properties)
    {
        gridProperties = properties;
        
        InitializeGrid();
    }

#if UNITY_EDITOR
    [Header("Gizmos")]
    public bool enableGizmos = false;
    public Color textColor = Color.red;
    public Vector2Int posGap = new (10,10);
    [UnityEngine.Range(1,100)] public float gridColorScale = 1;
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

        GUIStyle labelStyle = new ();
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = textColor;

        for (int x = 0; x < gridProperties.width; x++)
        {
            for (int y = 0; y < gridProperties.height; y++)
            {
                Vector3 cellWorldPos = GridToWorldPosition(x, y);

                Gizmos.color = cachedColors[x * gridProperties.height + y];
                Gizmos.DrawWireCube(cellWorldPos, new Vector3(cellSize, cellSize, 0));

                if (x % posGap.x == 0 && y % posGap.y == 0)
                {
                    Handles.Label(cellWorldPos, $"({x},{y})", labelStyle);
                }
            }
        }
    }
#endif

#region PUBLIC
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

    public GridPath FindPath_AStar(Vector2Int from, Vector2Int to)
    {
        GridPath path = new ();
        List<Vector2Int> openList = new ();
        HashSet<Vector2Int> closedList = new ();

        Dictionary<Vector2Int, int> gCosts = new ();
        Dictionary<Vector2Int, int> hCosts = new ();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new (); // 用于回溯路径

        openList.Add(from);
        gCosts[from] = 0;
        hCosts[from] = GetHeuristic(from, to);

        while (openList.Count > 0)
        {
            Vector2Int current = GetLowestFNode(openList, gCosts, hCosts);

            if (current == to)
            {
                Vector2Int pathNode = current;
                while (cameFrom.ContainsKey(pathNode))
                {
                    path.path.Push(pathNode);
                    pathNode = cameFrom[pathNode];
                }
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
                hCosts[neighbor] = GetHeuristic(neighbor, to);
            }
        }

        return path;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int node)
    {
        List<Vector2Int> neighbors = new ();
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
        return x >= 0 && x < gridProperties.width && y >= 0 && y < gridProperties.height &&
               grid.Get(x, y).type == GridCellReachableType.Reachabel;
    }

#endregion


}

public class GridSystem : MonoBehaviour
{
    

    [SerializeField] private GridProperties properties;
    [SerializeField] private VisualGrid m_VisualGrid;
    [SerializeField] private LogicalGrid m_LogicalGrid;
    private void Awake() 
    {
        m_VisualGrid = gameObject.GetOrAddComponent<VisualGrid>();
        m_LogicalGrid = gameObject.GetOrAddComponent<LogicalGrid>();
    }
    private void Start() 
    {
        var p = new GridProperties(128,128,1,default);
        Resize(p);
    }
    private void Resize(GridProperties props) 
    {
        properties = props;

        m_VisualGrid.OnResize(properties);
        m_LogicalGrid.OnResize(properties);
    }


}

public struct GridPath
{
    public Stack<Vector2Int> path;
}

public enum GridCellReachableType
{
    Reachabel,
    UnReachable
}
public enum GridCellType
{
    Gound,
    Wall,
    Building
}