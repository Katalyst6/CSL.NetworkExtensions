using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkExtensions.Framework
{
    public class MeshData
    {
        public Vector3[] Vertices { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector2[] UVs { get; set; }
        public int[] Triangles { get; set; }

        public Mesh CreateMesh(string meshName)
        {
            var modMesh = new Mesh();

            modMesh.name = meshName;
            modMesh.Clear();
            modMesh.vertices = Vertices;
            modMesh.uv = UVs;
            modMesh.triangles = Triangles;
            modMesh.RecalculateBounds();
            modMesh.RecalculateNormals();
            modMesh.Optimize();

            return modMesh;
        }
    }
}
