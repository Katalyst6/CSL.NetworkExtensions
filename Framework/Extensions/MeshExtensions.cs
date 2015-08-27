using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NetworkExtensions.Framework
{
    public static class MeshExtensions
    {
        [Obsolete("Use MeshModel.CreateMesh")]
        public static void Setup(this Mesh modMesh, MeshModel model, string meshName)
        {
            modMesh.name = meshName;
            modMesh.Clear();
            modMesh.vertices = model.Vertices;
            modMesh.uv = model.UVs;
            modMesh.triangles = model.Triangles;
            modMesh.RecalculateBounds();
            modMesh.RecalculateNormals();
        }

        public static Mesh Clone(this Mesh modMesh)
        {
            var newMesh = Object.Instantiate(modMesh);

            return newMesh;
        }
    }
}
