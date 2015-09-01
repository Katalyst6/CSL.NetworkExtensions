using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkExtensions.Framework
{
    public static class NodeExtensions
    {
        public static NetInfo.Node SetMeshes(this NetInfo.Node node, string newMeshPath, string newLODMeshPath = null)
        {
            node.m_mesh = AssetManager.instance.GetMesh(newMeshPath);

            if (newLODMeshPath != null)
            {
                node.m_lodMesh = AssetManager.instance.GetMesh(newLODMeshPath);
            }

            return node;
        }
    }
}
