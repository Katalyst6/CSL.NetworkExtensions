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
        public static Mesh Clone(this Mesh modMesh)
        {
            var newMesh = Object.Instantiate(modMesh);

            return newMesh;
        }
    }
}
