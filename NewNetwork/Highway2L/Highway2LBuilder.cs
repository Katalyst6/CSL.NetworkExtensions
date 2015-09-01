using System;
using System.Linq;
using NetworkExtensions.Framework;
using NetworkExtensions.NewNetwork.Highway2L.Meshes;
using UnityEngine;
using UnityExtension;
using System.IO;

namespace NetworkExtensions.NewNetwork.Highway2L
{
    public class Highway2LBuilder : ModPart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 40; } }
        public int Priority { get { return 10; } }

        public string PrefabName  { get { return "Oneway Road"; } }
        public string Name        { get { return "Rural Highway"; } }
        public string CodeName    { get { return "HIGHWAY_2L"; } }
        public string Description { get { return "A two-lane, one-way road suitable for low to medium traffic between rural areas. Lanes going the opposite direction need to be built separately. Highway does not allow zoning next to it!"; } }
        public string UICategory  { get { return "RoadsHighway"; } }

        public string ThumbnailsPath { get { return @"NewNetwork\Highway2L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"NewNetwork\Highway2L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public string GetPrefabName(NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    return PrefabName;
                case NetInfoVersion.Elevated:
                    return PrefabName + " " + NetInfoVersion.Elevated;
                case NetInfoVersion.Bridge:
                    return PrefabName + " " + NetInfoVersion.Bridge;
                case NetInfoVersion.Tunnel:
                    return PrefabName + " " + NetInfoVersion.Tunnel;
                case NetInfoVersion.Slope:
                    return PrefabName + " " + NetInfoVersion.Slope;
                default:
                    throw new NotImplementedException();
            }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = ToolsCSL.FindPrefab<NetInfo>("Highway");


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                info.m_surfaceLevel = 0;
                info.m_class = highwayInfo.m_class.Clone("RuralHighway");

                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;

                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_forwardRequired = NetSegment.Flags.None;

                var nodes1 = nodes0.ShallowClone();

                nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                nodes0.m_flagsRequired = NetNode.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.Transition;

                var grndMesh = Highway2LMeshes.GetGroundData().CreateMesh("HIGHWAY_2L_GROUND");
                var grndTransMesh = Highway2LMeshes.GetGroundTransitionData().CreateMesh("HIGHWAY_2L_GROUND_TRS");

                segments0.m_mesh = grndMesh;
                nodes0.m_mesh = grndMesh;
                nodes1.m_mesh = grndTransMesh;

                var grndLODMesh = new Mesh();
                grndLODMesh.LoadOBJ(OBJLoader.LoadOBJ(File.Open(Path.Combine(Mod.GetPath(), @"NewNetwork\Highway2L\Meshes\Grnd_SegmentLOD.obj"), FileMode.Open)));

                var grndHwTransLODMesh = new Mesh();
                grndHwTransLODMesh.LoadOBJ(OBJLoader.LoadOBJ(File.Open(Path.Combine(Mod.GetPath(), @"NewNetwork\Highway2L\Meshes\Grnd_HwTrans_NodeLOD.obj"), FileMode.Open)));

                var grndRdTransLODMesh = new Mesh();
                grndRdTransLODMesh.LoadOBJ(OBJLoader.LoadOBJ(File.Open(Path.Combine(Mod.GetPath(), @"NewNetwork\Highway2L\Meshes\Grnd_RdTrans_NodeLOD.obj"), FileMode.Open)));

                segments0.m_lodMesh = grndLODMesh;
                nodes0.m_lodMesh = grndHwTransLODMesh;
                nodes1.m_lodMesh = grndRdTransLODMesh;

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }


            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetSegmentsTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway2L\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\Highway2L\Textures\Ground_Segment__AlphaMap.png"));
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway2L\Textures\Ground_Node__MainTex.png",
                            @"NewNetwork\Highway2L\Textures\Ground_Node__AlphaMap.png"),
                        new TexturesSet
                           (@"NewNetwork\Highway2L\Textures\Ground_NodeLOD__MainTex.png",
                            @"NewNetwork\Highway2L\Textures\Ground_NodeLOD__AlphaMap.png",
                            @"NewNetwork\Highway2L\Textures\Ground_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway2L\Textures\Elevated_Node__MainTex.png",
                            @"NewNetwork\Highway2L\Textures\Elevated_Node__AlphaMap.png"));
                        // Lets leave the crossings there until we have a fix
                        //new TexturesSet
                        //   (@"NewNetwork\Highway2L\Textures\Elevated_NodeLOD__MainTex.png",
                        //    @"NewNetwork\Highway2L\Textures\Elevated_NodeLOD__AlphaMap.png",
                        //    @"NewNetwork\Highway2L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway2L\Textures\Slope_Node__MainTex.png",
                            @"NewNetwork\Highway2L\Textures\Slope_Node__AlphaMap.png"),
                        new TexturesSet
                           (@"NewNetwork\Highway2L\Textures\Slope_NodeLOD__MainTex.png",
                            @"NewNetwork\Highway2L\Textures\Slope_NodeLOD__AlphaMap.png",
                            @"NewNetwork\Highway2L\Textures\Slope_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Tunnel:
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            info.m_createPavement = (version == NetInfoVersion.Slope);
            info.m_createGravel = (version == NetInfoVersion.Ground);
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;

            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;


            // Disabling Parkings and Peds
            foreach (var l in info.m_lanes)
            {
                switch (l.m_laneType)
                {
                    case NetInfo.LaneType.Parking:
                        l.m_laneType = NetInfo.LaneType.None;
                        break;
                    case NetInfo.LaneType.Pedestrian:
                        l.m_laneType = NetInfo.LaneType.None;
                        break;
                }
            }

            // Setting up lanes
            var vehiculeLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();

            for (int i = 0; i < vehiculeLanes.Length; i++)
            {
                var l = vehiculeLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;

                if (version == NetInfoVersion.Ground)
                {
                    l.m_verticalOffset = 0f;
                }
            }


            if (version == NetInfoVersion.Ground)
            {
                var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (hwPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 2 / 3;
                    playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 2 / 3;
                }
            }
            else // Same as the original oneway
            {

            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_highwayRules = true;
                roadBaseAI.m_trafficLights = false;
            }

            var roadAI = info.GetComponent<RoadAI>();

            if (roadAI != null)
            {
                roadAI.m_enableZoning = false;
            }

            info.SetHighwayProps(highwayInfo);
            info.TrimHighwayProps();
        }
    }
}
