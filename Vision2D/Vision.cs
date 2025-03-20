using System;
using Dungeon.AgentLowLevelSystem;
using Dungeon.BlackBoardSystem;
using GameFramework;
using UnityEngine;

namespace Dungeon.Vision2D
{
    [RequireComponent(typeof(PolygonCollider2D),typeof(MeshRenderer),typeof(MeshFilter))]
    public class Vision : MonoBehaviour
    {
        void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            polyCollider = GetComponent<PolygonCollider2D>();
            polyCollider.isTrigger = true;

            var layer = LayerMask.NameToLayer("Vision");
            if (layer == -1)
                GameFrameworkLog.Error("[Vision] Vision layer not found. Please create a layer named 'Vision' in the project settings.");
            else
                gameObject.layer = layer;

            m_Blackboard = transform.parent.GetComponent<BlackboardController>().GetBlackboard();
            if (m_Blackboard == null)
                GameFrameworkLog.Error("[Vision] Vision must be a child of a GameObject with a BlackboardController component.");
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

        // public void OnTriggerEnter(Collider other)
        // {
        //     dynamic visible = other.GetComponent<IVisible>();
        //     if(visible == null)
        //         return;
            
        //     var key = m_Blackboard.GetOrRegisterKey(AgentBlackBoardEnum.GetNameOfIVisibleCountInVision(visible.GetType().Name));
        //     m_Blackboard.TryGetValue(key, out int count);
        //     m_Blackboard.SetValue(key,++count);
            
        //     OnVisionEnter?.Invoke(other.GetComponent<IVisible>()); 
        // }
        // public void OnTriggerExit(Collider other)
        // {
        //     dynamic visible = other.GetComponent<IVisible>();
        //     if(visible == null)
        //         return;
            
        //     var key = m_Blackboard.GetOrRegisterKey(AgentBlackBoardEnum.GetNameOfIVisibleCountInVision(visible.GetType().Name));
        //     m_Blackboard.TryGetValue(key, out int count);
        //     m_Blackboard.SetValue(key,--count);

        //     #if UNITY_EDITOR
        //     if(count < 0)
        //         GameFrameworkLog.Error("[Vision] Count of " + visible.GetType().Name + " in Vision is less than 0.");
        //     #endif
            
        //     OnVisionEnter?.Invoke(other.GetComponent<IVisible>());  
        // }
        public event Action<IVisible> OnVisionEnter;
        public event Action<IVisible> OnVisionExit;
        private Blackboard m_Blackboard;

        [HideInInspector] public MeshFilter meshFilter;
        [HideInInspector] public PolygonCollider2D polyCollider;
    }
}
