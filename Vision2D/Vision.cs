using System;
using UnityEngine;
using UnityGameFramework.Runtime;

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
                Log.Error("Vision layer not found. Please create a layer named 'Vision' in the project settings.");
            else
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

        public void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<IVisible>() == null)
                return;
            
            OnVisionEnter?.Invoke(other.GetComponent<IVisible>()); 
        }
        public void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<IVisible>() == null)
                return;
            
            OnVisionExit?.Invoke(other.GetComponent<IVisible>()); 
        }
        public event Action<IVisible> OnVisionEnter;
        public event Action<IVisible> OnVisionExit;

        [HideInInspector] public MeshFilter meshFilter;
        [HideInInspector] public PolygonCollider2D polyCollider;
    }
}
