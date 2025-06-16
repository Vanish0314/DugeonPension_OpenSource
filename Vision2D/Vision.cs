using System;
using System.Collections.Generic;
using Dungeon.Character;
using Dungeon.BlackBoardSystem;
using GameFramework;
using UnityEngine;
using Dungeon.DungeonEntity;

namespace Dungeon.Vision2D
{
    [RequireComponent(typeof(PolygonCollider2D), typeof(MeshRenderer), typeof(MeshFilter))]
    public class Vision : MonoBehaviour
    {
        void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            polyCollider = GetComponent<PolygonCollider2D>();
            polyCollider.isTrigger = true;

            var layer = LayerMask.NameToLayer("Vision");
            m_Blackboard = transform.parent.GetComponent<BlackboardController>().GetBlackboard();

#if UNITY_EDITOR
            if (layer == -1)
                GameFrameworkLog.Error("[Vision] Vision layer not found. Please create a layer named 'Vision' in the project settings.");

            if (m_Blackboard == null)
                GameFrameworkLog.Error("[Vision] Vision must be a child of a GameObject with a BlackboardController component.");
#endif

            gameObject.layer = layer;
        }

        public void RegenerateCollider()
        {
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices3D = mesh.vertices;
            Vector2[] vertices2D = new Vector2[vertices3D.Length];

            for (int i = 0; i < vertices3D.Length; i++)
            {
                vertices2D[i] = new Vector2(vertices3D[i].x, vertices3D[i].y);
            }

            polyCollider.SetPath(0, vertices2D);
        }

        public void Init(AgentLowLevelSystem low)
        {
            mOwner = low;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            GameFrameworkLog.Info($"[Vision] {transform.parent.name}的视野与{other.name}物体相交,请考虑是否应该相交");

            var visible = other.GetComponent<IVisible>();
            if (visible == null)
                return;

            GameFrameworkLog.Info($"[Vision] {other.gameObject.name} 进入了 {transform.parent.name}的视野.[仅进入视野范围]");

            var visitInfo = visible.OnVisited(new VisitInformation(transform.parent.gameObject, null));
            if (visitInfo.visited != null)
            {
                GameFrameworkLog.Info($"[Vision] {transform.parent.name} 看到了 {other.gameObject.name}[通过了察觉鉴定]");

                mOwner.OnSee(other.gameObject);
                {
                    var key = m_Blackboard.GetOrRegisterKey(AgentBlackBoardEnum.GetNameOfIVisibleCountInVision(visible.GetType().Name));
                    m_Blackboard.TryGetValue(key, out int count);
                    m_Blackboard.SetValue(key, ++count);

                    OnSee?.Invoke(other.gameObject);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var visible = other.GetComponent<IVisible>();
            if (visible == null)
                return;

            GameFrameworkLog.Info($"[Vision] {other.gameObject.name}离开了 {transform.parent.name} 的视野范围.[仅离开视野范围]");

            mOwner.OnUnSee(other.gameObject);
            {
                visible.OnUnvisited(new VisitInformation(transform.parent.gameObject, null));

                var key = m_Blackboard.GetOrRegisterKey(AgentBlackBoardEnum.GetNameOfIVisibleCountInVision(visible.GetType().Name));
                m_Blackboard.TryGetValue(key, out int count);
                m_Blackboard.SetValue(key, --count);

#if UNITY_EDITOR
                if (count < 0)
                {
                    GameFrameworkLog.Error($"[Vision] {visible.GetType().Name} 再视野中的数量<0, 请检查代码逻辑.");
                    m_Blackboard.SetValue(key, 0);
                }
#endif

                OnLoseVisionOf?.Invoke(other.gameObject);
            }
        }
        public event Action<GameObject> OnSee;
        public event Action<GameObject> OnLoseVisionOf;

        private Blackboard m_Blackboard;
        private AgentLowLevelSystem mOwner;
        [HideInInspector] public MeshFilter meshFilter;
        [HideInInspector] public PolygonCollider2D polyCollider;
    }
}
