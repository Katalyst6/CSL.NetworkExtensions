using UnityEngine;

namespace NetworkExtensions.Framework
{
    public class MeshAddendumModel
    {
        public Vector3[] Vertices { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector2[] UVs { get; set; }
        public int[] Triangles { get; set; }
    }
}
