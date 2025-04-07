using System.Collections;
using System.Collections.Generic;
using System.Text;
using Codice.Client.Common;
using Dungeon.AgentLowLevelSystem;
using Dungeon.DungeonEntity;
using Dungeon.Vision2D;
using GameFramework;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

#endif
using UnityEngine;

namespace Dungeon.DungeonEntity.Trap
{
    [RequireComponent(typeof(BoxCollider2D),typeof(Rigidbody2D))]
    public class DungeonTrapBuildingCollier : MonoBehaviour
    {
        private DungeonTrapBase trap;
        private BoxCollider2D box;

        public void Init(Vector2Int size, DungeonTrapBase trapBase, bool isTrigger)
        {
            box = GetComponent<BoxCollider2D>();
            box.size = size;
            box.offset = (Vector2)size / 2;
            box.isTrigger = isTrigger;
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            var low = collision.gameObject.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();
#if UNITY_EDITOR
            if (low == null)
            {
                StringBuilder sb = new();
                sb.AppendLine("[DungeonTrapBase] Detected Collision with no-hero object. which is not should happen");
                sb.AppendLine("Check if there has a no-hero obj set to wrong layer or collision layer is mis setted");
                sb.AppendLine("Info:" + collision.gameObject.name);
                GameFrameworkLog.Error(sb.ToString());
            }
#endif
            trap.OnBuildingEnter(low);
        }
        void OnTriggerEnter2D(Collider2D collision)
        {
            var low = collision.gameObject.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();
#if UNITY_EDITOR
            if (low == null)
            {
                StringBuilder sb = new();
                sb.AppendLine("[DungeonTrapBase] Detected Collision with no-hero object. which is not should happen");
                sb.AppendLine("Check if there has a no-hero obj set to wrong layer or collision layer is mis setted");
                sb.AppendLine("Info:" + collision.gameObject.name);
                GameFrameworkLog.Error(sb.ToString());
            }
#endif
            trap.OnBuildingEnter(low);
        }
    }
    [RequireComponent(typeof(BoxCollider2D),typeof(Rigidbody2D))]
    public class DungeonTrapEffectAreaCollider : MonoBehaviour
    {
        private DungeonTrapBase trap;
        private List<BoxCollider2D> boxes;
        public void Init(List<Vector2Int> area, DungeonTrapBase trapBase)
        {
            trap = trapBase;

            foreach(var pos in area)
            {
                var box = gameObject.AddComponent<BoxCollider2D>();
                box.size = Vector2.one;
                box.offset += (Vector2)pos * new Vector2(0.5f,0.5f);
            }
        }
        void OnTriggerEnter2D(Collider2D collision)
        {
            var low = collision.gameObject.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();
#if UNITY_EDITOR
            if (low == null)
            {
                StringBuilder sb = new();
                sb.AppendLine("[DungeonTrapBase] Detected Collision with no-hero object. which is not should happen");
                sb.AppendLine("Check if there has a no-hero obj set to wrong layer or collision layer is mis setted");
                sb.AppendLine("Info:" + collision.gameObject.name);
                GameFrameworkLog.Error(sb.ToString());
            }
#endif
            trap.OnEffectEnter(low);
        }
    }
    public abstract class DungeonTrapBase : DungeonVisibleEntity
    {
        [Header("基本设置")]
        [LabelText("陷阱名称")] public string trapName;
        [SerializeField, LabelText("可见检定"), Tooltip("用于检定是否能被看见")]
        protected DndCheckTarget mDndCheckTarget;

        [Space]
        [Header("网格设置")]

        [LabelText("陷阱大小")] public Vector2Int size = new(1, 1);
        [LabelText("陷阱是否可阻挡")] public bool isCollider = false;
        [HideInInspector] public List<Sprite> sprites = new();
        [ReadOnly] public List<Vector2Int> effectArea = new();

        public int GetSpriteIndex(Vector2Int pos)
        {
            return (pos.x < 0 || pos.x >= size.x || pos.y < 0 || pos.y >= size.y) ? -1 : pos.y * size.x + pos.x;
        }

