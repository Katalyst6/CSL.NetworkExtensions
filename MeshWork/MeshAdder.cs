using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using NetworkExtensions.NewNetwork.Highway6L.Meshes;
using NetworkExtensions.NewNetwork.Highway2L.Meshes;
using NetworkExtensions.NewNetwork.SmallAvenue4L.Meshes;

namespace NetworkExtensions.MeshWork
{
    class MeshAdder
    {
        public static void BuildMeshes(NetCollection newRoads, List<NetInfo> newInfos)
        {
            //Set all new roads to their defaults
            newRoads.m_prefabs = newInfos.ToArray();
            newRoads.m_prefabs[0].m_segments[0].m_mesh = newInfos[0].m_segments[0].m_lodMesh;
            newRoads.m_prefabs[0].m_nodes[0].m_mesh = newInfos[0].m_nodes[0].m_lodMesh;

            newRoads.m_prefabs[6].m_segments[0].m_mesh = newInfos[6].m_segments[0].m_lodMesh;
            newRoads.m_prefabs[6].m_nodes[0].m_mesh = newInfos[6].m_nodes[0].m_lodMesh;

            newRoads.m_prefabs[1].m_segments[0].m_mesh = newInfos[1].m_segments[0].m_lodMesh;
            newRoads.m_prefabs[1].m_nodes[0].m_mesh = newInfos[1].m_nodes[0].m_lodMesh;

            List<int> newRoadIndices = new List<int>();
            List<string> newRoadNames = new List<string>()
            {
                "Large Highway",
                "Rural Highway",
                "Small Avenue"
            };
            var myString = "newRoadIndices: ";
            foreach (var roadName in newRoadNames)
            {
                newRoadIndices.Add(Array.IndexOf(newRoads.m_prefabs, newRoads.m_prefabs.Where(m => m.name == roadName).First()));
                myString += Array.IndexOf(newRoads.m_prefabs, newRoads.m_prefabs.Where(m => m.name == roadName).First()).ToString() + " ";
            }
            myString += "\r\n";
            //Modified meshes defaulted to mutable lod meshes
            foreach(var newRoadIndex in newRoadIndices)
            {
                //newRoads.m_prefabs[newRoadIndex].m_segments[0].m_mesh = newInfos[newRoadIndex].m_segments[0].m_lodMesh;
                //newRoads.m_prefabs[newRoadIndex].m_nodes[0].m_mesh = newInfos[newRoadIndex].m_nodes[0].m_lodMesh;
                myString += "lod mesh loaded for index: " + newRoadIndex.ToString() + " ";

                //Set vertical offsets of lanes (may make another method for this later)
                if (!(newRoads.m_prefabs[newRoadIndex].name == newRoadNames[2]))
                {
                    foreach (var lane in newRoads.m_prefabs[newRoadIndex].m_lanes)
                    {
                        lane.m_verticalOffset = 0.0f;
                    }
                    newRoads.m_prefabs[newRoadIndex].m_surfaceLevel = 0.0f;
                    myString += "This is the index of Small Avenue: " + newRoadIndex.ToString() + "\r\n";
                }
            }

            newRoads.m_prefabs[newRoadIndices[2]].m_halfWidth = 12;
            myString += "This is also the index of Small Avenue: " + newRoadIndices[2].ToString() + "\r\n";
            //Set vertices, triangles, uvs, and normals

            List<MeshAddendumModel> segmentMeshModels = new List<MeshAddendumModel>() { Highway6LSegmentModel.BuildMesh(), Highway2LSegmentModel.BuildMesh(), SmallAvenue4LSegmentModel.BuildMesh() };
            List<MeshAddendumModel> nodeMeshModels = new List<MeshAddendumModel>() { Highway6LNodeModel.BuildMesh(), Highway2LNodeModel.BuildMesh(), SmallAvenue4LNodeModel.BuildMesh() };
            List<string> segmentNames = new List<string>() { "HW_6L_Segment0_Grnd", "HW_2L_Segment0_Grnd", "AV_4L_SM_Segment0_Grnd" };
            List<string> nodeNames = new List<string>() { "HW_6L_Node0_Grnd", "HW_2L_Node0_Grnd", "AV_4L_SM_Node0_Grnd" };

            for (var i = 0; i < newRoadIndices.Count(); i++)
            {
                newRoads.m_prefabs[newRoadIndices[i]].m_segments[0].m_mesh.name = segmentNames[i];
                newRoads.m_prefabs[newRoadIndices[i]].m_nodes[0].m_mesh.name = nodeNames[i];

                newRoads.m_prefabs[newRoadIndices[i]].m_segments[0].m_mesh.Clear();
                newRoads.m_prefabs[newRoadIndices[i]].m_nodes[0].m_mesh.Clear();

                newRoads.m_prefabs[newRoadIndices[i]].m_segments[0].m_mesh.vertices = segmentMeshModels[i].Vertices;
                newRoads.m_prefabs[newRoadIndices[i]].m_nodes[0].m_mesh.vertices = nodeMeshModels[i].Vertices;

                newRoads.m_prefabs[newRoadIndices[i]].m_segments[0].m_mesh.uv = segmentMeshModels[i].UVs;
                newRoads.m_prefabs[newRoadIndices[i]].m_nodes[0].m_mesh.uv = nodeMeshModels[i].UVs;

                newRoads.m_prefabs[newRoadIndices[i]].m_segments[0].m_mesh.triangles = segmentMeshModels[i].Triangles;
                newRoads.m_prefabs[newRoadIndices[i]].m_nodes[0].m_mesh.triangles = nodeMeshModels[i].Triangles;

                newRoads.m_prefabs[newRoadIndices[i]].m_segments[0].m_mesh.RecalculateBounds();
                newRoads.m_prefabs[newRoadIndices[i]].m_nodes[0].m_mesh.RecalculateBounds();

                newRoads.m_prefabs[newRoadIndices[i]].m_segments[0].m_mesh.RecalculateNormals();
                newRoads.m_prefabs[newRoadIndices[i]].m_nodes[0].m_mesh.RecalculateNormals();
            }
        }
    }
}
