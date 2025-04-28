using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Dungeon.AgentLowLevelSystem;
using Dungeon.BlackBoardSystem;
using Dungeon.Character.Hero;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using GameFramework;
using UnityEngine;

namespace Dungeon.GridSystem
{
    public partial class GridSystem : MonoBehaviour, IBlackBoardWriter
    {
        public Vector2Int GetExitGridPosition()
        {
            return gridData.properties.exitPosition;
        }
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
            GetRoomAt(GetExitGridPosition(), out var exitRoom);
            #if UNITY_EDITOR
            if (exitRoom == null) GameFrameworkLog.Error("[GridSystem] 设置的出口位置没有对应的房间");
            #endif

            var root = new GameObject("Rooms");
            root.transform.SetParent(transform);
            foreach (var room in m_DungeonRooms)
            {
                var go = new GameObject(room.name);
                go.transform.SetParent(root.transform);
                go.AddComponent<DungeonRoom>();
                go.GetComponent<DungeonRoom>().Init(room);

                if(room == exitRoom)
                {
                    go.GetComponent<DungeonRoom>().SetAsDungeonExitRoom();
                }
            }

        }

        private List<Room> m_DungeonRooms;

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

        public void SetAsDungeonExitRoom()
        {
            isDungeonExit = true;
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
            if (collision.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>() == null)
                GameFrameworkLog.Warning("[DungeonRoom] An object that is not hero is able to collide with trapTrigger,Object name: " + collision.gameObject.name);
#endif
            var hero = collision.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();
            if (hero == null)
                return;//TODO(vanish): 检测房间内怪物有多少

            if (isDungeonExit)
            {
                hero.GetComponent<HeroEntityBase>().OnArrivedAtDungeonExit();

                if (DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.IsAllHeroHappyingAtDungeonExit())
                {
                    DungeonGameEntry.DungeonGameEntry.Event.Fire(this, OnHeroTeamFinishDungeonExploreEvent.Create());
                }
            }
            else
            {
                hero.OnVisitedRoom(room);
                hero.GetExperience(DungeonGameEntry.DungeonGameEntry.DungeonResultCalculator.ExperienceGetByRoomRule.experiencePerRoom);
            }
        }
        private GridSystem.Room room;
        private bool isDungeonExit;
    }
}
