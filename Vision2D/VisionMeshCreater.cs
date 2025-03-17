using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.Vision2D
{
    public class VisionMeshCreater
    {
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private Vector2 center;

        public void SetCenter(Vector2 center)
        {
            this.center = center;
        }

        public void AddNewTriangle(Vector2 pos1, Vector2 pos2)
        {
            vertices.Add(center);
            vertices.Add(pos1 + center);
            vertices.Add(pos2 + center);

            triangles.Add(triangles.Count);
            triangles.Add(triangles.Count);
            triangles.Add(triangles.Count);
        }

        public Mesh GetMesh()
        {
            var mesh = new Mesh();
            mesh.name = "Vision";
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            return mesh;
        }

        public void Clear()
        {
            vertices.Clear();
            triangles.Clear();
        }
    }
}