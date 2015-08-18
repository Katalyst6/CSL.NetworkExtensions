using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using NetworkExtensions.NewNetwork.Highway6L.Meshes;

namespace NetworkExtensions.MeshWork
{
    class MeshAdder
    {
        public static void BuildMesh(NetCollection newRoads, List<NetInfo> newInfos)
        {
            //Set all new roads to their defaults
            newRoads.m_prefabs = newInfos.ToArray();

            //Modified meshes defaulted to mutable lod meshes
            newRoads.m_prefabs[5].m_segments[0].m_mesh = newInfos[5].m_segments[0].m_lodMesh;
            newRoads.m_prefabs[5].m_nodes[0].m_mesh = newInfos[5].m_nodes[0].m_lodMesh;

            //Set vertical offsets of lanes (may make another method for this later)
            foreach(var lane in newRoads.m_prefabs[5].m_lanes)
            {
                lane.m_verticalOffset = 0.0f;
            }
            newRoads.m_prefabs[5].m_surfaceLevel = 0.0f;

            //Set vertices, triangles, uvs, and normals
            BuildHelper(newRoads.m_prefabs[5].m_segments[0].m_mesh, Highway6LSegmentModel.BuildMesh(), "HW_6L_Segment0_Grnd");
            BuildHelper(newRoads.m_prefabs[5].m_nodes[0].m_mesh, Highway6LNodeModel.BuildMesh(), "HW_6L_Node0_Grnd");
        }

        private static void BuildHelper(Mesh modMesh, MeshAddendumModel model, string meshName)
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
