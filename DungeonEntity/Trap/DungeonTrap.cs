using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;
using Codice.Client.Common;
using DG.Tweening;
using Dungeon.AgentLowLevelSystem;
using Dungeon.DungeonEntity;
using Dungeon.DungeonGameEntry;
using Dungeon.SkillSystem;
using Dungeon.Vision2D;
using GameFramework;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;


#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace Dungeon.DungeonEntity.Trap
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class DungeonTrapBuildingCollier : MonoBehaviour
    {
        private DungeonTrapBase trap;
        private BoxCollider2D box;

        public void Init(Vector2Int size, DungeonTrapBase trapBase, Sprite sprite, bool isTrigger)
        {
#if UNITY_EDITOR
            if (LayerMask.NameToLayer("Trap") == -1)
                GameFrameworkLog.Error("[DungeonTrapBuildingCollier] No Collider Layer named 'Trap'");
            if (LayerMask.NameToLayer("DungeonStaticInteractiveEntity") == -1)
                GameFrameworkLog.Error("[DungeonTrapBuildingCollier] No Collider Layer named 'DungeonStaticInteractiveEntity'");
#endif
            trap = trapBase;
            gameObject.layer = LayerMask.NameToLayer(isTrigger ? "Trap" : "DungeonStaticInteractiveEntity");

            box = GetComponent<BoxCollider2D>();
            box.size = size;
            box.offset = (Vector2)size / 2f - Vector2.one / 2f;
            box.isTrigger = isTrigger;

            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            Transform spriteTransform = transform.Find("SpriteRenderer");
            GameObject spriteObj;
            SpriteRenderer sr;

            if (spriteTransform == null)
            {
                spriteObj = new GameObject("SpriteRenderer");
                spriteObj.transform.SetParent(transform);
                spriteObj.transform.localPosition = box.offset;
                sr = spriteObj.AddComponent<SpriteRenderer>();
            }
            else
            {
                spriteObj = spriteTransform.gameObject;
                sr = spriteObj.GetComponent<SpriteRenderer>() ?? spriteObj.AddComponent<SpriteRenderer>();
                spriteObj.transform.localPosition = box.offset;
            }

            sr.sprite = sprite;
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = size;
            sr.sortingOrder = 0;
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

    [RequireComponent(typeof(SkillShooter),typeof(BoxCollider2D),typeof(Rigidbody2D))]
    public abstract class DungeonTrapBase : DungeonVisibleEntity,ICombatable
    {
        public static readonly Dictionary<TrapDirection, float> DirectionToElurRotation = new Dictionary<TrapDirection, float>
        {
            {TrapDirection.Left, 0f},
            {TrapDirection.Up, -90f},
            {TrapDirection.Right, -180f},
            {TrapDirection.Down, -270f}
        };
        public void RotateToDirection(TrapDirection direction)
        {
            transform.rotation = Quaternion.Euler(0, 0, DirectionToElurRotation[direction]);
            transform.position = fourCorners[(int)direction];
        }
        public void RotateToDirection(TrapDirection direction, float timeToRotate)
        {
            var oriPos = transform.position;
            var oriRot = transform.rotation.eulerAngles.z;
            DOTween.To((float t) =>
            {
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(oriRot, DirectionToElurRotation[direction], t));
                transform.position = Vector3.Lerp(oriPos, fourCorners[(int)direction], t);
            },0,1,timeToRotate);
        }
        protected override void OnEnable()
        {
            DungeonGameEntry.DungeonGameEntry.DungeonEntityManager.RegisterDungeonEntity(this);

            gameObject.layer = LayerMask.NameToLayer("Trap");
            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            var box = GetComponent<BoxCollider2D>();
            box.isTrigger = false;

            m_SkillShooter = GetComponent<SkillShooter>();

            InitCollider2D();
            CalculateFourCorners();
        }
#if UNITY_EDITOR
        protected override void OnDestroy()
        {
            GameFrameworkLog.Warning("[Torch] Destroyed 清不要使用Destory而是对象池");
            DungeonGameEntry.DungeonGameEntry.DungeonEntityManager.UnregisterDungeonEntity(this);
        }
#endif

        private void InitCollider2D()
        {
            CreateOrResetChild("BuildingCollider", size, !isCollider);
            CreateOrResetChild("EffectAreaCollider", effectArea);
        }
        private void CalculateFourCorners()
        {
            fourCorners[0] = transform.position;
            fourCorners[1] = transform.position + new Vector3(0, size.y, 0);
            fourCorners[2] = transform.position + new Vector3(size.x, size.y, 0);
            fourCorners[3] = transform.position + new Vector3(size.x, 0, 0);
        }

        private void CreateOrResetChild(string name, Vector2Int size, bool isTrigger)
        {
            Transform childTransform = transform.Find(name);
            GameObject go;

            if (childTransform != null)
            {
                go = childTransform.gameObject;
            }
            else
            {
                go = new GameObject(name);
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
            }

            var collider = go.GetComponent<DungeonTrapBuildingCollier>() ?? go.AddComponent<DungeonTrapBuildingCollier>();
            collider.Init(size, this, sprite, isTrigger);
        }

        private void CreateOrResetChild(string name, List<Vector2Int> area)
        {
            Transform childTransform = transform.Find(name);
            GameObject go;

            if (childTransform != null)
            {
                go = childTransform.gameObject;
            }
            else
            {
                go = new GameObject(name);
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
            }

            var collider = go.GetComponent<DungeonTrapEffectAreaCollider>() ?? go.AddComponent<DungeonTrapEffectAreaCollider>();
            collider.Init(area, this);
        }

        public virtual void OnEffectEnter(AgentLowLevelSystem.AgentLowLevelSystem agent)
        {
            m_SkillShooter.Fire(skill, transform.position, agent.transform.position - transform.position);
        }

        public virtual void OnBuildingEnter(AgentLowLevelSystem.AgentLowLevelSystem agent)
        {
            return;
        }
        public override VisitInformation OnUnvisited(VisitInformation visitInfo)
        {
            visitInfo.visited = gameObject;
            return visitInfo;
        }

        public override VisitInformation OnVisited(VisitInformation visitInfo)
        {
            var visiter = visitInfo.visiter;
            var low = visiter.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();

            if (low == null)
                return visitInfo;

            var checkResut = low.DndCheck(mDndCheckTarget);
            if (checkResut.Succeed)
                visitInfo.visited = gameObject;

            return visitInfo;
        }

        public abstract GameObject GetGameObject();
        public abstract bool IsAlive();
        public abstract void OnKillSomebody(ICombatable killed);
        public abstract bool TakeSkill(Skill skill);
        public abstract void Stun(float duration);

        public enum TrapDirection
        {
            Up =0, // (0,0,) is up-left
            Down = 1, // (0,0,) is down-right
            Left = 2, // (0,0,) is down-left
            Right = 3 // (0,0,) is up-right
        }

        [Header("基本设置")]
        [LabelText("陷阱名称")] public string trapName;
        [LabelText("陷阱等级")] public int trapLevel;
        [SerializeField, LabelText("可见检定")] protected DndCheckTarget mDndCheckTarget;
        [SerializeField, LabelText("陷阱精灵")] protected Sprite sprite;

        [Space]
        [Header("功能设置")]
        [SerializeField, LabelText("陷阱效果")] protected SkillData skill;
        private SkillShooter m_SkillShooter;

        [Space]
        [Header("网格设置")]
        [LabelText("陷阱大小")] public Vector2Int size = new(1, 1);
        [LabelText("陷阱是否可阻挡")] public bool isCollider = false;
        [ReadOnly] public List<Vector2Int> effectArea = new();

        private TrapDirection currentDirection = TrapDirection.Left;
        private Vector3[] fourCorners = new Vector3[4];

        public abstract int Hp { get; set; }
        public abstract int MaxHp { get; set; }
        public abstract int Mp { get; set; }
        public abstract int MaxMp { get; set; }
        public abstract float AttackSpeed { get; set; }
        public abstract CombatorData BasicInfo { get; set; }
        public abstract StatusBarSetting StatusBarSetting { get; set; }
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(DungeonTrapBase), true)]
    public class DungeonTrapEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var trap = (DungeonTrapBase)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("效果区域编辑", EditorStyles.boldLabel);
            DrawEffectAreaEditor(trap);
            EditorGUILayout.LabelField("注意:设置时注意陷阱的方向,即注意0,0位置在哪一个角落");

            if (GUI.changed)
            {
                Undo.RecordObject(trap, "Edit Trap Sprites");
                EditorUtility.SetDirty(trap);
            }
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


