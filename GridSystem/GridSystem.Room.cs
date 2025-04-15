using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Dungeon.AgentLowLevelSystem;
using Dungeon.BlackBoardSystem;
using GameFramework;
using UnityEngine;

namespace Dungeon.GridSystem
{
    public partial class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        public void GetRoomAt(Vector2Int gridPos, out Room room)
        {
            m_LogicalGrid.GetRoomAt(gridPos, out room);
        }
        public void GetRoomAt(Vector3 worldPos, out Room room)
        {
            Vector2Int gridPos = m_LogicalGrid.WorldToGridPosition(worldPos);
            m_LogicalGrid.GetRoomAt(gridPos, out room);
        }
        private void InitRooms()
        {
            var roomGo = GameObject.Find("Rooms");
            if (roomGo != null) DestroyImmediate(roomGo);

            CalculateRoom();
        }
        private void CalculateRoom()
        {
            m_DungeonRooms = m_LogicalGrid.CalculateRoom();
            BuildRooms();
        }
        private void BuildRooms()
        {
            var root = new GameObject("Rooms");
            root.transform.SetParent(transform);
            foreach (var room in m_DungeonRooms)
            {
                var go = new GameObject(room.name);
                go.transform.SetParent(root.transform);
                go.AddComponent<DungeonRoom>();
                go.GetComponent<DungeonRoom>().Init(room);
            }
        }

        private List<Room> m_DungeonRooms;
        [SerializeField] private Vector2Int DungeonExitPosition;

        public class Room
        {
            public string name;
            public int hashID;
            public Vector2Int centerPos;
            public Room[] neighbours;
            public List<Vector2Int> cells;

            public Room(Vector2Int centerPos, string name, Room[] neighbours, List<Vector2Int> cells)
            {
                this.centerPos = centerPos;
                this.name = name;
                hashID = name.ComputeFNV1aHash();
                this.neighbours = neighbours;
                this.cells = cells;
            }
        }
    }

    [RequireComponent(typeof(PolygonCollider2D), typeof(Rigidbody2D))]
    public class DungeonRoom : MonoBehaviour
    {
        public void Init(GridSystem.Room room)
        {
            this.room = room;

            InitTransform(room);
            InitCollider(room);
            InitRigidbody();
        }

        private void InitTransform(GridSystem.Room room)
        {
            transform.position = DungeonGameEntry.DungeonGameEntry.GridSystem.GridToWorldPosition(room.centerPos);
            transform.name = room.name;
        }

        private void InitCollider(GridSystem.Room room)
        {
            var collider = GetComponent<PolygonCollider2D>();
            collider.isTrigger = true;

            Dictionary<(Vector2Int, Vector2Int), int> edgeDict = new();

            // 1. Find all edges in the room.
            foreach (var cell in room.cells)
            {
                Vector2Int[] corners = new Vector2Int[]
                {
                    cell,
                    cell + Vector2Int.right,
                    cell + Vector2Int.right + Vector2Int.up,
                    cell + Vector2Int.up
                };

                (Vector2Int, Vector2Int)[] edges = new (Vector2Int, Vector2Int)[]
                {
                    (corners[0], corners[1]),
                    (corners[1], corners[2]),
                    (corners[2], corners[3]),
                    (corners[3], corners[0])
                };

                foreach (var edge in edges)
                {
                    Vector2Int item1 = edge.Item1;
                    Vector2Int item2 = edge.Item2;

                    var normalized = (item1.x < item2.x || (item1.x == item2.x && item1.y < item2.y)) ? edge : (item2, item1);

                    if (edgeDict.ContainsKey(normalized))
                        edgeDict[normalized]++;
                    else
                        edgeDict[normalized] = 1;
                }
            }

            var outlineEdges = edgeDict.Where(kv => kv.Value == 1).Select(kv => kv.Key).ToList();

            if (outlineEdges.Count == 0)
            {
                Debug.LogWarning("No outline edges found.");
                return;
            }

            // 2. Create the outline path.
            List<Vector2> outlinePath = new();
            var currentEdge = outlineEdges[0];
            outlinePath.Add(DungeonGameEntry.DungeonGameEntry.GridSystem.GridToWorldPosition(currentEdge.Item1));
            outlinePath.Add(DungeonGameEntry.DungeonGameEntry.GridSystem.GridToWorldPosition(currentEdge.Item2));

            outlineEdges.RemoveAt(0);

            while (outlineEdges.Count > 0)
            {
                var lastPoint = currentEdge.Item2;

                var nextIndex = outlineEdges.FindIndex(e => e.Item1 == lastPoint || e.Item2 == lastPoint);
                if (nextIndex == -1) break;

                var nextEdge = outlineEdges[nextIndex];
                var nextPoint = nextEdge.Item1 == lastPoint ? nextEdge.Item2 : nextEdge.Item1;

                outlinePath.Add(DungeonGameEntry.DungeonGameEntry.GridSystem.GridToWorldPosition(nextPoint));

                currentEdge = (lastPoint, nextPoint);
                outlineEdges.RemoveAt(nextIndex);
            }

            collider.pathCount = 1;
            collider.SetPath(0, outlinePath.ToArray());

            // TODO(vanish): calculation above is wrong ,but no time to fix it now.
            var x = -Mathf.Ceil(transform.position.x);
            var y = -Mathf.Ceil(transform.position.y);
            collider.offset = new Vector2(x, y);
        }
        private void InitRigidbody()
        {
            gameObject.layer = LayerMask.NameToLayer("TrapTrigger");
            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
#if UNITY_EDITOR
            if(collision.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>()== null)
                GameFrameworkLog.Error("[DungeonRoom] An object that is not hero is able to collide with trapTrigger");
#endif

            var agent = collision.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();
            if (agent == null) return;

            agent.OnVisitedRoom(room);
        }
        private GridSystem.Room room;
    }
}
