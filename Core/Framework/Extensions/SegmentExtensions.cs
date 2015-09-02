using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkExtensions.Framework
{
    public static class SegmentExtensions
    {
        public static NetInfo.Segment SetMeshes(this NetInfo.Segment segment, string newMeshPath, string newLODMeshPath = null)
        {
            segment.m_mesh = AssetManager.instance.GetMesh(newMeshPath);

            if (newLODMeshPath != null)
            {
                segment.m_lodMesh = AssetManager.instance.GetMesh(newLODMeshPath);
            }

            return segment;
        }
    }
}
