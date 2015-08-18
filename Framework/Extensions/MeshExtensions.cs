using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkExtensions.Framework
{
    public static class MeshExtensions
    {
        public static void Setup(this Mesh modMesh, MeshAddendumModel model, string meshName)
        {
            modMesh.name = meshName;
            modMesh.Clear();
            modMesh.vertices = model.Vertices;
            modMesh.uv = model.UVs;
            modMesh.triangles = model.Triangles;
            modMesh.RecalculateBounds();
            modMesh.RecalculateNormals();
        }
    }
}