        void Start()
        {
            InitCollider2D();
        }
        public void InitCollider2D()
        {
            CreateOrResetChild("BuildingCollider", size, isCollider);
            CreateOrResetChild("EffectAreaCollider", effectArea);
        }
        private void CreateOrResetChild(string name, Vector2Int size, bool isTrigger)
        {
            var go = new GameObject(name);
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            var collider = go.AddComponent<DungeonTrapBuildingCollier>();
            collider.Init(size, this, isTrigger);
        }
        private void CreateOrResetChild(string name, List<Vector2Int> area)
        {
            var go = new GameObject(name);
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            var collider = go.AddComponent<DungeonTrapEffectAreaCollider>();
            collider.Init(area, this);
        }

        public void OnEffectEnter(AgentLowLevelSystem.AgentLowLevelSystem agent)
        {

        }
        public void OnBuildingEnter(AgentLowLevelSystem.AgentLowLevelSystem agent)
        {

        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(DungeonTrapBase), true)]
    public class DungeonTrapEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var trap = (DungeonTrapBase)target;

            int totalCells = trap.size.x * trap.size.y;
            while (trap.sprites.Count < totalCells)
                trap.sprites.Add(null);
            if (trap.sprites.Count > totalCells)
                trap.sprites.RemoveRange(totalCells, trap.sprites.Count - totalCells);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("陷阱图块编辑", EditorStyles.boldLabel);
            DrawSpriteGrid(trap.size, trap);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("效果区域编辑", EditorStyles.boldLabel);
            DrawEffectAreaEditor(trap);

            if (GUI.changed)
            {
                Undo.RecordObject(trap, "Edit Trap Sprites");
                EditorUtility.SetDirty(trap);
            }
        }

        private void DrawSpriteGrid(Vector2Int size, DungeonTrapBase trap)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            for (int y = size.y - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < size.x; x++)
                {
                    int index = (size.y - 1 - y) * size.x + x;

                    GUILayout.BeginVertical(GUILayout.Width(72));
                    EditorGUILayout.LabelField($"({x},{y})", GUILayout.Width(64));
                    trap.sprites[index] = (Sprite)EditorGUILayout.ObjectField(trap.sprites[index], typeof(Sprite), false, GUILayout.Width(64), GUILayout.Height(64));
                    GUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawEffectAreaEditor(DungeonTrapBase trap)
        {
            var effectMin = new Vector2Int(0, 0);
            var effectMax = new Vector2Int(0, 0);

            if (trap.effectArea.Count > 0)
            {
                effectMin = trap.effectArea[0];
                effectMax = trap.effectArea[0];

                foreach (var pos in trap.effectArea)
                {
                    effectMin = Vector2Int.Min(effectMin, pos);
                    effectMax = Vector2Int.Max(effectMax, pos);
                }
                effectMax += Vector2Int.one;
            }

            var spriteMin = Vector2Int.zero;
            var spriteMax = trap.size;

            var min = Vector2Int.Min(spriteMin, effectMin);
            var max = Vector2Int.Max(spriteMax, effectMax);

            min -= Vector2Int.one;
            max += Vector2Int.one;

            for (int y = max.y - 1; y >= min.y; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = min.x; x < max.x; x++)
                {
                    Vector2Int pos = new(x, y);

                    bool isSpriteCell = (x >= 0 && x < trap.size.x && y >= 0 && y < trap.size.y);
                    bool isEffect = trap.effectArea.Exists(p => p.x == pos.x && p.y == pos.y);

                    Color originalColor = GUI.backgroundColor;
                    if (isSpriteCell)
                        GUI.backgroundColor = Color.cyan;
                    else if (isEffect)
                        GUI.backgroundColor = Color.green;
                    else
                        GUI.backgroundColor = Color.white;

                    GUILayout.BeginVertical(GUILayout.Width(48), GUILayout.Height(48));
                    GUILayout.Label($"({x},{y})", EditorStyles.miniLabel);
                    bool toggled = GUILayout.Toggle(isEffect, "", "Button", GUILayout.Width(42), GUILayout.Height(42));
                    GUILayout.EndVertical();

                    if (toggled != isEffect)
                    {
                        if (toggled)
                            trap.effectArea.Add(pos);
                        else
                            trap.effectArea.Remove(pos);
                    }

                    GUI.backgroundColor = originalColor;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
#endif
}


