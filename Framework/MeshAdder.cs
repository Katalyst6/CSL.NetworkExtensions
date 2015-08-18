namespace NetworkExtensions.Framework
{
    // Dont base it on the id "ie: 5", those ids are volatile and are going to change
    //public class MeshAdder
    //{
    //    public static void BuildMesh(NetCollection newRoads, List<NetInfo> newInfos)
    //    {
    //        //Set all new roads to their defaults
    //        newRoads.m_prefabs = newInfos.ToArray();

    //        //Modified meshes defaulted to mutable lod meshes
    //        newRoads.m_prefabs[5].m_segments[0].m_mesh = newInfos[5].m_segments[0].m_lodMesh;
    //        newRoads.m_prefabs[5].m_nodes[0].m_mesh = newInfos[5].m_nodes[0].m_lodMesh;

    //        newRoads.m_prefabs[0].m_segments[0].m_mesh = newInfos[0].m_segments[0].m_lodMesh;
    //        newRoads.m_prefabs[0].m_nodes[0].m_mesh = newInfos[0].m_nodes[0].m_lodMesh;

    //        //Set vertical offsets of lanes (may make another method for this later)
    //        foreach(var lane in newRoads.m_prefabs[0].m_lanes)
    //        {
    //            lane.m_verticalOffset = 0.0f;
    //        }
    //        newRoads.m_prefabs[0].m_surfaceLevel = 0.0f;

    //        foreach (var lane in newRoads.m_prefabs[5].m_lanes)
    //        {
    //            lane.m_verticalOffset = 0.0f;
    //        }
    //        newRoads.m_prefabs[5].m_surfaceLevel = 0.0f;

    //        //Set vertices, triangles, uvs, and normals
    //        BuildHelper(newRoads.m_prefabs[5].m_segments[0].m_mesh, Highway2LSegmentModel.BuildMesh(), "HW_2L_Segment0_Grnd");
    //        BuildHelper(newRoads.m_prefabs[5].m_nodes[0].m_mesh, Highway2LNodeModel.BuildMesh(), "HW_2L_Node0_Grnd");

    //        BuildHelper(newRoads.m_prefabs[0].m_segments[0].m_mesh, Highway6LSegmentModel.BuildMesh(), "HW_6L_Segment0_Grnd");
    //        BuildHelper(newRoads.m_prefabs[0].m_nodes[0].m_mesh, Highway6LNodeModel.BuildMesh(), "HW_6L_Node0_Grnd");
    //    }

    //    public static void BuildHelper(Mesh modMesh, MeshAddendumModel model, string meshName)
    //    {
    //        modMesh.name = meshName;
    //        modMesh.Clear();
    //        modMesh.vertices = model.Vertices;
    //        modMesh.uv = model.UVs;
    //        modMesh.triangles = model.Triangles;
    //        modMesh.RecalculateBounds();
    //        modMesh.RecalculateNormals();
    //    }
    //}
}
